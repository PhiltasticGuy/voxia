using Android.Media;
using VoxIA.Mobile.Services;

namespace VoxIA.Mobile.Droid
{
    public class AndroidMediaPlayer : IMediaPlayer
    {
        private readonly MediaPlayer _player;

        public AndroidMediaPlayer()
        {
            _player = new MediaPlayer();
        }

        ~AndroidMediaPlayer()
        {
            if (_player != null)
            {
                _player.Release();
                _player.Dispose();
            }
        }

        public void Pause()
        {
            _player.Pause();
        }

        public void Play(string url)
        {
            _player.Reset();
            _player.SetDataSource(url);
            _player.Prepare();
            _player.Start();
        }
    }
}