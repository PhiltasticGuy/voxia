using System;
using System.Threading.Tasks;
using VoxIA.Core.Media;
using VoxIA.Core.Streaming;

namespace VoxIA.Mobile.Services.Streaming
{
    public interface IStreamingService
    {
        Task<string> StartStreaming(string filename);
        Task StopStreaming();
    }
}
