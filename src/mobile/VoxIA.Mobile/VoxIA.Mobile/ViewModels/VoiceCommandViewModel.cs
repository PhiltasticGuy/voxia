using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VoxIA.Core.Data;
using VoxIA.Core.Intents;
using VoxIA.Core.Media;
using VoxIA.Core.Transcription;
using VoxIA.Mobile.Services.Streaming;
using VoxIA.Mobile.Views;
using Xamarin.Forms;

namespace VoxIA.Mobile.ViewModels
{
    class VoiceCommandViewModel : BaseViewModel
    {
        private ISongProvider SongProvider => DependencyService.Get<ISongProvider>();
        private IMediaRecorder MediaRecorder => DependencyService.Get<IMediaRecorder>();
        private ITranscriptionService TranscriptionService => DependencyService.Get<ITranscriptionService>();
        private IIntentClassificationService IntentClassificationService => DependencyService.Get<IIntentClassificationService>();
        private IStreamingService StreamingService => DependencyService.Get<IStreamingService>();

        private bool _isTimerRunning = false;
        private int _seconds = 0;
        private int _minutes = 0;

        private string _secondsDisplay = "";
        private string _minutesDisplay = "";

        private bool _isRecordEnabled = false;
        private bool _isStopEnabled = false;
        private bool _isPlayEnabled = false;
        private bool _isExecuteEnabled = false;

        private string _transcript = "";
        private string _intent = "";
        private string _entity = "";

        public string SecondsDisplay
        {
            get => _secondsDisplay;
            set => SetProperty(ref _secondsDisplay, value);
        }
        public string MinutesDisplay
        {
            get => _minutesDisplay;
            set => SetProperty(ref _minutesDisplay, value);
        }

        public bool IsRecordEnabled
        {
            get => _isRecordEnabled;
            set => SetProperty(ref _isRecordEnabled, value);
        }
        public bool IsStopEnabled
        {
            get => _isStopEnabled;
            set => SetProperty(ref _isStopEnabled, value);
        }
        public bool IsPlayEnabled
        {
            get => _isPlayEnabled;
            set => SetProperty(ref _isPlayEnabled, value);
        }
        public bool IsExecuteEnabled
        {
            get => _isExecuteEnabled;
            set => SetProperty(ref _isExecuteEnabled, value);
        }
        public string Transcript
        {
            get => _transcript;
            set => SetProperty(ref _transcript, value);
        }
        public string Intent
        {
            get => _intent;
            set => SetProperty(ref _intent, value);
        }
        public string Entity
        {
            get => _entity;
            set => SetProperty(ref _entity, value);
        }

        public VoiceCommandViewModel()
        {
            Title = "Voice Command";

            _seconds = 0;
            _minutes = 0;
            _secondsDisplay = "00";
            _minutesDisplay = "00";

            _isRecordEnabled = true;
            _isStopEnabled = false;
            _isPlayEnabled = false;
            _isExecuteEnabled = false;
        }

        public void OnAppearing()
        {
        }

        public async Task OnRecordClickedAsync()
        {

            if (!MediaRecorder.IsRecording)
            {
                _seconds = 0;
                _minutes = 0;
                _isTimerRunning = true;

                Device.StartTimer(TimeSpan.FromSeconds(1), () => {
                    if(_isTimerRunning) {
                        _seconds++;

                        if (_seconds.ToString().Length == 1)
                        {
                            SecondsDisplay = "0" + _seconds.ToString();
                        }
                        else
                        {
                            SecondsDisplay = _seconds.ToString();
                        }
                        if (_seconds == 60)
                        {
                            _minutes++;
                            _seconds = 0;

                            if (_minutes.ToString().Length == 1)
                            {
                                MinutesDisplay = "0" + _minutes.ToString();
                            }
                            else
                            {
                                MinutesDisplay = _minutes.ToString();
                            }

                            SecondsDisplay = "00";
                        }
                    }

                    return _isTimerRunning;
                });

                IsRecordEnabled = false;
                IsPlayEnabled = false;
                IsStopEnabled = true;
                IsExecuteEnabled = false;

                // Ensure that the audio recording is audible.
                var player = DependencyService.Get<IMediaPlayer>();
                player.DeafenVolume();

                await MediaRecorder.Record();
            }
        }

