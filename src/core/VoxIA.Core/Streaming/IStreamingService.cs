using System.Threading.Tasks;
using VoxIA.Core.Media;
using VoxIA.Core.Streaming;

namespace VoxIA.Mobile.Services.Streaming
{
    public interface IStreamingService
    {
        Task RegisterClient(Client client);
        Task StartStreaming(Song song);
        Task StopStreaming();

        //Task UploadSong(Song song);
        //Task UpdateSong(Song song);
        //Task DeleteSong(Song song);
    }
}
