using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VoxIA.Mobile.Services.Api
{
    public interface ITranscriptionService
    {
        Task<string> TranscribeRecording(string filename, byte[] content);
    }
}
