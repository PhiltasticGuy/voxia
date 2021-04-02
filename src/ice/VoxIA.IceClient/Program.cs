using Demo;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IceSoup.Client
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                using var communicator = Ice.Util.initialize(ref args);
                RunIceClient(communicator);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return 1;
            }

            Console.ReadLine();

            return 0;
        }

        private static void RunIceClient(Ice.Communicator communicator)
        {
            var obj = communicator.stringToProxy("SimplePrinter:default -h localhost -p 10000");
            var printer = PrinterPrxHelper.checkedCast(obj);
            if (printer == null)
            {
                throw new ApplicationException("Invalid proxy");
            }

            var content = File.ReadAllBytes($"./local-lib/lorem.txt");
            var results = printer.begin_uploadFile(content);
            //printer.uploadFileAsync(content);

            Task.Run(() =>
            {
                string filename = "lorem.txt";
                string filepath = $"./local-lib/{filename}";
                const int chunkSize = 1140;
                int offset = 0;
                using var fs = File.OpenRead(filepath);
                using var br = new BinaryReader(fs);

                while (br.PeekChar() != -1)
                {
                    byte[] chunk = br.ReadBytes(chunkSize);
                    printer.uploadFileChunkAsync(filename, offset, chunk);
                    offset += chunk.Length;

                    string utfString = Encoding.UTF8.GetString(chunk, 0, chunk.Length);
                    Console.WriteLine(utfString);
                }
            });

            Task.Run(() =>
            {
                string filename = "lorem2.txt";
                string filepath = $"./local-lib/{filename}";
                const int chunkSize = 1026;
                int offset = 0;
                using var fs = File.OpenRead(filepath);
                using var br = new BinaryReader(fs);

                while (br.PeekChar() != -1)
                {
                    byte[] chunk = br.ReadBytes(chunkSize);
                    printer.uploadFileChunkAsync(filename, offset, chunk);
                    offset += chunk.Length;

                    string utfString = Encoding.UTF8.GetString(chunk, 0, chunk.Length);
                    Console.WriteLine(utfString);
                }
            });

            Console.WriteLine("Test #1");
            Console.WriteLine("Test #2");
            Console.WriteLine("Test #3");
            printer.printString("Hello World!");
            //Console.WriteLine(printer.getLibraryContent());

            printer.end_uploadFile(results);
        }
    }
}
