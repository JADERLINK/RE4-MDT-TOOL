using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SimpleEndianBinaryIO;

namespace RE4_MDT_CHOICE
{
    internal static class MainAction
    {
        public const string Version = "Version 1.2 (2025-01-29)";

        public static void Continue(string[] args, bool Is64bits, bool IsPS4, Endianness endianness = Endianness.LittleEndian) 
        {
            bool usingBatFile = false;
            int start = 0;
            if (args.Length > 0 && args[0].ToLowerInvariant() == "-bat")
            {
                usingBatFile = true;
                start = 1;
            }

            for (int i = start; i < args.Length; i++)
            {
                if (File.Exists(args[i]))
                {
                    try
                    {
                        Action(args[i], Is64bits, IsPS4, endianness);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + args[i]);
                        Console.WriteLine(ex);
                    }
                }
            }

            Console.WriteLine("Finished!!!");
            if (!usingBatFile)
            {
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
        }

        private static void Action(string file, bool Is64bits, bool IsPS4, Endianness endianness)
        {
            var fileInfo = new FileInfo(file);
            Console.WriteLine("File: " + fileInfo.Name);
            var Extension = Path.GetExtension(fileInfo.Name).ToUpperInvariant();

            if (Extension == ".MDT")
            {
                Extract.ToExtract(fileInfo, Is64bits, IsPS4, endianness);
            }
            else if (Extension == ".IDXCHOICEMDT")
            {
                Repack.ToRepack(fileInfo, Is64bits, IsPS4, endianness);
            }
            else
            {
                Console.WriteLine("The extension is not valid: " + Extension);
            }
          
        }
    }
}
