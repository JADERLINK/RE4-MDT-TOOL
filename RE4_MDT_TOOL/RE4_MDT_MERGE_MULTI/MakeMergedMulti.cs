using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RE4_MDT_PARSE;
using SimpleEndianBinaryIO;

namespace RE4_MDT_MERGE_MULTI
{
    internal class MakeMergedMulti
    {
        public static (MultiLang multiLang, string outputFullName) MakeMergedMulti_All(string entryFileName, bool is64bits, bool hasChinese, Endianness endianness = Endianness.LittleEndian) 
        {
            string[] langNames = new string[8];
            langNames[0] = "0_Japanese";
            langNames[1] = "1_English";
            langNames[2] = "2_French";
            langNames[3] = "3_German";
            langNames[4] = "4_Italian";
            langNames[5] = "5_Spanish";
            langNames[6] = "6_Chinese_zh_tw";
            langNames[7] = "9_Chinese_zh_cn";

            var fileInfo = new FileInfo(entryFileName.ToLowerInvariant());
            var directory = Path.GetDirectoryName(fileInfo.FullName);
            var baseName = Path.GetFileNameWithoutExtension(fileInfo.Name);
            for (int i = 0; i < langNames.Length; i++)
            {
                baseName = baseName.Replace("." + langNames[i].ToLowerInvariant(), "");
            }
            var outputFullName = Path.Combine(directory, baseName + ".merged.MDT");

            string[] langFiles = new string[8];
            for (int i = 0; i < langFiles.Length; i++)
            {
                langFiles[i] = Path.Combine(directory, baseName + "." + langNames[i] + ".MDT");
            }

            MonoLang[] langs = new MonoLang[8];

            for (int i = 0; i < langs.Length; i++)
            {
                if (i < 6 || hasChinese)
                {
                    var inf = new FileInfo(langFiles[i]);
                    if (File.Exists(langFiles[i]))
                    {
                        Console.WriteLine(inf.Name + " Exists.");
                        var stream = inf.OpenRead();
                        langs[i] = ParseMDT.ParseMono(stream, 0, stream.Length, is64bits, endianness);
                        stream.Close();
                    }
                    else
                    {
                        Console.WriteLine(inf.Name + " Do Not Exists.");
                        langs[i] = new MonoLang(0, new ushort[0], new uint[0]);
                    }
                }
                else
                {
                    langs[i] = new MonoLang(0, new ushort[0], new uint[0]);
                }
            }

            MultiLang multiLang = new MultiLang(langs);

            return (multiLang, outputFullName);
        }
    }
}
