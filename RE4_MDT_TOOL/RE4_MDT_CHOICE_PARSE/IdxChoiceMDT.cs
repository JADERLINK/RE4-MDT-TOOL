using System;
using System.Collections.Generic;
using System.Text;
using RE4_MDT_PARSE;
using System.IO;
using SimpleEndianBinaryIO;

namespace RE4_MDT_CHOICE_PARSE
{
    internal static class IdxChoiceMDT
    {
        public static void MakeIdxChoiceMDT(MultiLang multiLang, int[] selecteds, string baseName, string directory, string format)
        {
            string[] langNames = new string[8];
            langNames[0] = "0_Japanese     ";
            langNames[1] = "1_English      ";
            langNames[2] = "2_French       ";
            langNames[3] = "3_German       ";
            langNames[4] = "4_Italian      ";
            langNames[5] = "5_Spanish      ";
            langNames[6] = "6_Chinese_zh_tw";
            langNames[7] = "9_Chinese_zh_cn";

            var idx = new FileInfo(Path.Combine(directory, baseName + ".idxchoicemdt")).CreateText();
            idx.WriteLine("# RE4_MDT_CHOICE");
            idx.WriteLine("# by: JADERLINK");
            idx.WriteLine("# youtube.com/@JADERLINK");
            idx.WriteLine("# github.com/JADERLINK");
            idx.WriteLine();
            for (int i = 0; i < 8; i++)
            {
                string file = "null";
                if (multiLang.Langs[i].Offset.Length != 0)
                {
                    file = baseName + ".Table." + selecteds[i].ToString("D1") + "." + format;
                }
                idx.WriteLine(langNames[i] + " : " + file);
            }
            idx.Close();
        }

        public static string[] GetFilesNames(FileInfo configFileInfo)
        {
            string[] Files = new string[8] { "", "", "", "", "", "", "", "" };

            var idx = configFileInfo.OpenText();
            while (!idx.EndOfStream)
            {
                string line = idx.ReadLine().Trim().ToLowerInvariant();

                if ((line.Length == 0
                        || line.StartsWith("#")
                        || line.StartsWith("\\")
                        || line.StartsWith("/")
                        || line.StartsWith(":")
                        ))
                {
                    continue;
                }

                var split = line.Split(':');

                if (split.Length < 2)
                {
                    continue;
                }

                string key = split[0].Trim();
                string value = split[1].Trim();

                switch (key)
                {
                    case "0_japanese":
                        Files[0] = value;
                        break;
                    case "1_english":
                        Files[1] = value;
                        break;
                    case "2_french":
                        Files[2] = value;
                        break;
                    case "3_german":
                        Files[3] = value;
                        break;
                    case "4_italian":
                        Files[4] = value;
                        break;
                    case "5_spanish":
                        Files[5] = value;
                        break;
                    case "6_chinese_zh_tw":
                        Files[6] = value;
                        break;
                    case "9_chinese_zh_cn":
                        Files[7] = value;
                        break;
                }

            }
            idx.Close();

            return Files;
        }
    }
}
