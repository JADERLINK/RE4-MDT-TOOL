﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_MDT_SINGLE_MULTI_NS
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine("RE4_MDT_SINGLE_MULTI_NS");
            Console.WriteLine("by: JADERLINK");
            Console.WriteLine("youtube.com/@JADERLINK");
            Console.WriteLine("github.com/JADERLINK");
            Console.WriteLine("Version 1.0 (2025-01-01)");
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

            var diretory = Path.GetDirectoryName(fileInfo.FullName);
            var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            var outputFile = Path.Combine(diretory, name + ".single.MDT");

            var stream = fileInfo.OpenRead();
            var res = RE4_MDT_PARSE.ParseMDT.ParseMono(stream, 0, stream.Length, true); // NS
            stream.Close();

            var outputFileInfo = new FileInfo(outputFile);
            var outStream = outputFileInfo.OpenWrite();
            RE4_MDT_SINGLE_MULTI.MakeSingleMulti.MakeSingleMulti_UHD_NS(res, outStream, true); // NS
            outStream.Close();
        }
    }
}