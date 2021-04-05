using VoxIA.Mobile.Services;
using VoxIA.Mobile.ViewModels;
using Xamarin.Forms;

namespace VoxIA.Mobile.Views
{
    public partial class SongsPage : ContentPage
    {
        //private ISongProvider SongProvider => DependencyService.Get<ISongProvider>();
        private readonly SongsViewModel _viewModel;

        public SongsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new SongsViewModel();

            //SongsListView.ItemsSource = SongProvider.GetSongsByQueryAsync(searchBar.Text).Result;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();

            // Clear the SearchBar.
            this.searchBar.Text = string.Empty;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                //SongsListView.ItemsSource = await SongProvider.GetAllSongsAsync();
                //await _viewModel.OnLoadSongs();
                if (_viewModel.LoadSongsCommand.CanExecute(null))
                {
                    _viewModel.LoadSongsCommand.Execute(null);
                }
            }
            else
            {
                //SongsListView.ItemsSource = await SongProvider.GetSongsByQueryAsync(e.NewTextValue);
                //await _viewModel.OnPerformSearch(e.NewTextValue);
                if (_viewModel.LoadSongsCommand.CanExecute(e.NewTextValue))
                {
                    _viewModel.PerformSearch.Execute(e.NewTextValue);
                }
            }
        }

        private async void OnTextChangedAsync(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                //SongsListView.ItemsSource = await SongProvider.GetAllSongsAsync();
                await _viewModel.OnLoadSongs();
                //if (_viewModel.LoadSongsCommand.CanExecute(null))
                //{
                //    _viewModel.LoadSongsCommand.Execute(null);
                //}
            }
            else
            {
                //SongsListView.ItemsSource = await SongProvider.GetSongsByQueryAsync(e.NewTextValue);
                await _viewModel.OnPerformSearch(e.NewTextValue);
                //if (_viewModel.LoadSongsCommand.CanExecute(e.NewTextValue))
                //{
                //    _viewModel.PerformSearch.Execute(e.NewTextValue);
                //}
            }
        }
    }
}