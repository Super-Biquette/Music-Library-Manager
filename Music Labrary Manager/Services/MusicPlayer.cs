using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Music_Labrary_Manager.Services
{
    public class MusicPlayer
    {
        private MediaPlayer player = new MediaPlayer();

        public void Play(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    MessageBox.Show("File not found");
                    return;
                }
                string fullpath = Path.GetFullPath(path);
                player.Open(new Uri(fullpath, UriKind.Absolute));
                player.Play();
            }
            catch
            {
                MessageBox.Show("Error playing file");
            }
        }

        public void SetVolume(double  volume)
        { player.Volume = volume; }
        public void Stop() => player.Stop();
    }
}
