using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RE4_MDT_EDIT_MULTI
{
    internal static class MainAction
    {
        public static void Continue(string[] args, FileVersion version, bool isSplittedFiles)
        {
            RE4_MDT_EDIT.MdtEncoding mdtEncoding = null;
            RE4_MDT_EDIT.MdtEncoding mdtEncoding_Japanese0 = null;
            RE4_MDT_EDIT.MdtEncoding mdtEncoding_Chinese6 = null;
            RE4_MDT_EDIT.MdtEncoding mdtEncoding_Chinese9 = null;

            bool hasChinese;
            switch (version)
            {
                case FileVersion.MULTI8_UHD:
                case FileVersion.MULTI_NS:
                    hasChinese = true;
                    break;
                case FileVersion.MULTI6_UHD:
                case FileVersion.MULTI_PS4:
                case FileVersion.MULTI_BIG:
                default:
                    hasChinese = false;
                    break;
            }

            try
            {
                string jsonContent = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MdtEncodingLatin.json"), Encoding.UTF8);
                mdtEncoding = RE4_MDT_EDIT.MdtEncodingLoader.ParseFromJson(jsonContent);

                string jsonContent0 = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MdtEncodingJapanese.json"), Encoding.UTF8);
                mdtEncoding_Japanese0 = RE4_MDT_EDIT.MdtEncodingLoader.ParseFromJson(jsonContent0);

                if (hasChinese)
                {
                    string jsonContent6 = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MdtEncodingChinese6.json"), Encoding.UTF8);
                    mdtEncoding_Chinese6 = RE4_MDT_EDIT.MdtEncodingLoader.ParseFromJson(jsonContent6);

                    string jsonContent9 = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MdtEncodingChinese9.json"), Encoding.UTF8);
                    mdtEncoding_Chinese9 = RE4_MDT_EDIT.MdtEncodingLoader.ParseFromJson(jsonContent9);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }

            bool usingBatFile = false;

            if (mdtEncoding != null && mdtEncoding_Japanese0 != null && ( !hasChinese || (mdtEncoding_Chinese6 != null && mdtEncoding_Chinese9 != null)))
            {
                int start = 0;

                if (args.Length == 0)
                {
                    Console.WriteLine("== MdtEncodingLatin.json ==");
                    RE4_MDT_EDIT.MdtEncodingPrint.Print(mdtEncoding);
                    Console.WriteLine("== MdtEncodingJapanese.json ==");
                    RE4_MDT_EDIT.MdtEncodingPrint.Print(mdtEncoding_Japanese0);
                    if (hasChinese)
                    {
                        Console.WriteLine("== MdtEncodingChinese6.json ==");
                        RE4_MDT_EDIT.MdtEncodingPrint.Print(mdtEncoding_Chinese6);
                        Console.WriteLine("== MdtEncodingChinese9.json ==");
                        RE4_MDT_EDIT.MdtEncodingPrint.Print(mdtEncoding_Chinese9);
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
                            Action(args[i], mdtEncoding, mdtEncoding_Japanese0, mdtEncoding_Chinese6, mdtEncoding_Chinese9, version, isSplittedFiles, hasChinese);
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

        private static void Action(string file,
            RE4_MDT_EDIT.MdtEncoding mdtEncoding,
            RE4_MDT_EDIT.MdtEncoding mdtEncoding_Japanese0,
            RE4_MDT_EDIT.MdtEncoding mdtEncoding_Chinese6,
            RE4_MDT_EDIT.MdtEncoding mdtEncoding_Chinese9,
            FileVersion version, bool isSplittedFiles, bool hasChinese)
        {
            var fileInfo = new FileInfo(file);
            Console.WriteLine("File: " + fileInfo.Name);
            var Extension = Path.GetExtension(fileInfo.Name).ToUpperInvariant();

            if (Extension == ".MDT")
            {
                ExtractMulti.ToExtract(fileInfo, mdtEncoding, mdtEncoding_Japanese0, mdtEncoding_Chinese6, mdtEncoding_Chinese9, version, isSplittedFiles, hasChinese);
            }
            else if (Extension == ".IDXMULTIMDT")
            {
                RepackMulti.ToRepack(fileInfo, mdtEncoding, mdtEncoding_Japanese0, mdtEncoding_Chinese6, mdtEncoding_Chinese9, version, isSplittedFiles, hasChinese);
            }
            else
            {
                Console.WriteLine("The extension is not valid: " + Extension);
            }
        }


    }
}
