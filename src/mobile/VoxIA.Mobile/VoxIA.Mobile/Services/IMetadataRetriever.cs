﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VoxIA.Mobile.Models;

namespace VoxIA.Mobile.Services
{
    public interface IMetadataRetriever
    {
        Task<Song> PopulateMetadataAsync(Song song);
    }
}
