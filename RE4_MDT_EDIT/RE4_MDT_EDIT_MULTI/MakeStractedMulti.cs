using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RE4_MDT_EDIT_MULTI
{
    internal class MakeStractedMulti
    {
        public static void MakeFiles(MonoLangParsed[] langs, string fileName, string directory, bool splitedFiles, bool hasChinese)
        {
            MakeMono(langs[0], fileName + ".0_Japanese", directory, splitedFiles);
            MakeMono(langs[1], fileName + ".1_English", directory, splitedFiles);
            MakeMono(langs[2], fileName + ".2_French", directory, splitedFiles);
            MakeMono(langs[3], fileName + ".3_German", directory, splitedFiles);
            MakeMono(langs[4], fileName + ".4_Italian", directory, splitedFiles);
            MakeMono(langs[5], fileName + ".5_Spanish", directory, splitedFiles);
            if (hasChinese)
            {
                MakeMono(langs[6], fileName + ".6_Chinese_zh_tw", directory, splitedFiles);
                MakeMono(langs[7], fileName + ".9_Chinese_zh_cn", directory, splitedFiles);
            }

            string resDirectory = Path.Combine(directory, fileName);
            var idxInfo = new FileInfo(resDirectory + ".idxmultimdt");
            var sw = idxInfo.CreateText();
            sw.WriteLine("# RE4_MDT_EDIT");
            sw.WriteLine("# by: JADERLINK");
            sw.WriteLine("# youtube.com/@JADERLINK");
            sw.WriteLine("# github.com/JADERLINK");
            sw.WriteLine("# " + MainAction.Version);
            sw.WriteLine("# MdtEncoding.json used: " + langs[1].MdtEncodingInfoTitle);
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

        private static void MakeMono(MonoLangParsed mono, string fileName, string directory, bool splitedFiles) 
        {
            string resDirectory = Path.Combine(directory, fileName);

            if (splitedFiles)
            {
                Directory.CreateDirectory(resDirectory);

                for (int i = 0; i < mono.Lines.Length; i++)
                {
                    string file = Path.Combine(resDirectory, i.ToString("D4") + ".txtmdt");
                    File.WriteAllText(file, mono.Lines[i], Encoding.UTF8);
                }
            }
            else
            {
                File.WriteAllLines(resDirectory + ".txtmdt", mono.Lines, Encoding.UTF8);
            }

            var idxInfo = new FileInfo(resDirectory + ".idxmdt");
            var sw = idxInfo.CreateText();
            sw.WriteLine("# RE4_MDT_EDIT");
            sw.WriteLine("# by: JADERLINK");
            sw.WriteLine("# youtube.com/@JADERLINK");
            sw.WriteLine("# github.com/JADERLINK");
            sw.WriteLine("# " + MainAction.Version);
            sw.WriteLine("# MdtEncoding.json used: " + mono.MdtEncodingInfoTitle);
            sw.WriteLine("MAGIC:" + mono.Magic.ToString("X8"));
            sw.Close();

        }
    }
}
