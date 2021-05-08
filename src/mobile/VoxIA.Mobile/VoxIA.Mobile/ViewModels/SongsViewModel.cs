using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using VoxIA.Mobile.Models;
using VoxIA.Mobile.Services;
using VoxIA.Mobile.Services.Data;
using VoxIA.Mobile.Views;
using Xamarin.Forms;

namespace VoxIA.Mobile.ViewModels
{
    public class SongsViewModel : BaseViewModel
    {
        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        private ISongProvider SongProvider => DependencyService.Get<ISongProvider>();

        public Command LoadSongs { get; }
        public Command<Song> SongTapped { get; }
        public Command PerformSearch { get; }

        public ObservableCollection<Song> Songs { get; }

        private Song _selectedSong;

        public Song SelectedSong
        {
            get => _selectedSong;
            set
            {
                SetProperty(ref _selectedSong, value);
                OnSongSelected(value);
            }
        }

        public SongsViewModel()
        {
            Title = "Song Library";
            Songs = new ObservableCollection<Song>();

            LoadSongs = new Command(async () => await OnLoadSongs());
            SongTapped = new Command<Song>(OnSongSelected);
            PerformSearch = new Command<string>(async (string query) => await OnPerformSearch(query));
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedSong = null;
        }

        public async Task OnPerformSearch(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                await OnLoadSongs();
                return;
            }

            IsBusy = true;

            try
            {
                await _semaphoreSlim.WaitAsync();
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
                _semaphoreSlim.Release();
                IsBusy = false;
            }
        }

        public async Task OnLoadSongs()
        {
            IsBusy = true;

            try
            {
                await _semaphoreSlim.WaitAsync();
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
                _semaphoreSlim.Release();
                IsBusy = false;
            }
        }

        private async void OnSongSelected(Song song)
        {
            if (song == null)
                return;

            // This will push the SongDetailsPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(SongDetailsPage)}?{nameof(SongDetailsViewModel.SongId)}={song.Id}");
        }
    }
}