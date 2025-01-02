using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RE4_MDT_EDIT_MULTI
{
    internal class GetRepackedMulti
    {
        public static string[] GetLines(string file, bool IsSplittedFiles)
        {
            string[] res;

            FileInfo fileInfo = new FileInfo(file);
            string BaseFileName = Path.GetFileName(fileInfo.Name);
            string DirectoryName = fileInfo.DirectoryName;

            if (IsSplittedFiles)
            {
                string EntryFolder = Path.Combine(DirectoryName, BaseFileName);

                if (!Directory.Exists(EntryFolder))
                {
                    throw new ArgumentException($"The folder {BaseFileName} does not exist.");
                }

                uint iCount = 0; // quantidade de entry
                bool asFile = true;

                while (asFile)
                {
                    string txtpath = Path.Combine(EntryFolder, iCount.ToString("D4") + ".txtmdt");

                    if (File.Exists(txtpath))
                    {
                        iCount++;
                    }
                    else
                    {
                        asFile = false;
                    }
                }

                res = new string[iCount];

                for (int i = 0; i < iCount; i++)
                {
                    string entryPath = Path.Combine(EntryFolder, i.ToString("D4") + ".txtmdt");

                    if (File.Exists(entryPath))
                    {
                        res[i] = File.ReadAllText(entryPath, Encoding.UTF8);
                    }
                    else
                    {
                        res[i] = "";
                    }
                }

            }
            else
            {
                string entryPath = Path.Combine(DirectoryName, BaseFileName + ".txtmdt");
                if (!File.Exists(entryPath))
                {
                    throw new ArgumentException("The file does not exist: " + BaseFileName + ".txtmdt");
                }
                res = File.ReadAllLines(entryPath, Encoding.UTF8);
            }

            return res;
        }

        public static uint[] GetMagic(string file)
        {
            FileInfo fileInfo = new FileInfo(file);
            StreamReader idx = fileInfo.OpenText();
            uint[] magic =  new uint[8];

            string endLine = "";
            while (endLine != null)
            {
                endLine = idx.ReadLine();
                if (endLine != null)
                {
                    string trim = endLine.ToUpperInvariant().Trim();
                    if (!(trim.StartsWith(":") || trim.StartsWith("#") || trim.StartsWith("/") || trim.StartsWith("\\")))
                    {
                        GetMagic("MAGIC_0_JAPANESE", trim, ref magic[0]);
                        GetMagic("MAGIC_1_ENGLISH", trim, ref magic[1]);
                        GetMagic("MAGIC_2_FRENCH", trim, ref magic[2]);
                        GetMagic("MAGIC_3_GERMAN", trim, ref magic[3]);
                        GetMagic("MAGIC_4_ITALIAN", trim, ref magic[4]);
                        GetMagic("MAGIC_5_SPANISH", trim, ref magic[5]);
                        GetMagic("MAGIC_6_CHINESE_ZH_TW", trim, ref magic[6]);
                        GetMagic("MAGIC_9_CHINESE_ZH_CN", trim, ref magic[7]);
                    }

                }

            }

            idx.Close();
            return magic;
        }

        private static void GetMagic(string keyCheck, string line, ref uint magic) 
        {
            var split = line.Split(new char[] { ':' });
            if (split.Length >= 2)
            {
                string key = split[0].Trim();
                if (key.StartsWith(keyCheck))
                {
                    string value = split[1].Trim();
                    try
                    {
                        magic = uint.Parse(ReturnValidHexValue(value), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }


        private static string ReturnValidHexValue(string cont)
        {
            string res = "";
            foreach (var c in cont.ToUpperInvariant())
            {
                if (char.IsDigit(c)
                    || c == 'A'
                    || c == 'B'
                    || c == 'C'
                    || c == 'D'
                    || c == 'E'
                    || c == 'F'
                    )
                {
                    res += c;
                }
            }
            return res;
        }




    }
}
