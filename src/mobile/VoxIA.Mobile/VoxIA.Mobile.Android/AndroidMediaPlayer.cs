using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media.Session;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxIA.Mobile.Services;
using static Android.Media.MediaPlayer;

namespace VoxIA.Mobile.Droid
{
    public class AndroidMediaPlayer : Service, IMediaPlayer, IOnBufferingUpdateListener, IOnPreparedListener
    {
        private readonly MediaPlayer _player;

        public AndroidMediaPlayer()
        {
            _player = new MediaPlayer();

            //_player = MediaPlayer.Create(Application.Context, Resource.Raw.Spring_Chicken);
            //_player.Prepare();
        }

        IBinder binder;

        public override IBinder OnBind(Intent intent)
        {
            binder = new MediaPlayerServiceBinder(this);
            return binder;
        }

        public override bool OnUnbind(Intent intent)
        {
            //StopNotification();
            return base.OnUnbind(intent);
        }

        public void OnBufferingUpdate(MediaPlayer mp, int percent)
        {
            int duration = 0;
            //if (MediaPlayerState == PlaybackStateCompat.StatePlaying || MediaPlayerState == PlaybackStateCompat.StatePaused)
            //    duration = mp.Duration;

            //int newBufferedTime = duration * percent / 100;
            //if (newBufferedTime != Buffered)
            //{
            //    Buffered = newBufferedTime;
            //}
        }

        public void Pause()
        {
            _player.Pause();
        }

        public void Play(string url)
        {
            try
            {
                _player.SetDataSource(url);
            }
            catch (IllegalArgumentException e)
            {
                e.PrintStackTrace();
            }
            catch (IllegalStateException e)
            {
                e.PrintStackTrace();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }

            _player.SetOnBufferingUpdateListener(this);
            _player.Prepare();
            var info = _player.GetTrackInfo();
            _player.Start();
            //_player.SetOnPreparedListener(this);
        }

        public void OnPrepared(MediaPlayer mp)
        {
            _player.Start();
        }
    }

    public class MediaPlayerServiceBinder : Binder
    {
        private AndroidMediaPlayer service;

        public MediaPlayerServiceBinder(AndroidMediaPlayer service)
        {
            this.service = service;
        }

        public AndroidMediaPlayer GetMediaPlayerService()
        {
            return service;
        }
    }
}