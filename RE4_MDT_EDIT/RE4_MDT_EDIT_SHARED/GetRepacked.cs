using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RE4_MDT_EDIT_SHARED
{
    internal class GetRepacked
    {
        public static string[] GetLines(string BaseFileName, string DirectoryName, bool IsSplittedFiles) 
        {
            string[] res;

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

        public static uint GetMagicFromIdxMdt(FileInfo fileInfo) 
        {
            StreamReader idx = fileInfo.OpenText();
            uint magic = 0;

            string endLine = "";
            while (endLine != null)
            {
                endLine = idx.ReadLine();
                if (endLine != null)
                {
                    string trim = endLine.ToUpperInvariant().Trim();
                    if (!(trim.StartsWith(":") || trim.StartsWith("#") || trim.StartsWith("/") || trim.StartsWith("\\")))
                    {
                        var split = trim.Split(new char[] { ':' });
                        if (split.Length >= 2)
                        {
                            string key = split[0].Trim();
                            if (key.StartsWith("MAGIC"))
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

                }

            }

            idx.Close();
            return magic;
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
