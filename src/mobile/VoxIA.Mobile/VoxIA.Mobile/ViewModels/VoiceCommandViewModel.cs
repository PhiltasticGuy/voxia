using System;
using System.IO;
using System.Threading.Tasks;
using VoxIA.Mobile.Services.Api;
using VoxIA.Mobile.Services.Media;
using Xamarin.Forms;

namespace VoxIA.Mobile.ViewModels
{
    class VoiceCommandViewModel : BaseViewModel
    {
        private IMediaRecorder MediaRecorder => DependencyService.Get<IMediaRecorder>();
        private ITranscriptionService TranscriptionService => DependencyService.Get<ITranscriptionService>();

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
            //MediaRecorder.Record();
            //Thread.Sleep(5000);
            //MediaRecorder.Stop();
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

                //recorder.StopRecordingOnSilence = IsSilence.IsToggled;
                //var audioRecordTask = await recorder.StartRecording();

                IsRecordEnabled = false;
                //bntRecord.BackgroundColor = Color.Silver;
                IsPlayEnabled = false;
                //bntPlay.BackgroundColor = Color.Silver;
                IsStopEnabled = true;
                //bntStop.BackgroundColor = Color.FromHex("#7cbb45");
                IsExecuteEnabled = false;
                //bntPlay.BackgroundColor = Color.Silver;

                //await audioRecordTask;

                await MediaRecorder.Record();
            }
        }

        public void OnStopClicked()
        {
            MediaRecorder.Stop();

            _isTimerRunning = false;
            IsRecordEnabled = true;
            //bntRecord.BackgroundColor = Color.FromHex("#7cbb45");
            IsPlayEnabled = true;
            //bntPlay.BackgroundColor = Color.FromHex("#7cbb45");
            IsStopEnabled = false;
            //bntStop.BackgroundColor = Color.Silver;
            IsExecuteEnabled = true;
            //bntPlay.BackgroundColor = Color.FromHex("#7cbb45");
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
            //bntRecord.BackgroundColor = Color.FromHex("#7cbb45");
            IsPlayEnabled = false;
            //bntPlay.BackgroundColor = Color.FromHex("#7cbb45");
            IsStopEnabled = false;
            //bntStop.BackgroundColor = Color.Silver;
            IsExecuteEnabled = false;
            //bntStop.BackgroundColor = Color.Silver;
            SecondsDisplay = "00";
            MinutesDisplay = "00";

            Transcript = 
                await TranscriptionService.TranscribeRecording(
                    Path.GetFileName(MediaRecorder.RecordingFilePath), 
                    File.ReadAllBytes(MediaRecorder.RecordingFilePath)
                );
        }
    }
}
