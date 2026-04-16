using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                player.Open(new Uri(path));
                player.Play();
            }
            catch
            {
                System.Windows.MessageBox.Show("Error playing file");
            }
        }

        public void Pause() => player.Pause();
        public void Stop() => player.Stop();
    }
}