        public void OnStopClicked()
        {
            MediaRecorder.Stop();

            // Reset the audio volume.
            var player = DependencyService.Get<IMediaPlayer>();
            player.ResetVolume();

            _isTimerRunning = false;

            IsRecordEnabled = true;
            IsPlayEnabled = true;
            IsStopEnabled = false;
            IsExecuteEnabled = true;
            SecondsDisplay = "00";
            MinutesDisplay = "00";
        }

        public void OnPlayClicked()
        {
            MediaRecorder.PlayLastRecording();
        }

        public async Task OnExecuteClickedAsync()
        {
            _isTimerRunning = false;

            IsRecordEnabled = true;
            IsPlayEnabled = false;
            IsStopEnabled = false;
            IsExecuteEnabled = false;
            SecondsDisplay = "00";
            MinutesDisplay = "00";

            Transcript = 
                await TranscriptionService.TranscribeRecording(
                    Path.GetFileName(MediaRecorder.RecordingFilePath), 
                    File.ReadAllBytes(MediaRecorder.RecordingFilePath)
                );

            var intent = await IntentClassificationService.ParseIntent(Transcript);
            Intent = $"{{{intent.intent.name} : {intent.intent.confidence}}}";

            if (intent.intent.name == "play_song")
            {
                if (intent.entities.Count > 0)
                {
                    var entity = intent.entities.Aggregate((first, second) => 
                        first.confidence_entity > second.confidence_entity ? first : second
                    );
                    Entity = $"{{{entity.entity} : {entity.confidence_entity}}}";

                    var url = await StreamingService.StartStreaming(entity.entity);

                    if (!string.IsNullOrEmpty(url))
                    {
                        //var player = DependencyService.Get<IMediaPlayer>();
                        //await player.InitializeAsync(entity.entity, new Uri(url));
                        //player.Play();

                        // Navigate to the Currently Playing page.
                        await Shell.Current.GoToAsync($"///{nameof(CurrentlyPlayingPage)}?{nameof(CurrentSongViewModel.SongId)}={entity.entity}");
                    }
                    else
                    {
                        //TODO: Error handling! Dispay a nice message...
                        Console.WriteLine($"[ERROR] Could not play the song '{entity.entity}'. Please try again.");
                    }
                }
                else
                {
                    Entity = "";
                }
            }
            else if (intent.intent.name == "pause_song")
            {
                var player = DependencyService.Get<IMediaPlayer>();
                player.Pause();

                Entity = "";
            }
            else if (intent.intent.name == "stop_song")
            {
                await StreamingService.StopStreaming();

                Entity = "";
            }
            else if (intent.intent.name == "prev_song")
            {
                var player = DependencyService.Get<IMediaPlayer>();

                var songs = await SongProvider.GetAllSongsAsync();

                int i = 0;
                Song current;
                do
                {
                    current = songs[i++];
                }
                while (current.Id != player.CurrentlyPlayingSongId);

                Song prev = songs[(--i == 0 ? songs.Count - 1 : i - 1)];
                
                Entity = "";

                // Navigate to the Currently Playing page.
                await Shell.Current.GoToAsync($"///{nameof(CurrentlyPlayingPage)}?{nameof(CurrentSongViewModel.SongId)}={prev.Id}");
            }
            else if (intent.intent.name == "next_song")
            {
                var player = DependencyService.Get<IMediaPlayer>();

                var songs = await SongProvider.GetAllSongsAsync();

                int i = 0;
                Song current;
                do
                {
                    current = songs[i++];
                }
                while (current.Id != player.CurrentlyPlayingSongId);

                Song next = songs[(i) % songs.Count];

                Entity = "";

                // Navigate to the Currently Playing page.
                await Shell.Current.GoToAsync($"///{nameof(CurrentlyPlayingPage)}?{nameof(CurrentSongViewModel.SongId)}={next.Id}");
            }
        }
    }
}
