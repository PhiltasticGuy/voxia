using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxIA.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VoxIA.Mobile.Views
{
    public partial class CurrentlyPlayingPage : ContentPage
    {
        CurrentSongViewModel _viewModel;

        public CurrentlyPlayingPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new CurrentSongViewModel();
        }

        protected override void OnAppearing()
        {
            if (Application.Current.Properties.ContainsKey("currentSongId"))
            {
                var currentSongId = Application.Current.Properties["currentSongId"].ToString();

                if (_viewModel.SongId != currentSongId)
                {
                    _viewModel.SongId = currentSongId;
                }
            }
        }
    }
}