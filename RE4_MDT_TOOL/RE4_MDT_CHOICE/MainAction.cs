using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SimpleEndianBinaryIO;

namespace RE4_MDT_CHOICE
{
    internal static class MainAction
    {
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
                var diretory = Path.GetDirectoryName(fileInfo.FullName);
                var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
                var outputFile = Path.Combine(diretory, name);

                FileStream stream = null;
                RE4_MDT_PARSE.MultiLang multiLang = null;
                uint[] offsets = null;

                try
                {
                    stream = fileInfo.OpenRead();
                    if (IsPS4)
                    {
                        multiLang = RE4_MDT_PARSE.ParseMDT.Parse_PS4(stream);
                        offsets = Choice.GetOffset_PS4(stream);
                    }
                    else 
                    {
                        multiLang = RE4_MDT_PARSE.ParseMDT.Parse_UHD_NS(stream, Is64bits, endianness);
                        offsets = Choice.GetOffset_UHD_NS_BIG(stream, endianness);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally 
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
                
                Choice.MakeIdxChoiceMDT(multiLang, offsets, Is64bits, outputFile, endianness);
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
