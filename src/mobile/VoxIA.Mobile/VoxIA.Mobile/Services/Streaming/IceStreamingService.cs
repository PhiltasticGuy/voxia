using System;
using System.Threading.Tasks;
using VoxIA.Core.Media;
using VoxIA.Core.Streaming;
using VoxIA.ZerocIce.Core.Client;
using Xamarin.Forms;

namespace VoxIA.Mobile.Services.Streaming
{
    public class IceStreamingService : IStreamingService
    {
        private readonly GenericIceClient _client = DependencyService.Get<GenericIceClient>();
        private readonly Guid _clientId = Guid.NewGuid();

        public IceStreamingService()
        {

        }

        ~IceStreamingService()
        {
            if (_client != null)
            {
                _client.Stop();
            }
        }

        public Task RegisterClient(Client client)
        {
            throw new NotImplementedException();
        }

        public Task PlaySong(Song song)
        {
            return Task.CompletedTask;
        }

        public async Task<string> StartStreaming(string filename)
        {
            return await _client._mediaServer.PlaySongAsync(_clientId.ToString(), filename);
        }

        public async Task StopStreaming()
        {
            await _client._mediaServer.StopSongAsync(_clientId.ToString());
        }
    }
}
