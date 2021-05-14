using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VoxIA.Core.Intents;
using VoxIA.Core.Media;
using VoxIA.Core.Transcription;
using VoxIA.Mobile.Services.Streaming;
using VoxIA.ZerocIce.Core.Client;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace VoxIA.Mobile.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public Command SaveSettingsCommand { get; }

        private string _iceIpAddress;
        private int _icePort;
        private string _asrIpAddress;
        private int _asrPort;
        private string _nluIpAddress;
        private int _nluPort;

        public string IceIpAddress
        {
            get => _iceIpAddress;
            set
            {
                SetProperty(ref _iceIpAddress, value);
            }
        }
        public int IcePort
        {
            get => _icePort;
            set
            {
                SetProperty(ref _icePort, value);
            }
        }

        public string AsrIpAddress
        {
            get => _asrIpAddress;
            set
            {
                SetProperty(ref _asrIpAddress, value);
            }
        }
        public int AsrPort
        {
            get => _asrPort;
            set
            {
                SetProperty(ref _asrPort, value);
            }
        }

        public string NluIpAddress
        {
            get => _nluIpAddress;
            set
            {
                SetProperty(ref _nluIpAddress, value);
            }
        }
        public int NluPort
        {
            get => _nluPort;
            set
            {
                SetProperty(ref _nluPort, value);
            }
        }

        public SettingsViewModel()
        {
            Title = "Settings";
            IceIpAddress = Preferences.Get(nameof(IceIpAddress), "127.0.0.1");
            IcePort = Preferences.Get(nameof(IcePort), 10000);
            AsrIpAddress = Preferences.Get(nameof(AsrIpAddress), "127.0.0.1");
            AsrPort = Preferences.Get(nameof(AsrPort), 5000);
            NluIpAddress = Preferences.Get(nameof(NluIpAddress), "127.0.0.1");
            NluPort = Preferences.Get(nameof(NluPort), 5005);
            SaveSettingsCommand = new Command(async () => await SaveSettingsAsync());
        }

        public async Task SaveSettingsAsync()
        {
            Preferences.Set(nameof(IceIpAddress), IceIpAddress);
            Preferences.Set(nameof(IcePort), IcePort);
            var streaming = DependencyService.Get<IStreamingService>();
            await streaming.StopStreaming();
            var iceClient = DependencyService.Get<GenericIceClient>();
            iceClient.Stop();
            iceClient.SetServerUrl(IceIpAddress, IcePort);
            iceClient.Start(new string[] { });

            Preferences.Set(nameof(AsrIpAddress), AsrIpAddress);
            Preferences.Set(nameof(AsrPort), AsrPort);
            var transcription = DependencyService.Get<ITranscriptionService>();
            transcription.SetServerUrl(AsrIpAddress, AsrPort);

            Preferences.Set(nameof(NluIpAddress), NluIpAddress);
            Preferences.Set(nameof(NluPort), NluPort);
            var intents = DependencyService.Get<IIntentClassificationService>();
            intents.SetServerUrl(NluIpAddress, NluPort);
        }
    }
}
