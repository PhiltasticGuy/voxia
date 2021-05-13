using System;
using System.Threading.Tasks;

namespace VoxIA.Core.Media
{
    public interface IMediaPlayer
    {
        Task InitializeAsync(Uri uri);

        void Play();

        void Pause();

        public void DeafenVolume();

        public void ResetVolume();
    }
}
