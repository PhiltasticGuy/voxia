using System;
using System.Diagnostics;
using VoxIA.Core.Data;
using VoxIA.Mobile.Views;
using Xamarin.Forms;

namespace VoxIA.Mobile.ViewModels
{
    [QueryProperty(nameof(SongId), nameof(SongId))]
    public class SongDetailsViewModel : BaseViewModel
    {
        private ISongProvider SongProvider => DependencyService.Get<ISongProvider>();

        public Command PlayNow { get; }

        public string SongId
        {
            get => Id;
            set
            {
                Id = value;
                LoadSongById(value);
            }
        }

        private string _songTitle;
        private string _artistName;
        private string _albumCover;

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

        public SongDetailsViewModel()
        {
            Title = "Song Details";
            PlayNow = new Command(OnPlayNowClicked);
        }

        public async void OnPlayNowClicked()
        {
            // Remove the current page from the navigation stack because we
            // don't want the users to return to the details page when they
            // go back to the song library.
            Shell.Current.Navigation.RemovePage(Shell.Current.CurrentPage);

            // Navigate to the Currently Playing page.
            await Shell.Current.GoToAsync($"///{nameof(CurrentlyPlayingPage)}?{nameof(CurrentSongViewModel.SongId)}={SongId}");
        }

        public async void LoadSongById(string id)
        {
            try
            {
                var song = await SongProvider.GetSongByIdAsync(id);

                if (song != null)
                {
                    Id = song.Id;
                    SongTitle = song.Title;
                    ArtistName = song.ArtistName;
                    AlbumCover = song.AlbumCover;
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
