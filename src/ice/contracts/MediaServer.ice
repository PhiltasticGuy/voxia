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
            StringArray FindSongsByTitle(string title);
            StringArray FindSongsByArtist(string artist);

            // Playback Controls
            ["amd"] bool PlaySong(string clientId, string filename);
            bool PauseSong(string clientId);
            bool StopSong(string clientId);

            // Song Management
            bool AddSong(Song song);
            bool UpdateSong(Song song);
            bool DeleteSong(string filename);

            void printString(string s);
            string getLibraryContent();
        
            ["amd"] void uploadFile(ByteArray file);
            ["amd"] void uploadFileChunk(string filename, int offset, ByteArray file);
        }
    }
}