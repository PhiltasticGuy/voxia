using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxIA.Core.Data;
using VoxIA.Core.Media;
using VoxIA.ZerocIce.Core.Client;
using Xamarin.Forms;

namespace VoxIA.Mobile.Services.Data
{
    public class IceSongProvider : ISongProvider
    {
        private readonly GenericIceClient _client = DependencyService.Get<GenericIceClient>();

        public IceSongProvider()
        {

        }

        public async Task<IReadOnlyList<Song>> GetAllSongsAsync()
        {
            List<Song> allSongs = new List<Song>();

            var songs = await _client._mediaServer.GetAllSongsAsync();
            foreach (var song in songs)
            {
                //TODO: Album Cover could be loaded from mp3 files...
                allSongs.Add(new Song()
                {
                    Id = song.Id,
                    Title = song.Title,
                    Url = song.Url,
                    ArtistName = song.Artist,
                    AlbumCover = "album_cover_generic.png"
                });
            }

            return allSongs;
        }

        public async Task<Song> GetSongByIdAsync(string id)
        {
            return (await GetAllSongsAsync()).FirstOrDefault(_ => _.Id == id);
        }

        public async Task<IReadOnlyList<Song>> GetSongsByQueryAsync(string query)
        {
            List<Song> allSongs = new List<Song>();

            var songs = await _client._mediaServer.FindSongsAsync(query);
            foreach (var song in songs)
            {
                //TODO: Album Cover could be loaded from mp3 files...
                allSongs.Add(new Song()
                {
                    Id = song.Id,
                    Title = song.Title,
                    Url = song.Url,
                    ArtistName = song.Artist,
                    AlbumCover = "album_cover_generic.png"
                });
            }

            return allSongs;
        }
    }
}
