using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MultiPNGToHAR
{
    internal class Program
    {
        private static readonly List<byte> _header = new List<byte>
        {
            0x48, 0x41, 0x52, 0x43, 0xE0, 0x02, 0x00, 0x00, 0x8F, 0xA1, 0x72, 0x01
        };

        public static void Main(string[] args)
        {
            Console.WriteLine(
                "Input path containing PNG Files sorted by numerical value according to original HAR file.");
            var _pathToPNGs = Console.ReadLine();
            if (Directory.Exists(_pathToPNGs))
            {
                var files = Directory.GetFiles(_pathToPNGs, "*.png");
                var maxlen = files.Max(x => x.Length);
                var sortedFiles = files.OrderBy(x => x.PadLeft(maxlen, '0'));
                var _pngBytes = new List<byte>();
                foreach (var file in sortedFiles)
                {
                    var reader = new BinaryReader(new FileStream(file, FileMode.Open, FileAccess.ReadWrite));
                    reader.BaseStream.Position = 0x00;
                    _pngBytes.AddRange(reader.ReadBytes((int) reader.BaseStream.Length).ToList());
                    reader.Close();
                }

                _header.AddRange(_pngBytes);
                _header.Add(0x82);
                File.WriteAllBytes(Directory.GetCurrentDirectory() + @"\New.har", _header.ToArray());

                Console.WriteLine("Complete! Press any key to exit.");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("ERROR! Could not find path! Please make sure you have inputted path correctly!");
                Console.ReadKey();
            }
        }
    }
}