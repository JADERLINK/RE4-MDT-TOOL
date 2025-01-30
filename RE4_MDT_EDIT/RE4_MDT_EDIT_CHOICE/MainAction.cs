using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using SimpleEndianBinaryIO;

namespace RE4_MDT_EDIT_CHOICE
{
    internal static class MainAction
    {
        public static void Continue(string[] args, FileVersion version, bool isSplittedFiles)
        {
            ChoiseMDT choiseMDT = null;
            bool EncodingLoaded = false;

            bool hasChinese;
            switch (version)
            {
                case FileVersion.CHOICE_UHD:
                case FileVersion.CHOICE_NS:
                    hasChinese = true;
                    break;
                case FileVersion.CHOICE_PS4:
                case FileVersion.CHOICE_BIG:
                default:
                    hasChinese = false;
                    break;
            }

            try
            {
                string jsonContent = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChoiceEncoding.json"), Encoding.UTF8);
                choiseMDT = ChoiseMDT.GetChoiseMDT(jsonContent, hasChinese);
                EncodingLoaded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }

            bool usingBatFile = false;

            if (EncodingLoaded)
            {
                int start = 0;

                if (args.Length == 0)
                {
                    foreach (var item in choiseMDT.MdtEncodings)
                    {
                        Console.WriteLine($"== {item.Key} ==");
                        RE4_MDT_EDIT.MdtEncodingPrint.Print(item.Value);
                    }
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
                            Action(args[i], choiseMDT, version, isSplittedFiles);
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

        private static void Action(string file, ChoiseMDT choiseMDT, FileVersion version, bool isSplittedFiles)
        {
            var fileInfo = new FileInfo(file);
            Console.WriteLine("File: " + fileInfo.Name);
            var Extension = Path.GetExtension(fileInfo.Name).ToUpperInvariant();

            bool IsPS4 = false;
            bool Is64bits = false;
            Endianness endianness = Endianness.LittleEndian;

            switch (version)
            {
                case FileVersion.CHOICE_PS4:
                    IsPS4 = true;
                    Is64bits = true;
                    break;
                case FileVersion.CHOICE_NS:
                    Is64bits = true;
                    break;
                case FileVersion.CHOICE_BIG:
                    endianness = Endianness.BigEndian;
                    break;
            }

            if (Extension == ".MDT")
            {
                ExtractChoice.ToExtract(fileInfo, Is64bits, IsPS4, endianness, choiseMDT, isSplittedFiles);
            }
            else if (Extension == ".IDXCHOICEMDT")
            {
                RepackChoice.ToRepack(fileInfo, Is64bits, IsPS4, endianness, choiseMDT, isSplittedFiles);
            }
            else
            {
                Console.WriteLine("The extension is not valid: " + Extension);
            }
        }
    }
}
