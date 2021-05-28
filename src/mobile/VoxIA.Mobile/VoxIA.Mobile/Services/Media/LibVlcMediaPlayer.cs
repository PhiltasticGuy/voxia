using LibVLCSharp.Shared;
using System;
using System.IO;
using System.Threading.Tasks;
using VoxIA.Core.Media;
using Xamarin.Forms;

namespace VoxIA.Mobile.Services.Media
{
    public class LibVlcMediaPlayer : IMediaPlayer
    {
        private readonly MediaPlayer _player;
        private readonly LibVLC _vlc;
        private int _volume;
        private string _currentlyPlayingSongId;

        public string CurrentlyPlayingSongId => _currentlyPlayingSongId;

        public bool IsPlaying => _player?.IsPlaying == true;

        public LibVlcMediaPlayer()
        {
            LibVLCSharp.Shared.Core.Initialize();

            _vlc = new LibVLC();
            _player = new MediaPlayer(_vlc);
        }

        public async Task InitializeAsync(string songId, Uri uri)
        {
            Task.Run(() => {
                IMetadataRetriever metadataRetriever = DependencyService.Get<IMetadataRetriever>();
                var song = metadataRetriever.PopulateMetadataAsync(uri);
                MessagingCenter.Send(MessengerKeys.App, MessengerKeys.MetadataLoaded, song);
            });

            _currentlyPlayingSongId = songId;

            using (var media = new LibVLCSharp.Shared.Media(_vlc, uri, ":no-video"))
            {
                _player.Media = media;
            }

            _player.TimeChanged += TimeChanged;
            _player.PositionChanged += PositionChanged;
            _player.LengthChanged += LengthChanged;
            _player.EndReached += EndReached;
            _player.Playing += Playing;
            _player.Paused += Paused;
        }

        public void Pause()
        {
            _player.Pause();
        }

        public void Play()
        {
            _player.Play();
        }

        public void DeafenVolume()
        {
            if (_player.IsPlaying)
            {
                _volume = _player.Volume;
                _player.Volume = 0;
            }
        }

        public void ResetVolume()
        {
            if (_player.IsPlaying)
            {
                _player.Volume = _volume;
            }
        }

        private void PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e) =>
            MessagingCenter.Send(MessengerKeys.App, MessengerKeys.Position, e.Position);

        private void Paused(object sender, System.EventArgs e) =>
            MessagingCenter.Send(MessengerKeys.App, MessengerKeys.Play, false);

        private void Playing(object sender, System.EventArgs e) =>
            MessagingCenter.Send(MessengerKeys.App, MessengerKeys.Play, true);

        private void EndReached(object sender, System.EventArgs e) =>
            MessagingCenter.Send(MessengerKeys.App, MessengerKeys.EndReached);

        private void LengthChanged(object sender, MediaPlayerLengthChangedEventArgs e) =>
            MessagingCenter.Send(MessengerKeys.App, MessengerKeys.Length, (long)TimeSpan.FromMilliseconds(e.Length).TotalSeconds);

        private void TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e) =>
            MessagingCenter.Send(MessengerKeys.App, MessengerKeys.Time, e.Time);
    }
}