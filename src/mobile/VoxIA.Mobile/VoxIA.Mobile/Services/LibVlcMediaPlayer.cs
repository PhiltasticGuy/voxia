using LibVLCSharp.Shared;
using System;
using System.Threading.Tasks;
using VoxIA.Mobile.Models;
using Xamarin.Forms;

namespace VoxIA.Mobile.Services
{
    public class LibVlcMediaPlayer : IMediaPlayer
    {
        private readonly MediaPlayer _player;
        private readonly LibVLC _vlc;

        public LibVlcMediaPlayer()
        {
            Core.Initialize();

            _vlc = new LibVLC();
            _player = new MediaPlayer(_vlc);
        }

        public async Task InitializeAsync(Song song)
        {
            IMetadataRetriever metadataRetriever = DependencyService.Get<IMetadataRetriever>();
            await metadataRetriever.PopulateMetadataAsync(song);

            using (var media = new Media(_vlc, new Uri(song.Url), ":no-video"))
                _player.Media = media;

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