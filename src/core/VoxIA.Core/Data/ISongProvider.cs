using System.Collections.Generic;
using System.Threading.Tasks;
using VoxIA.Core.Media;

namespace VoxIA.Core.Data
{
    public interface ISongProvider
    {
        Task<Song> GetSongByIdAsync(string id);

        Task<IReadOnlyList<Song>> GetAllSongsAsync();

        Task<IReadOnlyList<Song>> GetSongsByQueryAsync(string query);
    }
}
