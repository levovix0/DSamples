using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;
using NAudio;
using NAudio.Wave;
using Path = System.IO.Path;

namespace DSamples
{
    public static class ExtensionMethods
    {
        public static T XamlClone<T>(this T original)
            where T : class
        {
            if (original == null)
                return null;

            object clone;
            using (var stream = new MemoryStream())
            {
                XamlWriter.Save(original, stream);
                stream.Seek(0, SeekOrigin.Begin);
                clone = XamlReader.Load(stream);
            }

            if (clone is T)
                return (T)clone;
            else
                return null;
        }
    }

    public partial class SampleBox : UserControl
    {
        DispatcherTimer timer = new DispatcherTimer();

        public static SampleBox Create(string filename)
        {
            SampleBox box = new SampleBox();
            box.Width = 290;
            box.Height = 32;
            box.Margin = new Thickness(0, 0, 0, 10);
            box.BindWithFile(filename);

            return box;
        }
        
        public static List<float> LoadWaveform(string filename, int lenght)
        {
            var reader = new WaveChannel32(new AudioFileReader(filename));
            var res = new List<float>();

            int step = (int)(reader.Length / lenght);
            step -= step % 4;

            const int sampleSize = 1024;
            var buffer = new byte[step];
            while (reader.Position < reader.Length)
            {
                int readed = reader.Read(buffer, 0, step);
                float avg = 0;
                for (int i = 0; i < readed / sampleSize; i++)
                {
                    var point = BitConverter.ToSingle(buffer, i * sampleSize);
                    avg += Math.Abs(point);
                }
                avg /= (readed / sampleSize);
                res.Add(avg);
            }
            reader.Close();

            float max = Single.MinValue;
            float min = Single.MaxValue;
            foreach (var re in res)
            {
                if (re > max) max = re;
                if (re < max) min = re;
            }
            for (int i = 0; i < res.Count; ++i)
            {
                res[i] = (res[i] - min) / (max - min);
            }

            return res;
        }

        public void BuildWaveform(List<float> input)
        {
             WaveformPannel.Children.Clear();
             foreach (var f in input)
             {
                 var x = (Application.Current.Resources["WavePiece"] as Grid).XamlClone();
                 x.Height = /*WaveformHeight*/ 24 * f;
                 WaveformPannel.Children.Add(x);
             }
        }

        public string file;

        public void BindWithFile(string filename)
        {
            file = filename;
            NameText.Content = Path.GetFileNameWithoutExtension(filename);
            BuildWaveform(LoadWaveform(file, (int)(/*WaveformWidth*/ 107 / 5)));
        }
        
        public SampleBox()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(1000 / 30);
            timer.Tick += OnUpdate;
            MusicPlayer.OnMusicStop += delegate
            {
                timer.Stop();
                ResetWaveformProgress();
                // кнопка [||] -> [|>]
                PlayBtn.Children[0].Visibility = Visibility.Visible;
                PlayBtn.Children[1].Visibility = Visibility.Collapsed;
                PlayBtn.Children[2].Visibility = Visibility.Collapsed;
            };
        }

        private bool _is_clicking = false;

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _is_clicking = true;
        }

        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_is_clicking)
            {
                PlayOrStop();
            }

            _is_clicking = false;
        }

        void PlayOrStop()
        {
            if (MusicPlayer.State == PlaybackState.Stopped)
            {
                Play();
            }
            else
            {
                MusicPlayer.Stop();
                if (!object.ReferenceEquals(MusicPlayer.sender, this))
                    Play();
            }
        }

        void Play()
        {
            MusicPlayer.Stop();
            MusicPlayer.OpenFile(file, this);
            
            MusicPlayer.Play();
            timer.Start();
            // кнопка [|>] -> [||]
            PlayBtn.Children[0].Visibility = Visibility.Collapsed;
            PlayBtn.Children[1].Visibility = Visibility.Visible;
            PlayBtn.Children[2].Visibility = Visibility.Visible;
        }

        private void OnUpdate(object sender, EventArgs e)
        {
            // TODO: текущий вариант тормозить, реализовать через собственный рендеринг
            // update waveform progress
            if (MusicPlayer.State == PlaybackState.Stopped)
            {
                ResetWaveformProgress();
                return;
            }
            var pos = (int)(MusicPlayer.GetT() * WaveformPannel.Children.Count);
            for (int i = 0; i < WaveformPannel.Children.Count; i++)
            {
                var rect = (WaveformPannel.Children[i] as Grid).Children[0] as Rectangle;
                if (i <= pos)
                {
                    rect.Fill = Application.Current.Resources["WavePieceColor2"] as Brush;
                }
                else
                {
                    rect.Fill = Application.Current.Resources["WavePieceColor"] as Brush;
                }
            }
        }

        private void ResetWaveformProgress()
        {
            foreach (var a in WaveformPannel.Children)
            {
                var rect = (a as Grid).Children[0] as Rectangle;
                var anim = new ColorAnimation(
                    (rect.Fill as SolidColorBrush).Color, 
                    (Application.Current.Resources["WavePieceColor"] as SolidColorBrush).Color, 
                    new Duration(TimeSpan.FromMilliseconds(200)));
                
                anim.FillBehavior = FillBehavior.Stop;
                var brh = new SolidColorBrush((Application.Current.Resources["WavePieceColor"] as SolidColorBrush).Color);
                brh.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                rect.Fill = brh;
                anim.Completed += delegate(object sender, EventArgs args)
                {
                    brh.BeginAnimation(SolidColorBrush.ColorProperty, null);
                };
            }
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            _is_clicking = false;
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var path = Path.GetFullPath(file);
                var dataObject = new DataObject(DataFormats.FileDrop, new string[]{path});
                dataObject.SetData(DataFormats.StringFormat, dataObject);
                DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
            }
        }
    }
}
