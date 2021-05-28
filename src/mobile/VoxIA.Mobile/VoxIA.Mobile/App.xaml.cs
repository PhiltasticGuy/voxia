using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using VoxIA.Core.Data;
using VoxIA.Core.Intents;
using VoxIA.Core.Media;
using VoxIA.Core.Transcription;
using VoxIA.Mobile.Services.Data;
using VoxIA.Mobile.Services.Media;
using VoxIA.Mobile.Services.Streaming;
using VoxIA.Mobile.ViewModels;
using VoxIA.ZerocIce.Core.Client;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace VoxIA.Mobile
{
    public partial class App : Application
    {
        public App(Dictionary<string, string> iceProperties)
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            DependencyService.Register<IMediaPlayer, LibVlcMediaPlayer>();
            DependencyService.RegisterSingleton<ITranscriptionService>(
                new SpeechBrainService(
                    Preferences.Get(nameof(SettingsViewModel.AsrIpAddress), "192.168.0.11"),
                    Preferences.Get(nameof(SettingsViewModel.AsrPort), 5000)
                )
            );
            DependencyService.RegisterSingleton<IIntentClassificationService>(
                new RasaService(
                    Preferences.Get(nameof(SettingsViewModel.NluIpAddress), "192.168.0.11"),
                    Preferences.Get(nameof(SettingsViewModel.NluPort), 5005)
                )
            );
            DependencyService.Register<IStreamingService, IceStreamingService>();
            DependencyService.Register<ISongProvider, IceSongProvider>();

            var client = new GenericIceClient(iceProperties);
            client.Start(new string[] { });
            DependencyService.RegisterSingleton<GenericIceClient>(client);

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
