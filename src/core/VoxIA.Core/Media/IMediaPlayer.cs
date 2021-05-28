using System;
using System.Threading.Tasks;

namespace VoxIA.Core.Media
{
    public interface IMediaPlayer
    {
        bool IsPlaying { get; }
        string CurrentlyPlayingSongId { get; }

        Task InitializeAsync(string songId, Uri uri);

        void Play();

        void Pause();

        public void DeafenVolume();

        public void ResetVolume();
    }
}
