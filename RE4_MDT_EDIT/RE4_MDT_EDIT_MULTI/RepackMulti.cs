using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RE4_MDT_EDIT_SHARED;

namespace RE4_MDT_EDIT_MULTI
{
    internal static class RepackMulti
    {
        public static void ToRepack(FileInfo fileInfo,
            RE4_MDT_EDIT.MdtEncoding mdtEncoding,
            RE4_MDT_EDIT.MdtEncoding mdtEncoding_Japanese0,
            RE4_MDT_EDIT.MdtEncoding mdtEncoding_Chinese6,
            RE4_MDT_EDIT.MdtEncoding mdtEncoding_Chinese9,
            FileVersion version, bool isSplittedFiles, bool hasChinese)
        {
            var directory = Path.GetDirectoryName(fileInfo.FullName);
            var baseName = Path.GetFileNameWithoutExtension(fileInfo.Name);

            uint[] magic = GetRepackedMulti.GetMagicMulti(fileInfo);

            string[] lines0 = GetRepacked.GetLines(baseName + ".0_Japanese", directory, isSplittedFiles);
            string[] lines1 = GetRepacked.GetLines(baseName + ".1_English", directory, isSplittedFiles);
            string[] lines2 = GetRepacked.GetLines(baseName + ".2_French", directory, isSplittedFiles);
            string[] lines3 = GetRepacked.GetLines(baseName + ".3_German", directory, isSplittedFiles);
            string[] lines4 = GetRepacked.GetLines(baseName + ".4_Italian", directory, isSplittedFiles);
            string[] lines5 = GetRepacked.GetLines(baseName + ".5_Spanish", directory, isSplittedFiles);
            string[] lines6 = new string[0];
            string[] lines7 = new string[0];
            if (hasChinese)
            {
                lines6 = GetRepacked.GetLines(baseName + ".6_Chinese_zh_tw", directory, isSplittedFiles);
                lines7 = GetRepacked.GetLines(baseName + ".9_Chinese_zh_cn", directory, isSplittedFiles);
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

            var outputFile = Path.Combine(directory, baseName + ".MDT");
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

    }
}
