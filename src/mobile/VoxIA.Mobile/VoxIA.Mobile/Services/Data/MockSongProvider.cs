using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoxIA.Core.Data;
using VoxIA.Core.Media;

namespace VoxIA.Mobile.Services.Data
{
    public class MockSongProvider : ISongProvider
    {
        private readonly List<Song> _songs;

        public MockSongProvider()
        {
            _songs = new List<Song>()
            {
                new Song {
                    Id = "https://freepd.com/music/Hold%20on%20a%20Sec.mp3",
                    Title = "Hold on a Sec",
                    ArtistName = "Bryan Teoh",
                    AlbumCover = "album_cover_generic.png",
                    Length = 125
                },
                new Song {
                    Id = "https://freepd.com/music/Beat%20Thee.mp3",
                    Title = "Beat Thee",
                    ArtistName = "Alexander Nakarada" ,
                    AlbumCover = "album_cover_generic.png",
                    Length = 191
                },
                new Song {
                    Id = "https://freepd.com/music/Spring%20Chicken.mp3",
                    Title = "Spring Chicken",
                    ArtistName = "Bryan Teoh" ,
                    AlbumCover = "album_cover_generic.png",
                    Length = 166
                },
                new Song {
                    Id = "https://freepd.com/music/Study%20and%20Relax.mp3",
                    Title = "Study and Relax",
                    ArtistName = "Kevin MacLeod",
                    AlbumCover = "album_cover_generic.png",
                    Length = 223
                },
                new Song {
                    Id = "https://freepd.com/music/The%20Celebrated%20Minuet.mp3",
                    Title = "The Celebrated Minuit",
                    ArtistName="Rafael Krux",
                    AlbumCover = "album_cover_generic.png",
                    Length = 216
                },
                new Song {
                    Id = "https://freepd.com/music/Wakka%20Wakka.mp3",
                    Title = "Wakka Wakka",
                    ArtistName="Bryan Teoh",
                    AlbumCover = "album_cover_generic.png",
                    Length = 121
                },
                new Song {
                    Id = "https://freepd.com/music/Mysterious%20Lights.mp3",
                    Title = "Mysterious Lights",
                    ArtistName="Bryan Teoh",
                    AlbumCover = "album_cover_generic.png",
                    Length = 178
                }
            };
        }

        public async Task<IReadOnlyList<Song>> GetAllSongsAsync()
        {
            return await Task.Run(() => _songs);
        }

        public async Task<IReadOnlyList<Song>> GetSongsByQueryAsync(string query)
        {
            string sanitizedQuery = query?.Trim().ToLower() ?? "";

            return await Task.Run(() =>
                {
                    // Simulate long-running task.
                    //Thread.Sleep(5000);

                    return _songs.FindAll(song =>
                    {
                        return
                            song.Title.ToLower().Contains(sanitizedQuery) ||
                            song.ArtistName.ToLower().Contains(sanitizedQuery);
                    });
                });
        }

        public async Task<Song> GetSongByIdAsync(string id)
        {
            return await Task.Run(() => _songs.FirstOrDefault(_ => _.Id == id));
        }
    }
}
