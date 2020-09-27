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
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio;
using NAudio.Wave;

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
                 x.Height = /*ActualHeight*/ 24 * f;
                 WaveformPannel.Children.Add(x);
             }
        }

        public string file;

        public SampleBox()
        {
            InitializeComponent();
            BuildWaveform(LoadWaveform("x.wav", (int)(/*ActualWidth*/ 107 / 5)));
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
                // TODO: play the music
            }
            _is_clicking = false;
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            _is_clicking = false;
        }
    }
}
