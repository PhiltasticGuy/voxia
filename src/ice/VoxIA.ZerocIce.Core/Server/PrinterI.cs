using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VoxIA.ZerocIce.Core.Server
{
    public class PrinterI : Demo.PrinterDisp_
    {
        private readonly Mutex _mutex = new();

        public override string getLibraryContent(Ice.Current current = null)
        {
            var files = Directory.GetFiles($".\\stream-lib");

            StringBuilder sb = new();
            foreach (var f in files)
            {
                sb.AppendLine(f);
            }

            return sb.ToString();
        }

        public override void printString(string s, Ice.Current current = null)
        {
            Console.WriteLine(s);
        }

        public override Task uploadFileAsync(byte[] file, Ice.Current current = null)
        {
            return Task.Run(() => {
                MemoryStream ms = new(file);
                StreamReader sr = new(ms);
                var content = sr.ReadToEnd();

                File.WriteAllBytes("./upload-area/uploaded.txt", file);

                Thread.Sleep(5000);

                _mutex.WaitOne();
                Console.WriteLine(content);
                _mutex.ReleaseMutex();
            });
        }

        public override Task uploadFileChunkAsync(string filename, int offset, byte[] file, Ice.Current current = null)
        {
            return Task.Run(() =>
            {
                string content = string.Empty;
                {
                    using MemoryStream ms = new(file);
                    using StreamReader sr = new(ms);
                    content = sr.ReadToEnd();
                }

                _mutex.WaitOne();
                Console.WriteLine("###############################################################################");
                Console.WriteLine($"      Filename : {filename}");
                Console.WriteLine($"        Offset : {offset}");
                Console.WriteLine($" Buffer Length : {file.Length}");
                Console.WriteLine();
                Console.WriteLine($"       Content : {content}");
                Console.WriteLine("###############################################################################");

                try
                {
                    string filepath = $"./upload-area/{filename}";
                    if (offset == 0)
                    {
                        File.Delete(filepath);
                        File.WriteAllBytes(filepath, file);
                    }
                    else
                    {
                        using FileStream fs = new(filepath, FileMode.Append);
                        using BinaryWriter bw = new(fs);
                        bw.Write(file);
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                }
                _mutex.ReleaseMutex();
            });
        }
    }
}
