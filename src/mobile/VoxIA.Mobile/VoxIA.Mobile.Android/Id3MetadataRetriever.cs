using Android.Media;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoxIA.Core.Media;

namespace VoxIA.Mobile.Droid
{
    public class Id3MetadataRetriever : IMetadataRetriever
    {
        public Id3MetadataRetriever()
        {
        }

        public Song RetrieveMetadata(string url)
        {
            MediaMetadataRetriever _metadataRetriever = null;
            
            try
            {
                _metadataRetriever = new MediaMetadataRetriever();
                _metadataRetriever.SetDataSourceAsync(url, new Dictionary<string, string>());

                Song song = new Song()
                {
                    Title = _metadataRetriever.ExtractMetadata(MetadataKey.Title),
                    ArtistName = _metadataRetriever.ExtractMetadata(MetadataKey.Artist)
                };
                var album = _metadataRetriever.ExtractMetadata(MetadataKey.Album);
                var duration = _metadataRetriever.ExtractMetadata(MetadataKey.Duration);
                if (!string.IsNullOrEmpty(duration) && int.TryParse(duration, out int durationResult))
                {
                    song.Length = (int)TimeSpan.FromMilliseconds(durationResult).TotalSeconds;
                }

                return song;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (_metadataRetriever != null)
                {
                    _metadataRetriever.Release();
                    _metadataRetriever.Dispose();
                }
            }
        }

        public async Task<Song> PopulateMetadataAsync(Uri uri)
        {
            MediaMetadataRetriever _metadataRetriever = null;

            try
            {
                _metadataRetriever = new MediaMetadataRetriever();
                _metadataRetriever.SetDataSource(uri.AbsoluteUri, new Dictionary<string, string>());
                //await _metadataRetriever.SetDataSourceAsync(song.Url);

                Song song = new Song();
                song.Title = _metadataRetriever.ExtractMetadata(MetadataKey.Title);
                song.ArtistName = _metadataRetriever.ExtractMetadata(MetadataKey.Artist);
                var duration = _metadataRetriever.ExtractMetadata(MetadataKey.Duration);
                if (!string.IsNullOrEmpty(duration) && int.TryParse(duration, out int durationResult))
                {
                    song.Length = (int)TimeSpan.FromMilliseconds(durationResult).TotalSeconds;
                }

                var album = _metadataRetriever.ExtractMetadata(MetadataKey.Album);

                return song;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (_metadataRetriever != null)
                {
                    _metadataRetriever.Release();
                    _metadataRetriever.Dispose();
                }
            }
        }
    }
}