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
        }

        sequence<Song> SongArray;
        
        enum RegisterResult
        { 
            Success,
            AlreadyRegistered,
            MaxClientsReached,
            UnknownClient
        }

        class RegisterResponse
        {
            RegisterResult result;
            string StreamingUrl;
        }

        interface MediaServer
        {
            ["amd"] RegisterResponse RegisterClient(string clientId);
            ["amd"] bool UnregisterClient(string clientId);

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