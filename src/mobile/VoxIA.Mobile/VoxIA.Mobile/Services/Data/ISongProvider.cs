﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VoxIA.Mobile.Models;

namespace VoxIA.Mobile.Services.Data
{
    public interface ISongProvider
    {
        Task<Song> GetSongByIdAsync(string id);

        Task<IReadOnlyList<Song>> GetAllSongsAsync();

        Task<IReadOnlyList<Song>> GetSongsByQueryAsync(string query);
    }
}