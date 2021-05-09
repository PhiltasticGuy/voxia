using System.Threading.Tasks;

namespace VoxIA.Core.Transcription
{
    public interface ITranscriptionService
    {
        Task<string> TranscribeRecording(string filename, byte[] content);
    }
}
