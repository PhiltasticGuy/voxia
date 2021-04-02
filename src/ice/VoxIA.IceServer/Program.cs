using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IceSoup.Server
{
    public class PrinterI : Demo.PrinterDisp_
    {
        private bool _isRunning = false;

        public void Destroy()
        {
            lock (this)
            {
                _isRunning = false;
            }
        }

        public void Run()
        {
            lock (this)
            {
                _isRunning = true;
                while (_isRunning)
                {
                    
                }
            }
        }

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

        //public override void uploadFile(Ice.Current current = null)
        //{
        //    var files = Directory.GetFiles($".\\stream-lib");

        //    StringBuilder sb = new StringBuilder();
        //    foreach (var f in files)
        //    {
        //        sb.AppendLine(f);
        //    }
        //}

        public override void printString(string s, Ice.Current current = null)
        {
            Console.WriteLine(s);
        }

        //public override void uploadFile(byte[] file, Ice.Current current = null)
        //{
        //    MemoryStream ms = new MemoryStream(file);
        //    StreamReader sr = new StreamReader(ms);
        //    var content = sr.ReadToEnd();

        //    File.WriteAllBytes("./upload-area/uploaded.txt", file);

        //    Thread.Sleep(60000);

        //    Console.WriteLine(content);
        //}

        public override Task uploadFileAsync(byte[] file, Ice.Current current = null)
        {
            return Task.Run(() => {
                MemoryStream ms = new(file);
                StreamReader sr = new(ms);
                var content = sr.ReadToEnd();

                File.WriteAllBytes("./upload-area/uploaded.txt", file);

                Thread.Sleep(60000);

                Console.WriteLine(content);
            });
        }

        readonly Mutex _mutex = new();

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

    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                using var communicator = Ice.Util.initialize(ref args);

                //
                // Destroy the communicator on Ctrl+C or Ctrl+Break
                //
                Console.CancelKeyPress += (sender, eventArgs) => communicator.destroy();

                RunIceServer(communicator);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return 1;
            }

            return 0;
        }

        private static void RunIceServer(Ice.Communicator communicator)
        {
            // Create the upload area directory.
            Directory.CreateDirectory("./upload-area");

            var adapter =
                communicator.createObjectAdapterWithEndpoints("SimplePrinterAdapter", "default -h localhost -p 10000");
            var server = new PrinterI();
            adapter.add(server, Ice.Util.stringToIdentity("SimplePrinter"));
            adapter.activate();
            //communicator.waitForShutdown();

            var t = new Thread(new ThreadStart(server.Run));
            t.Start();

            try
            {
                communicator.waitForShutdown();
            }
            finally
            {
                server.Destroy();
                t.Join();
            }
        }
    }
}
