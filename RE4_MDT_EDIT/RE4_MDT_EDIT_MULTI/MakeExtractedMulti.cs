using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RE4_MDT_EDIT_SHARED;

namespace RE4_MDT_EDIT_MULTI
{
    internal class MakeExtractedMulti
    {
        public static void MakeFilesMulti(MonoLangParsed[] langs, string baseFileName, string directory, bool splitedFiles, bool hasChinese)
        {
            MakeExtracted.MakeMonoFiles(langs[0], baseFileName + ".0_Japanese", directory, splitedFiles);
            MakeExtracted.MakeMonoFiles(langs[1], baseFileName + ".1_English", directory, splitedFiles);
            MakeExtracted.MakeMonoFiles(langs[2], baseFileName + ".2_French", directory, splitedFiles);
            MakeExtracted.MakeMonoFiles(langs[3], baseFileName + ".3_German", directory, splitedFiles);
            MakeExtracted.MakeMonoFiles(langs[4], baseFileName + ".4_Italian", directory, splitedFiles);
            MakeExtracted.MakeMonoFiles(langs[5], baseFileName + ".5_Spanish", directory, splitedFiles);
            if (hasChinese)
            {
                MakeExtracted.MakeMonoFiles(langs[6], baseFileName + ".6_Chinese_zh_tw", directory, splitedFiles);
                MakeExtracted.MakeMonoFiles(langs[7], baseFileName + ".9_Chinese_zh_cn", directory, splitedFiles);
            }

            string resDirectory = Path.Combine(directory, baseFileName);
            var idxInfo = new FileInfo(resDirectory + ".idxmultimdt");
            var sw = idxInfo.CreateText();
            sw.WriteLine("# RE4_MDT_EDIT");
            sw.WriteLine("# by: JADERLINK");
            sw.WriteLine("# youtube.com/@JADERLINK");
            sw.WriteLine("# github.com/JADERLINK");
            sw.WriteLine("# " + ProgVersion.Version);
            sw.WriteLine("# MdtEncodingLatin.json used: " + langs[1].MdtEncodingInfoTitle);
            sw.WriteLine("# MdtEncodingJapanese.json used: " + langs[0].MdtEncodingInfoTitle);
            if (hasChinese) 
            {
                sw.WriteLine("# MdtEncodingChinese6.json used: " + langs[6].MdtEncodingInfoTitle);
                sw.WriteLine("# MdtEncodingChinese9.json used: " + langs[7].MdtEncodingInfoTitle);
            }
            sw.WriteLine("MAGIC_0_Japanese:" + langs[0].Magic.ToString("X8"));
            sw.WriteLine("MAGIC_1_English:" + langs[1].Magic.ToString("X8"));
            sw.WriteLine("MAGIC_2_French:" + langs[2].Magic.ToString("X8"));
            sw.WriteLine("MAGIC_3_German:" + langs[3].Magic.ToString("X8"));
            sw.WriteLine("MAGIC_4_Italian:" + langs[4].Magic.ToString("X8"));
            sw.WriteLine("MAGIC_5_Spanish:" + langs[5].Magic.ToString("X8"));
            if (hasChinese)
            {
                sw.WriteLine("MAGIC_6_Chinese_zh_tw:" + langs[6].Magic.ToString("X8"));
                sw.WriteLine("MAGIC_9_Chinese_zh_cn:" + langs[7].Magic.ToString("X8"));
            }
            sw.Close();
        }

    }
}
