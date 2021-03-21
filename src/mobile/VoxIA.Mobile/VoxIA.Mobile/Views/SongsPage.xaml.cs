using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxIA.Mobile.Models;
using VoxIA.Mobile.ViewModels;
using VoxIA.Mobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VoxIA.Mobile.Views
{
    public partial class SongsPage : ContentPage
    {
        SongsViewModel _viewModel;

        public SongsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new SongsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}