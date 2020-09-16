using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MultiPNGToHAR
{
    internal class Program
    {
        private static readonly List<byte> _header = new List<byte>
        {
            0x48, 0x41, 0x52, 0x43
        };

        private static string _file;
        private static int _fileCount;

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
                    _fileCount++;
                    reader.Close();
                }

                _file = Directory.GetCurrentDirectory() + @"\New.har";
                _pngBytes.Add(0x82);
                File.WriteAllBytes(_file, _pngBytes.ToArray());
                var newFile = ComputeInfo();
                newFile.AddRange(_pngBytes);
                File.Delete(_file);
                File.WriteAllBytes(_file, newFile.ToArray());


                Console.WriteLine("Complete, Press any key to exit.");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("ERROR! Could not find path! Please make sure you have inputted path correctly!");
                Console.ReadKey();
            }
        }

        public static List<byte> ComputeInfo()
        {
            var combinedHeader = _header;
            var currentFile = new FileInfo(_file);
            var fileSize = currentFile.Length;
            var fileSizeArr = BitConverter.GetBytes(fileSize).ToList();
            fileSizeArr.RemoveRange(4, fileSizeArr.Count() - 4);
            var fileCountArr = BitConverter.GetBytes(_fileCount).ToList();
            fileCountArr.RemoveRange(4, fileCountArr.Count() - 4);
            combinedHeader.AddRange(fileCountArr);
            combinedHeader.AddRange(fileSizeArr);

            return combinedHeader;
        }
    }
}