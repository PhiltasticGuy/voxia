using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VoxIA.ZerocIce.Core.Server
{
    public class MediaServer : MediaServerDisp_
    {
        private readonly Mutex _mutex = new();
        private readonly Dictionary<string, LibVlcPlaybackService> _services = new();

        public MediaServer()
        {
            LibVLCSharp.Shared.Core.Initialize();
        }

        public override async Task<Song[]> GetAllSongsAsync(Ice.Current current = null)
        {
            //TODO: Remove hard-coded folder!
            var files = Directory.GetFiles($"tracks");

            using var vlc = new LibVLC();

            List<Song> songs = new();
            foreach (var file in files)
            {
                using var media = new Media(vlc, file, FromType.FromPath);
                await media.Parse();
                songs.Add(
                    new Song()
                    {
                        Title = media.Meta(MetadataType.Title),
                        Artist = media.Meta(MetadataType.Artist),
                        Url = Path.GetFileName(file)
                    }
                );
            }

            return songs.ToArray();
        }

        public override async Task<Song[]> FindSongsAsync(string query, Ice.Current current = null)
        {
            string sanitizedQuery = query.Trim().ToLower();

            var songs = new List<Song>(await GetAllSongsAsync());
            return songs.FindAll(_ => {
                return
                    _.Title.ToLower().Contains(sanitizedQuery) ||
                    _.Artist.ToLower().Contains(sanitizedQuery);
            }).ToArray();
        }

        public override async Task<bool> PlaySongAsync(string clientId, string filename, Ice.Current current = null)
        {
            LibVlcPlaybackService service;
            if (_services.ContainsKey(clientId))
            {
                service = _services[clientId];
            }
            else
            {
                _services[clientId] = service = new LibVlcPlaybackService(false, "--no-video");
            }

            var song = new Song() { Url = filename };

            service.Playing += (sender, e) => Console.WriteLine($"[LibVLCSharp] : Started the stream for client '{clientId}'.");
            service.Paused += (sender, e) => Console.WriteLine($"[LibVLCSharp] : Paused the stream for client '{clientId}'.");
            service.Stopped += (sender, e) => Console.WriteLine($"[LibVLCSharp] : Stopped the stream for client '{clientId}'.");

            await service.InitializeAsync(new IceClient(), song);
            return service?.Play() == true;
        }

        public override bool PauseSong(string clientId, Ice.Current current = null)
        {
            LibVlcPlaybackService service;
            if (_services.ContainsKey(clientId))
            {
                service = _services[clientId];
            }
            else
            {
                _services[clientId] = service = new LibVlcPlaybackService(false, "--no-video");
            }

            service?.Pause();

            return true;
        }

        public override bool StopSong(string clientId, Ice.Current current = null)
        {
            LibVlcPlaybackService service;
            if (_services.ContainsKey(clientId))
            {
                service = _services[clientId];
            }
            else
            {
                _services[clientId] = service = new LibVlcPlaybackService(false, "--no-video");
            }

            service?.Stop();

            return true;
        }

        public override Task UploadSongAsync(string filename, byte[] content, Ice.Current current = null)
        {
            return Task.Run(() => {
                //TODO: Remove hard-coded folder!
                File.WriteAllBytes("./tracks/" + filename, content);
            });
        }

        public override Task UploadSongChunkAsync(string filename, int offset, byte[] content, Ice.Current current = null)
        {
            return Task.Run(() =>
            {
                //string content = string.Empty;
                //{
                //    using MemoryStream ms = new(file);
                //    using StreamReader sr = new(ms);
                //    content = sr.ReadToEnd();
                //}

                //_mutex.WaitOne();
                //Console.WriteLine("###############################################################################");
                //Console.WriteLine($"      Filename : {filename}");
                //Console.WriteLine($"        Offset : {offset}");
                //Console.WriteLine($" Buffer Length : {content.Length}");
                //Console.WriteLine();
                //Console.WriteLine($"       Content : {content}");
                //Console.WriteLine("###############################################################################");

                try
                {
                    //TODO: Remove hard-coded folder!
                    string filepath = $"./tracks/{filename}";
                    if (offset == 0)
                    {
                        File.Delete(filepath);
                        File.WriteAllBytes(filepath, content);
                    }
                    else
                    {
                        using FileStream fs = new(filepath, FileMode.Append);
                        using BinaryWriter bw = new(fs);
                        bw.Write(content);
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                }
                //_mutex.ReleaseMutex();
            });
        }

        public override Task<bool> UpdateSongAsync(Song song, Ice.Current current = null)
        {
            if (song == null)
            {
                //TODO: Log an error message!
                return Task.FromResult(false);
            }

            if (string.IsNullOrEmpty(song?.Url))
            {
                //TODO: Log an error message!
                return Task.FromResult(false);
            }

            if (string.IsNullOrEmpty(song?.Title))
            {
                //TODO: Log an error message!
                return Task.FromResult(false);
            }

            if (string.IsNullOrEmpty(song?.Artist))
            {
                //TODO: Log an error message!
                return Task.FromResult(false);
            }

            //TODO: Remove hard-coded folder!
            var filePath = "./tracks/" + song?.Url;
            if (!File.Exists(filePath))
            {
                //TODO: Log an error message!
                return Task.FromResult(false);
            }

            using var vlc = new LibVLC();
            using var media = new Media(vlc, filePath, FromType.FromPath);
            media.SetMeta(MetadataType.Title, song?.Title);
            media.SetMeta(MetadataType.Artist, song?.Artist);

            return Task.FromResult(media.SaveMeta());
        }

        public override bool DeleteSong(string filename, Ice.Current current = null)
        {
            //TODO: Remove hard-coded folder!
            string path = "./tracks/" + filename;
            if (!File.Exists(path))
            {
                //TODO: Log an error message!
                return false;
            }

            File.Delete(path);
            return true;
        }
    }
}
