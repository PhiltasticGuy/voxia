using System.Threading.Tasks;
using VoxIA.Mobile.Models;

namespace VoxIA.Mobile.Services
{
    public interface IMediaPlayer
    {
        Task PlayAsync(Song song);

        void Pause();
    }
}
