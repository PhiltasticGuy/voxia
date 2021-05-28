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
        }

        sequence<Song> SongArray;

        interface MediaServer
        {
            // Song Library
            ["amd"] SongArray GetAllSongs(string clientId);
            ["amd"] SongArray FindSongs(string clientId, string query);

            // Playback Controls
            ["amd"] string PlaySong(string clientId, string filename);
            bool PauseSong(string clientId);
            bool StopSong(string clientId);

            // Song Management
            ["amd"] void UploadSong(string clientId, string filename, ByteArray content);
            ["amd"] void UploadSongChunk(string clientId, string filename, int offset, ByteArray content);
            ["amd"] bool UpdateSong(string clientId, Song song);
            bool DeleteSong(string clientId, string filename);
        }
    }
}