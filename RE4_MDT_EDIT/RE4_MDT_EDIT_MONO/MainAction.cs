using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SimpleEndianBinaryIO;

namespace RE4_MDT_EDIT_MONO
{
    internal static class MainAction
    {
        public const string Version = "Version 1.0 (2025-01-01)";

        public static void Continue(string[] args, bool is64Bits, Endianness endianness, bool isSplittedFiles) 
        {
            RE4_MDT_EDIT.MdtEncoding mdtEncoding = null;
            try
            {
                string jsonContent = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MdtEncoding.json"), Encoding.UTF8);
                mdtEncoding = RE4_MDT_EDIT.MdtEncodingLoader.ParseFromJson(jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }

            bool usingBatFile = false;

            if (mdtEncoding != null)
            {
                int start = 0;

                if (args.Length == 0)
                {
                    RE4_MDT_EDIT.MdtEncodingPrint.Print(mdtEncoding);
                }
                else 
                {
                    if (args[0].ToLowerInvariant() == "-bat")
                    {
                        usingBatFile = true;
                        start = 1;
                    }
                }

                for (int i = start; i < args.Length; i++)
                {
                    if (File.Exists(args[i]))
                    {
                        try
                        {
                            Action(args[i], mdtEncoding, is64Bits, endianness, isSplittedFiles);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + args[i]);
                            Console.WriteLine(ex);
                        }
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

        private static void Action(string file, RE4_MDT_EDIT.MdtEncoding mdtEncoding, bool is64Bits, Endianness endianness, bool isSplittedFiles)
        {
            var fileInfo = new FileInfo(file);
            Console.WriteLine("File: " + fileInfo.Name);
            var Extension = Path.GetExtension(fileInfo.Name).ToUpperInvariant();

            if (Extension == ".MDT")
            {
                var diretory = Path.GetDirectoryName(fileInfo.FullName);
                var name = Path.GetFileNameWithoutExtension(fileInfo.Name);

                var stream = fileInfo.OpenRead();
                var res = RE4_MDT_PARSE.ParseMDT.ParseMono(stream, 0, stream.Length, is64Bits, endianness); // all
                stream.Close();

                var lines = RE4_MDT_EDIT.Extract.Extract_All(res, mdtEncoding, isSplittedFiles);
                MakeStracted.MakeFiles(res.Magic, lines, name, diretory, mdtEncoding.InfoTitle, isSplittedFiles);
                Console.WriteLine($"Extracted {lines.Length} entries.");
            }
            else if (Extension == ".IDXMDT")
            {
                var magic = GetRepacked.GetMagic(fileInfo.FullName);
                var lines = GetRepacked.GetLines(fileInfo.FullName, isSplittedFiles);

                var encodedlines = RE4_MDT_EDIT.Repack.Encoder(lines, mdtEncoding);

                var mono = new RE4_MDT_PARSE.MonoLang(magic, encodedlines.charArr, encodedlines.offsetList);

                var diretory = Path.GetDirectoryName(fileInfo.FullName);
                var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
                var outputFile = Path.Combine(diretory, name + ".MDT");
                var outputFileInfo = new FileInfo(outputFile);
                var outStream = outputFileInfo.OpenWrite();
                RE4_MDT_PARSE.MakeMDT.MakeMono(mono, outStream, 0, is64Bits, out _, endianness); // all
                outStream.Close();
                Console.WriteLine($"Repackaged {lines.Length} entries.");
            }
            else
            {
                Console.WriteLine("The extension is not valid: " + Extension);
            }
        }
    }
}
