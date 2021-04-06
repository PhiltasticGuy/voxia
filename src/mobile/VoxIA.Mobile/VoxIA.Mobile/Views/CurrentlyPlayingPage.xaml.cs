using VoxIA.Mobile.ViewModels;
using Xamarin.Forms;

namespace VoxIA.Mobile.Views
{
    public partial class CurrentlyPlayingPage : ContentPage
    {
        private readonly CurrentSongViewModel _viewModel;

        public CurrentlyPlayingPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new CurrentSongViewModel();
        }

        protected override void OnAppearing()
        {
            _viewModel.OnAppearing();
        }
    }
}