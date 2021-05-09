using System;
using System.Threading.Tasks;
using VoxIA.Core.Media;
using VoxIA.Core.Streaming;
using VoxIA.ZerocIce.Core.Client;

namespace VoxIA.Mobile.Services.Streaming
{
    public class IceStreamingService : IStreamingService
    {
        private readonly IIceClient _client;

        public IceStreamingService()
        {

        }

        public Task RegisterClient(Client client)
        {
            throw new NotImplementedException();
        }

        public Task PlaySong(Song song)
        {
            return Task.CompletedTask;
        }

        public Task StartStreaming(Song song)
        {
            throw new NotImplementedException();
        }

        public Task StopStreaming()
        {
            throw new NotImplementedException();
        }
    }
}
