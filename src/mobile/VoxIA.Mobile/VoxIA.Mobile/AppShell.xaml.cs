using System;
using VoxIA.Mobile.Views;
using Xamarin.Forms;

namespace VoxIA.Mobile
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SongDetailsPage), typeof(SongDetailsPage));
            Routing.RegisterRoute(nameof(CurrentlyPlayingPage), typeof(CurrentlyPlayingPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
