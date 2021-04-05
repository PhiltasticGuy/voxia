using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxIA.Mobile.Services;

namespace VoxIA.Mobile.Droid
{
    public class AndroidMediaPlayer : IMediaPlayer
    {
        private readonly MediaPlayer _player;

        public AndroidMediaPlayer()
        {
            _player = MediaPlayer.Create(Application.Context, Resource.Raw.PaulCantrell_Chopin_PreludeOp28No4);
            //_player.Prepare();
        }

        public void Pause()
        {
            _player.Pause();
        }

        public void Play()
        {
            _player.Start();
        }
    }
}