﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoxIA.Mobile.Models;
using VoxIA.Mobile.Services;
using Xamarin.Forms;

namespace VoxIA.Mobile.ViewModels
{
    // TODO: Should be a difference between CurrentlyPlaying and SongDetails! Split this up...

    [QueryProperty(nameof(SongId), nameof(SongId))]
    public class CurrentSongViewModel : BaseViewModel
    {
        public const string ICON_MEDIA_PLAY = "\uf04b";
        public const string ICON_MEDIA_PAUSE = "\uf04c";
        public const string ICON_MEDIA_PREVIOUS = "\uf048";
        public const string ICON_MEDIA_NEXT = "\uf051";

        public string Test => ICON_MEDIA_PLAY;

        private ISongProvider SongProvider => DependencyService.Get<ISongProvider>();

        private string _songId;

        private string _songTitle;
        private string _artistName;
        private bool _isPlaying;

        public string SongId
        {
            get
            {
                return _songId;
            }
            set
            {
                _songId = value;
                LoadSongById(value);
            }
        }

        public string Id { get; set; }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => SetProperty(ref _isPlaying, value); 
        }
        public bool IsPaused => !IsPlaying;

        public string SongTitle
        {
            get => _songTitle;
            set => SetProperty(ref _songTitle, value);
        }

        public string ArtistName
        {
            get => _artistName;
            set => SetProperty(ref _artistName, value);
        }

        public CurrentSongViewModel()
        {
            Title = "Currently Playing";
            OpenWebCommand = new Command(() => {
                IsPlaying = !IsPlaying;
                OnPropertyChanged(nameof(IsPaused));
            });
        }

        public ICommand OpenWebCommand { get; }

        private async void LoadSongById(string id)
        {
            try
            {
                var song = await SongProvider.GetSongByIdAsync(id);

                if (song != null)
                {
                    Id = song.Id;
                    SongTitle = song.Title;
                    ArtistName = song.ArtistName;

                    Application.Current.Properties["currentSongId"] = id;
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
