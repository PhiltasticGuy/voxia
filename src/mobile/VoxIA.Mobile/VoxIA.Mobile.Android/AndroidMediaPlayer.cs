using Android.Media;
using System;
using System.Threading.Tasks;
using VoxIA.Core.Media;

namespace VoxIA.Mobile.Droid
{
    public class AndroidMediaPlayer : IMediaPlayer
    {
        private readonly MediaPlayer _player;

        public bool IsPlaying => _player?.IsPlaying == true;

        public string CurrentlyPlayingSongId => throw new NotImplementedException();

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

        public async Task InitializeAsync(Uri uri)
        {
            Id3MetadataRetriever metadataRetriever = new Id3MetadataRetriever();
            await metadataRetriever.PopulateMetadataAsync(uri);

            _player.Reset();
            await _player.SetDataSourceAsync(uri.AbsoluteUri);
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

        public void DeafenVolume()
        {
            // NOT SUPPORTED BY PLAYER!
        }

        public void ResetVolume()
        {
            // NOT SUPPORTED BY PLAYER!
        }

        public Task InitializeAsync(string songId, Uri uri)
        {
            throw new NotImplementedException();
        }
    }
}