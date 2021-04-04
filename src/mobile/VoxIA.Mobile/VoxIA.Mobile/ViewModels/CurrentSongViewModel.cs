using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
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

        public CurrentSongViewModel()
        {
            Title = "Currently Playing";

            PlaySongCommand = new Command(TogglePlayState);
            PauseSongCommand = new Command(TogglePlayState);
            PreviousSongCommand = new Command(PlayPreviousSong);
            NextSongCommand = new Command(PlayNextSong);
        }

        private void TogglePlayState()
        {
            IsPlaying = !IsPlaying;

            // Don't forget to signal a change in 'IsPaused' too!
            OnPropertyChanged(nameof(IsPaused));
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
