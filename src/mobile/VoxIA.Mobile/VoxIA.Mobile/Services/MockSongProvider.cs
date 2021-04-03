using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                new Song { Id = Guid.NewGuid().ToString(), Title = "First item", ArtistName="This is an item description." },
                new Song { Id = Guid.NewGuid().ToString(), Title = "Second item", ArtistName="This is an item description." },
                new Song { Id = Guid.NewGuid().ToString(), Title = "Third item", ArtistName="This is an item description." },
                new Song { Id = Guid.NewGuid().ToString(), Title = "Fourth item", ArtistName="This is an item description." },
                new Song { Id = Guid.NewGuid().ToString(), Title = "Fifth item", ArtistName="This is an item description." },
                new Song { Id = Guid.NewGuid().ToString(), Title = "Sixth item", ArtistName="This is an item description." }
            };
        }

        public async Task<IReadOnlyList<Song>> GetAllSongsAsync()
        {
            return await Task.FromResult(_songs);
        }

        public async Task<Song> GetSongByIdAsync(string id)
        {
            return await Task.FromResult(_songs.FirstOrDefault(_ => _.Id == id));
        }
    }
}
