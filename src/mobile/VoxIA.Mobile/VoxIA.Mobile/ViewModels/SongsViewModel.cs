using System;
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
        public new IDataStore<Song> DataStore => DependencyService.Get<IDataStore<Song>>();
        private Song _selectedSong;

        public ObservableCollection<Song> Songs { get; }
        public Command LoadSongsCommand { get; }
        public Command AddSongCommand { get; }
        public Command<Song> SongTapped { get; }

        public SongsViewModel()
        {
            Title = "Browse Songs";
            Songs = new ObservableCollection<Song>();
            LoadSongsCommand = new Command(async () => await ExecuteLoadSongsCommand());

            SongTapped = new Command<Song>(OnSongSelected);

            AddSongCommand = new Command(OnAddSong);
        }

        async Task ExecuteLoadSongsCommand()
        {
            IsBusy = true;

            try
            {
                Songs.Clear();
                var songs = await DataStore.GetItemsAsync(true);
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

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={song.Id}");
        }
    }
}