using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RE4_MDT_EDIT_MULTI
{
    internal static class MainAction
    {
        public const string Version = "Version 1.0 (2025-01-01)";

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
                string jsonContent = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MdtEncoding.json"), Encoding.UTF8);
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
                    Console.WriteLine("== MdtEncoding.json ==");
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
                var diretory = Path.GetDirectoryName(fileInfo.FullName);
                var name = Path.GetFileNameWithoutExtension(fileInfo.Name);

                RE4_MDT_PARSE.MultiLang multiLang = null;

                var stream = fileInfo.OpenRead();
                switch (version)
                {
                    case FileVersion.MULTI8_UHD:
                    case FileVersion.MULTI6_UHD:
                        multiLang = RE4_MDT_PARSE.ParseMDT.Parse_UHD_NS(stream, false, SimpleEndianBinaryIO.Endianness.LittleEndian);
                        break;
                    case FileVersion.MULTI_PS4:
                        multiLang = RE4_MDT_PARSE.ParseMDT.Parse_PS4(stream);
                        break;
                    case FileVersion.MULTI_NS:
                        multiLang = RE4_MDT_PARSE.ParseMDT.Parse_UHD_NS(stream, true, SimpleEndianBinaryIO.Endianness.LittleEndian);
                        break;
                    case FileVersion.MULTI_BIG:
                        multiLang = RE4_MDT_PARSE.ParseMDT.Parse_UHD_NS(stream, false, SimpleEndianBinaryIO.Endianness.BigEndian);
                        break;
                }
                stream.Close();

                MonoLangParsed[] langs = new MonoLangParsed[8];
                langs[0] = new MonoLangParsed(multiLang.Langs[0].Magic, RE4_MDT_EDIT.Extract.Extract_All(multiLang.Langs[0], mdtEncoding_Japanese0, isSplittedFiles), mdtEncoding_Japanese0.InfoTitle);
                langs[1] = new MonoLangParsed(multiLang.Langs[1].Magic, RE4_MDT_EDIT.Extract.Extract_All(multiLang.Langs[1], mdtEncoding, isSplittedFiles), mdtEncoding.InfoTitle);
                langs[2] = new MonoLangParsed(multiLang.Langs[2].Magic, RE4_MDT_EDIT.Extract.Extract_All(multiLang.Langs[2], mdtEncoding, isSplittedFiles), mdtEncoding.InfoTitle);
                langs[3] = new MonoLangParsed(multiLang.Langs[3].Magic, RE4_MDT_EDIT.Extract.Extract_All(multiLang.Langs[3], mdtEncoding, isSplittedFiles), mdtEncoding.InfoTitle);
                langs[4] = new MonoLangParsed(multiLang.Langs[4].Magic, RE4_MDT_EDIT.Extract.Extract_All(multiLang.Langs[4], mdtEncoding, isSplittedFiles), mdtEncoding.InfoTitle);
                langs[5] = new MonoLangParsed(multiLang.Langs[5].Magic, RE4_MDT_EDIT.Extract.Extract_All(multiLang.Langs[5], mdtEncoding, isSplittedFiles), mdtEncoding.InfoTitle);
                if (hasChinese)
                {
                    langs[6] = new MonoLangParsed(multiLang.Langs[6].Magic, RE4_MDT_EDIT.Extract.Extract_All(multiLang.Langs[6], mdtEncoding_Chinese6, isSplittedFiles), mdtEncoding_Chinese6.InfoTitle);
                    langs[7] = new MonoLangParsed(multiLang.Langs[7].Magic, RE4_MDT_EDIT.Extract.Extract_All(multiLang.Langs[7], mdtEncoding_Chinese9, isSplittedFiles), mdtEncoding_Chinese9.InfoTitle);
                }
                else 
                {
                    langs[6] = new MonoLangParsed(0, new string[0], "");
                    langs[7] = new MonoLangParsed(0, new string[0], "");
                }
           
                MakeStractedMulti.MakeFiles(langs, name, diretory, isSplittedFiles, hasChinese);
                Console.WriteLine($"Extracted {langs[0].Lines.Length} entries in 0_Japanese.");
                Console.WriteLine($"Extracted {langs[1].Lines.Length} entries in 1_English.");
                Console.WriteLine($"Extracted {langs[2].Lines.Length} entries in 2_French.");
                Console.WriteLine($"Extracted {langs[3].Lines.Length} entries in 3_German.");
                Console.WriteLine($"Extracted {langs[4].Lines.Length} entries in 4_Italian.");
                Console.WriteLine($"Extracted {langs[5].Lines.Length} entries in 5_Spanish.");
                if (hasChinese)
                {
                    Console.WriteLine($"Extracted {langs[6].Lines.Length} entries in 6_Chinese_zh_tw.");
                    Console.WriteLine($"Extracted {langs[7].Lines.Length} entries in 9_Chinese_zh_cn.");
                }
            }
            else if (Extension == ".IDXMULTIMDT")
            {
                string diretory = Path.GetDirectoryName(fileInfo.FullName);
                string fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);

                uint[] magic = GetRepackedMulti.GetMagic(fileInfo.FullName);

                string[] lines0 = GetRepackedMulti.GetLines(Path.Combine(diretory, fileName + ".0_Japanese"), isSplittedFiles);
                string[] lines1 = GetRepackedMulti.GetLines(Path.Combine(diretory, fileName + ".1_English"), isSplittedFiles);
                string[] lines2 = GetRepackedMulti.GetLines(Path.Combine(diretory, fileName + ".2_French"), isSplittedFiles);
                string[] lines3 = GetRepackedMulti.GetLines(Path.Combine(diretory, fileName + ".3_German"), isSplittedFiles);
                string[] lines4 = GetRepackedMulti.GetLines(Path.Combine(diretory, fileName + ".4_Italian"), isSplittedFiles);
                string[] lines5 = GetRepackedMulti.GetLines(Path.Combine(diretory, fileName + ".5_Spanish"), isSplittedFiles);
                string[] lines6 = new string[0];
                string[] lines7 = new string[0];
                if (hasChinese)
                {
                    lines6 = GetRepackedMulti.GetLines(Path.Combine(diretory, fileName + ".6_Chinese_zh_tw"), isSplittedFiles);
                    lines7 = GetRepackedMulti.GetLines(Path.Combine(diretory, fileName + ".9_Chinese_zh_cn"), isSplittedFiles);
                }
            
                var encodedlines0 = RE4_MDT_EDIT.Repack.Encoder(lines0, mdtEncoding_Japanese0);
                var encodedlines1 = RE4_MDT_EDIT.Repack.Encoder(lines1, mdtEncoding);
                var encodedlines2 = RE4_MDT_EDIT.Repack.Encoder(lines2, mdtEncoding);
                var encodedlines3 = RE4_MDT_EDIT.Repack.Encoder(lines3, mdtEncoding);
                var encodedlines4 = RE4_MDT_EDIT.Repack.Encoder(lines4, mdtEncoding);
                var encodedlines5 = RE4_MDT_EDIT.Repack.Encoder(lines5, mdtEncoding);
                (ushort[] charArr, uint[] offsetList) encodedlines6 = (new ushort[0], new uint[0]);
                (ushort[] charArr, uint[] offsetList) encodedlines7 = (new ushort[0], new uint[0]);
                if (hasChinese)
                {
                    encodedlines6 = RE4_MDT_EDIT.Repack.Encoder(lines6, mdtEncoding_Chinese6);
                    encodedlines7 = RE4_MDT_EDIT.Repack.Encoder(lines7, mdtEncoding_Chinese9);
                }

                RE4_MDT_PARSE.MonoLang[] langs = new RE4_MDT_PARSE.MonoLang[8];
                langs[0] = new RE4_MDT_PARSE.MonoLang(magic[0], encodedlines0.charArr, encodedlines0.offsetList);
                langs[1] = new RE4_MDT_PARSE.MonoLang(magic[1], encodedlines1.charArr, encodedlines1.offsetList);
                langs[2] = new RE4_MDT_PARSE.MonoLang(magic[2], encodedlines2.charArr, encodedlines2.offsetList);
                langs[3] = new RE4_MDT_PARSE.MonoLang(magic[3], encodedlines3.charArr, encodedlines3.offsetList);
                langs[4] = new RE4_MDT_PARSE.MonoLang(magic[4], encodedlines4.charArr, encodedlines4.offsetList);
                langs[5] = new RE4_MDT_PARSE.MonoLang(magic[5], encodedlines5.charArr, encodedlines5.offsetList);
                langs[6] = new RE4_MDT_PARSE.MonoLang(magic[6], encodedlines6.charArr, encodedlines6.offsetList);
                langs[7] = new RE4_MDT_PARSE.MonoLang(magic[7], encodedlines7.charArr, encodedlines7.offsetList);
                RE4_MDT_PARSE.MultiLang multiLang = new RE4_MDT_PARSE.MultiLang(langs);

                var outputFile = Path.Combine(diretory, fileName + ".MDT");
                var outputFileInfo = new FileInfo(outputFile);
                var outStream = outputFileInfo.OpenWrite();

                switch (version)
                {
                    case FileVersion.MULTI8_UHD:
                        RE4_MDT_PARSE.MakeMDT.Make_UHD_NS(multiLang, outStream, false, SimpleEndianBinaryIO.Endianness.LittleEndian);
                        break;
                    case FileVersion.MULTI_PS4:
                        RE4_MDT_PARSE.MakeMDT.Make_PS4(multiLang, outStream);
                        break;
                    case FileVersion.MULTI_NS:
                        RE4_MDT_PARSE.MakeMDT.Make_UHD_NS(multiLang, outStream, true, SimpleEndianBinaryIO.Endianness.LittleEndian);
                        break;
                    case FileVersion.MULTI_BIG:
                        RE4_MDT_PARSE.MakeMDT.Make_UHD_BIG(multiLang, outStream, SimpleEndianBinaryIO.Endianness.BigEndian);
                        break;
                    case FileVersion.MULTI6_UHD:
                        RE4_MDT_PARSE.MakeMDT.Make_UHD_BIG(multiLang, outStream, SimpleEndianBinaryIO.Endianness.LittleEndian);
                        break;
                }
                outStream.Close();

                Console.WriteLine($"Repackaged {lines0.Length} entries in 0_Japanese.");
                Console.WriteLine($"Repackaged {lines1.Length} entries in 1_English.");
                Console.WriteLine($"Repackaged {lines2.Length} entries in 2_French.");
                Console.WriteLine($"Repackaged {lines3.Length} entries in 3_German.");
                Console.WriteLine($"Repackaged {lines4.Length} entries in 4_Italian.");
                Console.WriteLine($"Repackaged {lines5.Length} entries in 5_Spanish.");
                if (hasChinese)
                {
                    Console.WriteLine($"Repackaged {lines6.Length} entries in 6_Chinese_zh_tw.");
                    Console.WriteLine($"Repackaged {lines7.Length} entries in 9_Chinese_zh_cn.");
                }
              
            }
            else
            {
                Console.WriteLine("The extension is not valid: " + Extension);
            }
        }


    }
}
