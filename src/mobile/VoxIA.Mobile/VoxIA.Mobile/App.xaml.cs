using System;
using VoxIA.Mobile.Services;
using VoxIA.Mobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VoxIA.Mobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            DependencyService.Register<MockSongDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
