using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using VoxIA.Mobile.Models;
using VoxIA.Mobile.Services;
using VoxIA.Mobile.Views;
using Xamarin.Forms;

namespace VoxIA.Mobile.ViewModels
{
    public class SongsViewModel : BaseViewModel
    {
        private ISongProvider SongProvider => DependencyService.Get<ISongProvider>();

        private Song _selectedSong;

        public ObservableCollection<Song> Songs { get; }
        public Command LoadSongsCommand { get; }
        public Command AddSongCommand { get; }
        public Command<Song> SongTapped { get; }
        public Command PerformSearch { get; }

        public SongsViewModel()
        {
            Title = "Song Library";
            Songs = new ObservableCollection<Song>();

            LoadSongsCommand = new Command(async () => await OnLoadSongs());
            SongTapped = new Command<Song>(OnSongSelected);
            AddSongCommand = new Command(OnAddSong);
            PerformSearch = new Command<string>(async (string query) => await OnPerformSearch(query));
        }

        async Task OnPerformSearch(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                await OnLoadSongs();
                return;
            }

            IsBusy = true;

            try
            {
                Songs.Clear();
                var songs = await SongProvider.GetSongsByQueryAsync(query);
                foreach (var song in songs)
                {
                    Songs.Add(song);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task OnLoadSongs()
        {
            IsBusy = true;

            try
            {
                Songs.Clear();
                var songs = await SongProvider.GetAllSongsAsync();
                foreach (var song in songs)
                {
                    Songs.Add(song);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedSong = null;
        }

        public Song SelectedSong
        {
            get => _selectedSong;
            set
            {
                SetProperty(ref _selectedSong, value);
                OnSongSelected(value);
            }
        }

        private async void OnAddSong(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        async void OnSongSelected(Song song)
        {
            if (song == null)
                return;

            // This will push the SongDetailsPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(SongDetailsPage)}?{nameof(SongDetailsViewModel.SongId)}={song.Id}");
        }
    }
}