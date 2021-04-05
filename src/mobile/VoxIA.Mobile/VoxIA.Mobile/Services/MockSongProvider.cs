using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoxIA.Mobile.Models;

namespace VoxIA.Mobile.Services
{
    public class MockSongProvider : ISongProvider
    {
        private readonly List<Song> _songs;

        public MockSongProvider()
        {
            _songs = new List<Song>()
            {
                new Song {
                    Id = "0",
                    Title = "Hold on a Sec",
                    ArtistName = "Bryan Tech",
                    AlbumCover = "album_cover_generic.png",
                    Url = "https://freepd.com/music/Hold%20on%20a%20Sec.mp3"
                },
                new Song {
                    Id = "1",
                    Title = "Beat Thee",
                    ArtistName = "Alexander Nakarada" ,
                    AlbumCover = "album_cover_generic.png",
                    Url = "https://freepd.com/music/Beat%20Thee.mp3"
                },
                new Song {
                    Id = "2",
                    Title = "Spring Chicken",
                    ArtistName = "Bryan Tech" ,
                    AlbumCover = "album_cover_generic.png",
                    Url = "https://freepd.com/music/Spring%20Chicken.mp3"
                },
                new Song {
                    Id = "3",
                    Title = "Study and Relax",
                    ArtistName = "Kevin MacLeod",
                    AlbumCover = "album_cover_generic.png",
                    Url = "https://freepd.com/music/Study%20and%20Relax.mp3"
                },
                new Song {
                    Id = "4",
                    Title = "The Celebrated Minuit",
                    ArtistName="Rafael Krux",
                    AlbumCover = "album_cover_generic.png",
                    Url = "https://freepd.com/music/The%20Celebrated%20Minuet.mp3"
                },
                new Song {
                    Id = "5",
                    Title = "Wakka Wakka",
                    ArtistName="Bryan Tech",
                    AlbumCover = "album_cover_generic.png",
                    Url = "https://freepd.com/music/Wakka%20Wakka.mp3"
                }
            };
            //_songs = new List<Song>()
            //{
            //    new Song {
            //        Id = Guid.NewGuid().ToString(),
            //        Title = "First item",
            //        ArtistName = "This is an item description.",
            //        AlbumCover = "album_cover_generic.png"
            //    },
            //    new Song {
            //        Id = Guid.NewGuid().ToString(),
            //        Title = "Second item",
            //        ArtistName = "This is an item description." ,
            //        AlbumCover = "album_cover_generic.png"
            //    },
            //    new Song {
            //        Id = Guid.NewGuid().ToString(),
            //        Title = "Third item",
            //        ArtistName = "This is an item description." ,
            //        AlbumCover = "album_cover_generic.png"
            //    },
            //    new Song {
            //        Id = Guid.NewGuid().ToString(),
            //        Title = "Fourth item",
            //        ArtistName = "This is an item description.",
            //        AlbumCover = "album_cover_generic.png"
            //    },
            //    new Song {
            //        Id = Guid.NewGuid().ToString(),
            //        Title = "Fifth item",
            //        ArtistName="This is an item description.",
            //        AlbumCover = "album_cover_generic.png"
            //    },
            //    new Song {
            //        Id = Guid.NewGuid().ToString(),
            //        Title = "Sixth item",
            //        ArtistName="This is an item description.",
            //        AlbumCover = "album_cover_generic.png"
            //    },
            //    new Song {
            //        Id = Guid.NewGuid().ToString(),
            //        Title = "Seventh item",
            //        ArtistName="This is an item description.",
            //        AlbumCover = "album_cover_generic.png"
            //    },
            //    new Song {
            //        Id = Guid.NewGuid().ToString(),
            //        Title = "Eighth item",
            //        ArtistName="This is an item description.",
            //        AlbumCover = "album_cover_generic.png"
            //    },
            //    new Song {
            //        Id = Guid.NewGuid().ToString(),
            //        Title = "Ninth item",
            //        ArtistName="This is an item description.",
            //        AlbumCover = "album_cover_generic.png"
            //    },
            //    new Song {
            //        Id = Guid.NewGuid().ToString(),
            //        Title = "Tenth item",
            //        ArtistName="This is an item description.",
            //        AlbumCover = "album_cover_generic.png"
            //    }
            //};
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
