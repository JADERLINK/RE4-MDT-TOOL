using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_MDT_MERGE_MULTI8_UHD
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine("RE4_MDT_MERGE_MULTI8_UHD");
            Console.WriteLine("by: JADERLINK");
            Console.WriteLine("youtube.com/@JADERLINK");
            Console.WriteLine("github.com/JADERLINK");
            Console.WriteLine("Version 1.2 (2025-01-29)");
            Console.WriteLine("");


            for (int i = 0; i < args.Length; i++)
            {
                if (File.Exists(args[i]))
                {
                    try
                    {
                        Action(args[i]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + args[i]);
                        Console.WriteLine(ex);
                    }
                }
            }

            Console.WriteLine("Finished!!!");
            Console.WriteLine("Press any key to close the console.");
            Console.ReadKey();
        }

        static void Action(string file)
        {
            var fileInfo = new FileInfo(file);
            Console.WriteLine("File: " + fileInfo.Name);
            var Extension = Path.GetExtension(fileInfo.Name).ToUpperInvariant();
            if (!Extension.Contains("MDT"))
            {
                throw new ArgumentException("Invalid file!");
            }

            var (multiLang, outputFullName) = RE4_MDT_MERGE_MULTI.MakeMergedMulti.MakeMergedMulti_All(fileInfo.FullName, false, true); //UHD multi8

            var outputFileInfo = new FileInfo(outputFullName);
            var outStream = outputFileInfo.OpenWrite();
            RE4_MDT_PARSE.MakeMDT.Make_UHD_NS(multiLang, outStream, false); //UHD multi8
            outStream.Close();
        }
    }
}
