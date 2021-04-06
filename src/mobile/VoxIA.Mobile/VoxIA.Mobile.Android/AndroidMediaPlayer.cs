using Android.Media;
using System.Threading.Tasks;
using VoxIA.Mobile.Models;
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

        public async Task InitializeAsync(Song song)
        {
            Id3MetadataRetriever metadataRetriever = new Id3MetadataRetriever();
            await metadataRetriever.PopulateMetadataAsync(song);

            _player.Reset();
            await _player.SetDataSourceAsync(song.Url);
            _player.Prepare();
        }

        public void Pause()
        {
            _player.Pause();
        }

        public void Play()
        {
            _player.Start();
        }
    }
}