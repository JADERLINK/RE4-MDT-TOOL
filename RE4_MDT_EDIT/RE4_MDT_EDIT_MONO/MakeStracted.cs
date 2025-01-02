using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RE4_MDT_EDIT_MONO
{
    internal class MakeStracted
    {
        public static void MakeFiles(uint Magic, string[] lines, string fileName, string directory, string MdtEncodingTitle, bool splitedFiles) 
        {
            string resDirectory = Path.Combine(directory, fileName);

            if (splitedFiles)
            {
                Directory.CreateDirectory(resDirectory);

                for (int i = 0; i < lines.Length; i++)
                {
                    string file = Path.Combine(resDirectory, i.ToString("D4") + ".txtmdt");
                    File.WriteAllText(file, lines[i], Encoding.UTF8);
                }
            }
            else 
            {
                File.WriteAllLines(resDirectory + ".txtmdt", lines, Encoding.UTF8);
            }
            
            var idxInfo = new FileInfo(resDirectory + ".idxmdt");
            var sw = idxInfo.CreateText();
            sw.WriteLine("# RE4_MDT_EDIT");
            sw.WriteLine("# by: JADERLINK");
            sw.WriteLine("# youtube.com/@JADERLINK");
            sw.WriteLine("# github.com/JADERLINK");
            sw.WriteLine("# " + MainAction.Version);
            sw.WriteLine("# MdtEncoding.json used: " + MdtEncodingTitle);
            sw.WriteLine("MAGIC:" + Magic.ToString("X8"));
            sw.Close();
        }
    }
}
