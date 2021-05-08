using Plugin.AudioRecorder;
using System;
using System.Threading.Tasks;
using VoxIA.Mobile.Services.Media;

namespace VoxIA.Mobile.Droid
{
    public class AndroidMediaRecorder : IMediaRecorder
    {
        private AudioRecorderService _recorder;

        public bool IsRecording => _recorder?.IsRecording == true;

        public string RecordingFilePath => _recorder?.GetAudioFilePath();

        public AndroidMediaRecorder()
        {
            _recorder = new AudioRecorderService
            {
                StopRecordingOnSilence = true,
                StopRecordingAfterTimeout = true,
                TotalAudioTimeout = TimeSpan.FromSeconds(15),
                AudioSilenceTimeout = TimeSpan.FromSeconds(2)
            };
        }

        public async Task Record()
        {
            _recorder.StopRecordingOnSilence = true;
            var audioRecordTask = await _recorder.StartRecording();

        }

        public async Task Stop()
        {
            var filePath = _recorder.GetAudioFilePath();

            if (filePath != null)
            {
                await _recorder.StopRecording();
            }
        }

        public void PlayLastRecording()
        {
            var filePath = _recorder.GetAudioFilePath();

            if (filePath != null)
            {
                AudioPlayer player = new AudioPlayer();
                player.Play(filePath);
            }
        }
    }
}