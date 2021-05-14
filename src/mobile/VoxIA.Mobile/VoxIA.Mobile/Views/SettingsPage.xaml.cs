using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxIA.Core.Media;
using VoxIA.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VoxIA.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private readonly SettingsViewModel _viewModel;
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new SettingsViewModel();
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            bool shouldSave = true;

            var player = DependencyService.Get<IMediaPlayer>();
            if (player.IsPlaying)
            {
                shouldSave = await DisplayAlert(
                    "Stop song?", 
                    "Changing the settings will stop the current song. Are you sure you want to continue?",
                    "Yes", 
                    "No"
                );
            }

            if (shouldSave)
            {
                await _viewModel.SaveSettingsAsync();
            }
        }
    }
}