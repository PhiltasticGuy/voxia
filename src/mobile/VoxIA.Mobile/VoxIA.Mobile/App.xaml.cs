using VoxIA.Core.Data;
using VoxIA.Core.Intents;
using VoxIA.Core.Media;
using VoxIA.Core.Transcription;
using VoxIA.Mobile.Services.Data;
using VoxIA.Mobile.Services.Media;
using VoxIA.Mobile.Services.Streaming;
using VoxIA.ZerocIce.Core.Client;
using Xamarin.Forms;

namespace VoxIA.Mobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            DependencyService.Register<IMediaPlayer, LibVlcMediaPlayer>();
            DependencyService.Register<ITranscriptionService, SpeechBrainService>();
            DependencyService.Register<IIntentClassificationService, RasaService>();
            DependencyService.Register<IStreamingService, IceStreamingService>();
            DependencyService.Register<ISongProvider, IceSongProvider>();

            var client = new GenericIceClient();
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
