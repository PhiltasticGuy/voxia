using System.Threading.Tasks;
using VoxIA.Mobile.Models;

namespace VoxIA.Mobile.Services.Media
{
    public interface IMediaPlayer
    {
        Task InitializeAsync(Song song);

        void Play();

        void Pause();
    }
}
