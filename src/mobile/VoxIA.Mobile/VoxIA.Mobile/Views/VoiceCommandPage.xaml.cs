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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VoiceCommandPage : ContentPage
    {
        private readonly VoiceCommandViewModel _viewModel;

        public VoiceCommandPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new VoiceCommandViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        private void bntRecord_Clicked(object sender, EventArgs e) => _viewModel.OnRecordClickedAsync();

        private void bntStop_Clicked(object sender, EventArgs e) => _viewModel.OnStopClicked();

        private void bntPlay_Clicked(object sender, EventArgs e) => _viewModel.OnPlayClicked();

        private void bntExecute_Clicked(object sender, EventArgs e) => _viewModel.OnExecuteClicked();
    }
}