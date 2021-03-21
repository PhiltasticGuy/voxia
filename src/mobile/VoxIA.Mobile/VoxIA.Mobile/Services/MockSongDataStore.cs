using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxIA.Mobile.Models;

namespace VoxIA.Mobile.Services
{
    public class MockSongDataStore : IDataStore<Song>
    {
        private List<Song> _items;

        public MockSongDataStore()
        {
            _items = new List<Song>()
            {
                new Song { Id = Guid.NewGuid().ToString(), Title = "First item", ArtistName="This is an item description." },
                new Song { Id = Guid.NewGuid().ToString(), Title = "Second item", ArtistName="This is an item description." },
                new Song { Id = Guid.NewGuid().ToString(), Title = "Third item", ArtistName="This is an item description." },
                new Song { Id = Guid.NewGuid().ToString(), Title = "Fourth item", ArtistName="This is an item description." },
                new Song { Id = Guid.NewGuid().ToString(), Title = "Fifth item", ArtistName="This is an item description." },
                new Song { Id = Guid.NewGuid().ToString(), Title = "Sixth item", ArtistName="This is an item description." }
            };
        }

        public async Task<bool> AddItemAsync(Song item)
        {
            _items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = _items.Where((Song arg) => arg.Id == id).FirstOrDefault();
            _items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Song> GetItemAsync(string id)
        {
            return await Task.FromResult(_items.FirstOrDefault(_ => _.Id == id));
        }

        public async Task<IEnumerable<Song>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(_items);
        }

        public async Task<bool> UpdateItemAsync(Song item)
        {
            var oldItem = _items.Where((Song arg) => arg.Id == item.Id).FirstOrDefault();
            _items.Remove(oldItem);
            _items.Add(item);

            return await Task.FromResult(true);
        }
    }
}
