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

        public Task PlaySong(Song song)
        {
            return Task.CompletedTask;
        }

        public async Task<string> StartStreaming(string filename)
        {
            return await _client.PlaySongAsync(filename);
        }

        public async Task StopStreaming()
        {
            await _client.StopSongAsync();
        }
    }
}
