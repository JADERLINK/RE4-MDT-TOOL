using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RE4_MDT_EDIT_SHARED
{
    internal class MakeExtracted
    {
        public static void MakeMonoFiles(MonoLangParsed mono, string baseFileName, string directory, bool splitedFiles)
        {
            string resDirectory = Path.Combine(directory, baseFileName);

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
            sw.WriteLine("# " + ProgVersion.Version);
            sw.WriteLine("# MdtEncoding*.json used: " + mono.MdtEncodingInfoTitle);
            sw.WriteLine("MAGIC:" + mono.Magic.ToString("X8"));
            sw.Close();

        }
    }
}
