using System.Threading.Tasks;

namespace VoxIA.Core.Media
{
    public interface IMetadataRetriever
    {
        Task<Song> PopulateMetadataAsync(Song song);
    }
}
