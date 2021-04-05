using System;
using System.Collections.Generic;
using System.Text;

namespace VoxIA.Mobile.Services
{
    public interface IMediaPlayer
    {
        void Play(string url);

        void Pause();
    }
}
