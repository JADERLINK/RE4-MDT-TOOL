using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RE4_MDT_PARSE;
using SimpleEndianBinaryIO;

namespace RE4_MDT_SPLIT
{
    internal class MakeMultiToMono
    {
        public static void MakeMultiToMono_All(MultiLang multiLang, bool is64bits, string baseFileName, bool hasChinese, Endianness endianness = Endianness.LittleEndian) 
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

            for (int i = 0; i < multiLang.Langs.Length; i++)
            {
                if (i < 6 || hasChinese)
                {
                    string outputFile = baseFileName + "." + langNames[i] + ".mdt";
                    var outputFileInfo = new FileInfo(outputFile);
                    var outStream = outputFileInfo.OpenWrite();
                    MakeMDT.MakeMono(multiLang.Langs[i], outStream, 0, is64bits, out _, endianness);
                    outStream.Close();
                }
            }
        }

    }
}
