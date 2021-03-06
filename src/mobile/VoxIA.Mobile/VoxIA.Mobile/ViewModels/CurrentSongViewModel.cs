using System;
using System.Diagnostics;
using System.Windows.Input;
using VoxIA.Core.Data;
using VoxIA.Core.Media;
using VoxIA.Mobile.Services;
using VoxIA.Mobile.Services.Streaming;
using Xamarin.Forms;

namespace VoxIA.Mobile.ViewModels
{
    [QueryProperty(nameof(SongId), nameof(SongId))]
    public class CurrentSongViewModel : BaseViewModel
    {
        private ISongProvider SongProvider => DependencyService.Get<ISongProvider>();

        public ICommand PlaySongCommand { get; }
        public ICommand PauseSongCommand { get; }
        public ICommand PreviousSongCommand { get; }
        public ICommand NextSongCommand { get; }

        private string _songId;
        private string _songTitle;
        private string _artistName;
        private string _albumCover;
        private bool _isPlaying;
        //private int _length = 0;
        //private int _position = 0;
        //private float _progress = 0f;

        public string SongId
        {
            get
            {
                return _songId;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _songId = value;
                    LoadSongById(value);
                }
            }
        }

        public string Id { get; set; }

        public string SongTitle
        {
            get => _songTitle;
            set => SetProperty(ref _songTitle, value);
        }

        public string ArtistName
        {
            get => _artistName;
            set => SetProperty(ref _artistName, value);
        }

