using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxIA.Mobile.Models;
using VoxIA.Mobile.Services;
using VoxIA.Mobile.ViewModels;
using VoxIA.Mobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VoxIA.Mobile.Views
{
    public partial class SongsPage : ContentPage
    {
        private readonly SongsViewModel _viewModel;

        public SongsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new SongsViewModel();

            SongsListView.ItemsSource = DependencyService.Get<ISongProvider>().GetSongsByQueryAsync(searchBar.Text).Result;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        private async void OnTextChangedAsync(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                this.SongsListView.ItemsSource = await DependencyService.Get<ISongProvider>().GetAllSongsAsync();
            }
            else
            {
                this.SongsListView.ItemsSource = await DependencyService.Get<ISongProvider>().GetSongsByQueryAsync(e.NewTextValue);
            }
        }
    }
}