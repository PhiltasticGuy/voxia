using System;
using System.Diagnostics;
using System.Timers;
using System.Windows.Input;
using VoxIA.Mobile.Models;
using VoxIA.Mobile.Services;
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
        private string _url;
        private bool _isPlaying;

        private readonly Timer _timer = new Timer(1000);
        private int _length = 0;
        private int _position = 0;
        private float _progress = 0f;

        public string SongId
        {
            get
            {
                return _songId;
            }
            set
            {
                _songId = value;
                LoadSongById(value);
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

        public string Url
        {
            get => _url;
            set => SetProperty(ref _url, value);
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => SetProperty(ref _isPlaying, value);
        }

        public bool IsPaused => !IsPlaying;

        public string PositionText
        {
            get => TimeSpan.FromSeconds(_position).ToString(@"mm\:ss");
        }

        public int Position
        {
            get => _position;
            set
            {
                SetProperty(ref _position, value);
                OnPropertyChanged(nameof(PositionText));
            }
        }

        public float SongProgress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public int Length
        {
            get => _length;
            set
            {
                SetProperty(ref _length, value);
                OnPropertyChanged(nameof(SongLength));
            }
        }

        public string SongLength
        {
            get => TimeSpan.FromSeconds(_length).ToString(@"mm\:ss");
        }

        public CurrentSongViewModel()
        {
            Title = "Currently Playing";

            PlaySongCommand = new Command(TogglePlayStateAsync);
            PauseSongCommand = new Command(TogglePlayStateAsync);
            PreviousSongCommand = new Command(PlayPreviousSong);
            NextSongCommand = new Command(PlayNextSong);

            _timer.Elapsed += (sender, e) =>
            {
                Position += 1;

                if (Position == Length)
                {
                    _timer.Stop();

                    TogglePlayStateAsync();

                    Position = 0;
                    SongProgress = 0f;
                }
                else
                {
                    SongProgress = (float)Position / Length;
                }
            };
        }

        private async void TogglePlayStateAsync()
        {
            IsPlaying = !IsPlaying;

            // Don't forget to signal a change in 'IsPaused' too!
            OnPropertyChanged(nameof(IsPaused));

            if (IsPlaying)
            {
                _timer.Start();

                var song = new Song()
                {
                    Id = Id,
                    Url = Url
                };

                var x = DependencyService.Get<IMediaPlayer>();
                await x.PlayAsync(song);

                Id = song.Id;
                SongTitle = song.Title;
                ArtistName = song.ArtistName;
                AlbumCover = song.AlbumCover;
                Url = song.Url;
                Length = song.Length;
            }
            else
            {
                _timer.Stop();
                var x = DependencyService.Get<IMediaPlayer>();
                x.Pause();
            }
        }

        private void PlayPreviousSong()
        {
        }

        private void PlayNextSong()
        {
        }

        private async void LoadSongById(string id)
        {
            try
            {
                var song = await SongProvider.GetSongByIdAsync(id);

                if (song != null)
                {
                    Id = song.Id;
                    //SongTitle = song.Title;
                    //ArtistName = song.ArtistName;
                    //AlbumCover = song.AlbumCover;
                    Url = song.Url;
                    //Length = song.Length;

                    Application.Current.Properties["currentSongId"] = id;
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
