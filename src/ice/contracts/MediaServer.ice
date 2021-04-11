module VoxIA
{
    module ZerocIce
    {
        // ["clr:generic:List"]
        // sequence<MyClass> MyClassList;
        sequence<byte> ByteArray;
        sequence<string> StringArray;

        class Song
        {
            string Id;
            string Title;
            string Artist;
            string Url;
        };

        sequence<Song> SongArray;

        interface MediaServer
        {
            // Song Library
            ["amd"] SongArray GetAllSongs();
            ["amd"] SongArray FindSongs(string query);

            // Playback Controls
            ["amd"] bool PlaySong(string clientId, string filename);
            bool PauseSong(string clientId);
            bool StopSong(string clientId);

            // Song Management
            ["amd"] void UploadSong(string filename, ByteArray content);
            ["amd"] void UploadSongChunk(string filename, int offset, ByteArray content);
            ["amd"] bool UpdateSong(Song song);
            bool DeleteSong(string filename);
        }
    }
}