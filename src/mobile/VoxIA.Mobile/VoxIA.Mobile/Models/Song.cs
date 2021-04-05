using System;
using System.Collections.Generic;
using System.Text;

namespace VoxIA.Mobile.Models
{
    public class Song
    {
        public string Id { get; set; }
        
        public string Title { get; set; }

        public string ArtistName { get; set; }

        public string AlbumCover { get; set; }

        public int Length { get; set; }
    }
}
