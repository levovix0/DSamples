using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace DSamples
{
    static class MusicPlayer
    {
        private static WaveOut _waveOut = new WaveOut();
        private static WaveFileReader _waveIn;
        public static object sender;

        public static float GetT()
        {
            if (_waveIn == null || _waveIn.Length == 0) return 0;
            return (float)_waveIn.Position / (float)_waveIn.Length;
        }

        static MusicPlayer()
        {
            _waveOut.Volume = 0.5f;
            _waveOut.PlaybackStopped += delegate
            {
                Stop();
            };
        }

        public static void OpenFile(string filename, object sender)
        {
            MusicPlayer.sender = sender;
            _waveIn = new WaveFileReader(filename);
            _waveOut.Init(_waveIn);
        }

        public static PlaybackState State => _waveOut.PlaybackState;

        public static void Play()
        {
            _waveIn.Position = 0;
            _waveOut.Play();
        }

        public static event EventHandler OnMusicStop;
        public static void Stop()
        {
            if (_waveIn == null) return;
            _waveOut.Stop();
            _waveIn.Close();
            _waveIn = null;
            OnMusicStop?.Invoke(sender, null);
        }
    }
}
