using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using VoxIA.Mobile.Models;
using VoxIA.Mobile.Services;
using Xamarin.Forms;

namespace VoxIA.Mobile.ViewModels
{
    // TODO: Should be a difference between CurrentlyPlaying and SongDetails! Split this up...

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

        private readonly Timer _timer = new Timer(1000);
        private readonly int _length = 10;
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

        public string SongLength
        {
            get => TimeSpan.FromSeconds(_length).ToString(@"mm\:ss");
        }

        public CurrentSongViewModel()
        {
            Title = "Currently Playing";

            PlaySongCommand = new Command(TogglePlayState);
            PauseSongCommand = new Command(TogglePlayState);
            PreviousSongCommand = new Command(PlayPreviousSong);
            NextSongCommand = new Command(PlayNextSong);

            _timer.Elapsed += (sender, e) =>
            {
                Position += 1;

                if (Position == _length)
                {
                    _timer.Stop();

                    TogglePlayState();

                    Position = 0;
                    SongProgress = 0f;
                }
                else
                {
                    SongProgress = (float)Position / _length;
                }
            };
        }

        private void TogglePlayState()
        {
            IsPlaying = !IsPlaying;

            // Don't forget to signal a change in 'IsPaused' too!
            OnPropertyChanged(nameof(IsPaused));

            if (IsPlaying)
            {
                _timer.Start();
            }
            else
            {
                _timer.Stop();
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
                    SongTitle = song.Title;
                    ArtistName = song.ArtistName;

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
