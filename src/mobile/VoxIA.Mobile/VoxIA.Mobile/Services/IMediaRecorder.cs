using System.Threading.Tasks;
using VoxIA.Mobile.Models;

namespace VoxIA.Mobile.Services
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