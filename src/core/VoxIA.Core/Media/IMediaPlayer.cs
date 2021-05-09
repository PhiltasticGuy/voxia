using System.Threading.Tasks;

namespace VoxIA.Core.Media
{
    public interface IMediaPlayer
    {
        Task InitializeAsync(Song song);

        void Play();

        void Pause();
    }
}
