using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace VoxIA.Mobile.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://gitlab.com/uapv/voxia"));
        }

        public ICommand OpenWebCommand { get; }
    }
}