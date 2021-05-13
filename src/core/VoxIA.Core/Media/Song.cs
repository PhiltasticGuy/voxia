namespace VoxIA.Core.Media
{
    public class Song
    {

        public string Id { get; set; }

        public string Title { get; set; }

        public string ArtistName { get; set; }

        public string AlbumCover { get; set; }

        public int Length { get; set; }

        //public string Filename { get; set; }

        public Song()
        {
            Id = "";
            Title = "";
            ArtistName = "";
            AlbumCover = "";
            Length = 0;
            //Filename = "";
        }
    }
}
