using System.Threading.Tasks;

namespace VoxIA.Core.Media
{
    public interface IMediaRecorder
    {
        bool IsRecording { get; }
        string RecordingFilePath { get; }

        Task Record();
        Task Stop();
        void PlayLastRecording();
    }
}