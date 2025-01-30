using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RE4_MDT_EDIT_SHARED;

namespace RE4_MDT_EDIT_MULTI
{
    internal static class ExtractMulti
    {
        public static void ToExtract(FileInfo fileInfo,
            RE4_MDT_EDIT.MdtEncoding mdtEncoding,
            RE4_MDT_EDIT.MdtEncoding mdtEncoding_Japanese0,
            RE4_MDT_EDIT.MdtEncoding mdtEncoding_Chinese6,
            RE4_MDT_EDIT.MdtEncoding mdtEncoding_Chinese9,
            FileVersion version, bool isSplittedFiles, bool hasChinese)
        {
            var directory = Path.GetDirectoryName(fileInfo.FullName);
            var baseName = Path.GetFileNameWithoutExtension(fileInfo.Name);

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

            MakeExtractedMulti.MakeFilesMulti(langs, baseName, directory, isSplittedFiles, hasChinese);
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
    }
}
