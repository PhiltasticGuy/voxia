using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using VoxIA.Mobile.Models;
using VoxIA.Mobile.Services;
using Xamarin.Forms;

namespace VoxIA.Mobile.ViewModels
{
    [QueryProperty(nameof(SongId), nameof(SongId))]
    public class SongDetailsViewModel : BaseViewModel
    {
        private ISongProvider SongProvider => DependencyService.Get<ISongProvider>();

        public string SongId
        {
            get => Id;
            set
            {
                Id = value;
                LoadSongById(value);
            }
        }

        private string _songTitle;
        private string _artistName;
        private string _albumCover;

        public string Id { get; set; }

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

        public string AlbumCover
        {
            get => _albumCover;
            set => SetProperty(ref _albumCover, value);
        }

        public async void LoadSongById(string id)
        {
            try
            {
                var song = await SongProvider.GetSongByIdAsync(id);

                if (song != null)
                {
                    Id = song.Id;
                    SongTitle = song.Title;
                    ArtistName = song.ArtistName;
                    AlbumCover = song.AlbumCover;
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