        public string AlbumCover
        {
            get => _albumCover;
            set => SetProperty(ref _albumCover, value);
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                SetProperty(ref _isPlaying, value);

                // Don't forget to signal a change in 'IsPaused' too!
                OnPropertyChanged(nameof(IsPaused));
            }
        }

        public bool IsPaused => !IsPlaying;

        //public string PositionText
        //{
        //    get => TimeSpan.FromSeconds(_position).ToString(@"mm\:ss");
        //}

        //public int Position
        //{
        //    get => _position;
        //    set
        //    {
        //        SetProperty(ref _position, value);
        //        OnPropertyChanged(nameof(PositionText));
        //    }
        //}

        //public float SongProgress
        //{
        //    get => _progress;
        //    set => SetProperty(ref _progress, value);
        //}

        //public int Length
        //{
        //    get => _length;
        //    set
        //    {
        //        SetProperty(ref _length, value);
        //        OnPropertyChanged(nameof(SongLength));
        //    }
        //}

        //public string SongLength
        //{
        //    get => TimeSpan.FromSeconds(_length).ToString(@"mm\:ss");
        //}

        public CurrentSongViewModel()
        {
            Title = "Currently Playing";

            PlaySongCommand = new Command(TogglePlayState);
            PauseSongCommand = new Command(TogglePlayState);
            PreviousSongCommand = new Command(PlayPreviousSong);
            NextSongCommand = new Command(PlayNextSong);
        }

        public void OnAppearing()
        {
            //MessagingCenter.Subscribe<string, long>(MessengerKeys.App, MessengerKeys.Time, (app, time) =>
            //{
            //    Position = (int)TimeSpan.FromMilliseconds(time).TotalSeconds;
            //});
            //MessagingCenter.Subscribe<string, float>(MessengerKeys.App, MessengerKeys.Position, (app, position) => SongProgress = position);
            //MessagingCenter.Subscribe<string, long>(MessengerKeys.App, MessengerKeys.Length, (app, length) => Length = (int)length);
            MessagingCenter.Subscribe<string>(MessengerKeys.App, MessengerKeys.EndReached, app => EndReached());
            MessagingCenter.Subscribe<Song>(MessengerKeys.App, MessengerKeys.MetadataLoaded, metadata => {
                SongTitle = metadata.Title;
                ArtistName = metadata.ArtistName;
            });
        }

        private void TogglePlayState()
        {
            IsPlaying = !IsPlaying;

            var x = DependencyService.Get<IMediaPlayer>();
            if (IsPlaying)
            {
                x.Play();
            }
            else
            {
                x.Pause();
            }
        }

        private async void PlayPreviousSong()
        {
            try
            {
                var songs = await SongProvider.GetAllSongsAsync();

                int i = 0;
                Song current;
                do
                {
                    current = songs[i++];
                }
                while (current.Id != Id);

                Song prev = songs[(--i == 0 ? songs.Count - 1 : i - 1)];

                var streaming = DependencyService.Get<IStreamingService>();
                await streaming.StopStreaming();
                var url = await streaming.StartStreaming(prev.Id);

                if (!string.IsNullOrEmpty(url))
                {
                    Console.WriteLine($"[INFO] Playing the song '{prev.Id}' from '{url}'.");

                    var x = DependencyService.Get<IMediaPlayer>();
                    await x.InitializeAsync(prev.Id, new Uri(url));

                    Id = prev.Id;
                    SongTitle = prev.Title;
                    ArtistName = prev.ArtistName;
                    AlbumCover = prev.AlbumCover;
                    IsPlaying = true;

                    x.Play();
                }
                else
                {
                    //TODO: Error handling! Dispay a nice message...
                    Console.WriteLine($"[ERROR] Could not play the song '{prev.Id}'. Please try again.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to Load Item!");
                Debug.WriteLine(ex);
            }
            //try
            //{
            //    var songs = await SongProvider.GetAllSongsAsync();

            //    int i = 0;
            //    Song current;
            //    do
            //    {
            //        current = songs[i++];
            //    }
            //    while (current.Id != Id);

            //    Song next = songs[(--i == 0 ? songs.Count - 1 : i - 1)];

            //    var x = DependencyService.Get<IMediaPlayer>();
            //    await x.InitializeAsync(next.Id);

            //    Id = next.Id;
            //    SongTitle = next.Title;
            //    ArtistName = next.ArtistName;
            //    AlbumCover = next.AlbumCover;
            //    Url = next.Filename;
            //    IsPlaying = true;

            //    x.Play();
            //}
            //catch (Exception)
            //{
            //    Debug.WriteLine("Failed to Load Item");
            //}
        }

        private async void PlayNextSong()
        {
            try
            {
                var songs = await SongProvider.GetAllSongsAsync();

                int i = 0;
                Song current;
                do
                {
                    current = songs[i++];
                }
                while (current.Id != Id);

                Song next = songs[(i) % songs.Count];

                var streaming = DependencyService.Get<IStreamingService>();
                await streaming.StopStreaming();
                var url = await streaming.StartStreaming(next.Id);

                if (!string.IsNullOrEmpty(url))
                {
                    Console.WriteLine($"[INFO] Playing the song '{next.Id}' from '{url}'.");

                    var x = DependencyService.Get<IMediaPlayer>();
                    await x.InitializeAsync(next.Id, new Uri(url));

                    Id = next.Id;
                    SongTitle = next.Title;
                    ArtistName = next.ArtistName;
                    AlbumCover = next.AlbumCover;
                    IsPlaying = true;

                    x.Play();
                }
                else
                {
                    //TODO: Error handling! Dispay a nice message...
                    Console.WriteLine($"[ERROR] Could not play the song '{next.Id}'. Please try again.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to Load Item!");
                Debug.WriteLine(ex);
            }
            //try
            //{
            //    var songs = await SongProvider.GetAllSongsAsync();

            //    int i = 0;
            //    Song current;
            //    do
            //    {
            //        current = songs[i++];
            //    }
            //    while (current.Id != Id);

            //    Song next = songs[(i) % songs.Count];

            //    var x = DependencyService.Get<IMediaPlayer>();
            //    await x.InitializeAsync(next);

            //    Id = next.Id;
            //    SongTitle = next.Title;
            //    ArtistName = next.ArtistName;
            //    AlbumCover = next.AlbumCover;
            //    Url = next.Filename;
            //    IsPlaying = true;

            //    x.Play();
            //}
            //catch (Exception)
            //{
            //    Debug.WriteLine("Failed to Load Item");
            //}
        }

        private void EndReached()
        {
            //SongProgress = 0;
            //Position = 0;
            IsPlaying = false;
        }

        private async void LoadSongById(string id)
        {
            try
            {
                var song = await SongProvider.GetSongByIdAsync(id);

                if (song != null)
                {
                    Id = song.Id;
                }

                var streaming = DependencyService.Get<IStreamingService>();
                await streaming.StopStreaming();
                var url = await streaming.StartStreaming(song.Id);

                if (!string.IsNullOrEmpty(url))
                {
                    Console.WriteLine($"[INFO] Playing the song '{song.Id}' from '{url}'.");

                    var x = DependencyService.Get<IMediaPlayer>();
                    await x.InitializeAsync(song.Id, new Uri(url));

                    SongTitle = song.Title;
                    ArtistName = song.ArtistName;
                    AlbumCover = song.AlbumCover;
                    IsPlaying = true;

                    x.Play();
                }
                else
                {
                    //TODO: Error handling! Dispay a nice message...
                    Console.WriteLine($"[ERROR] Could not play the song '{song.Id}'. Please try again.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to Load Item!");
                Debug.WriteLine(ex);
            }
        }
    }
}
