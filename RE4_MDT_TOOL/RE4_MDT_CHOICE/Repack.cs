using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using SimpleEndianBinaryIO;
using RE4_MDT_PARSE;
using RE4_MDT_CHOICE_PARSE;

namespace RE4_MDT_CHOICE
{
    internal static class Repack
    {
        public static void ToRepack(FileInfo fileInfo, bool Is64bits, bool IsPS4, Endianness endianness) 
        {
            var directory = Path.GetDirectoryName(fileInfo.FullName);
            var baseName = Path.GetFileNameWithoutExtension(fileInfo.Name);

            string[] FilesNames = IdxChoiceMDT.GetFilesNames(fileInfo);

            HashSet<string> validFiles = new HashSet<string>();
            Dictionary<int, string> langIds = new Dictionary<int, string>();
            for (int i = 0; i < FilesNames.Length; i++)
            {
                string temp = Path.ChangeExtension(FilesNames[i], "mdt");
                FilesNames[i] = null;

                if ( (i < 6 || !(IsPS4 || endianness == Endianness.BigEndian))
                    && (temp != null && temp.Length > 0 && temp != "null" && temp != "null.mdt") )
                {
                    if (File.Exists(Path.Combine(directory, temp)))
                    {
                        FilesNames[i] = temp;
                        if (!validFiles.Contains(temp))
                        {
                            validFiles.Add(temp);
                            langIds.Add(i, temp);
                            Console.WriteLine("Loaded File: " + temp);
                        }
                    }
                    else
                    {
                        Console.WriteLine("File does not exist: " + temp);
                    }
                }
            }

            //Carregar os arquivos
            Dictionary<int, MonoLang> monoLangDic = new Dictionary<int, MonoLang>();
            foreach (var item in langIds)
            {
                Stream stream = null;
                try
                {
                    var info = new FileInfo(Path.Combine(directory, item.Value));
                    stream = info.OpenRead();
                    var monoLang = ParseMDT.ParseMono(stream, 0, info.Length, Is64bits, endianness);
                    if (monoLang.Offset.Length != 0)
                    {
                        monoLangDic.Add(item.Key, monoLang);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally 
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
            }

            //Pega a ordem correta e qual arquivo vai em qual offset
            int[] langOrder = new int[8] {-1, -1, -1, -1, -1, -1, -1, -1 };
            for (int i = 0; i < FilesNames.Length; i++)
            {
                if (FilesNames[i] != null)
                {
                    foreach (var item in langIds)
                    {
                        if (FilesNames[i] == item.Value && monoLangDic.ContainsKey(item.Key))
                        {
                            langOrder[i] = item.Key;
                        }
                    }
                }
            }

            //Verifica se existe arquivo sem texto
            bool hasEmptyFile = false;
            for (int i = 0; i < langOrder.Length; i++)
            {
                if (langOrder[i] < 0 && (i < 6 || !(IsPS4 || endianness == Endianness.BigEndian)))
                {
                    hasEmptyFile = true;
                }
            }

            //Cria o arquivo mdt
            var outputFile = Path.Combine(directory, baseName + ".MDT");
            Stream finalFile = new FileInfo(outputFile).Create();

            if (IsPS4)
            {
                Choice.MakeChoiceMulti_PS4(monoLangDic, langOrder, finalFile, hasEmptyFile);
            }
            else if (endianness == Endianness.BigEndian)
            {
                Choice.MakeChoiceMulti_UHD_BIG(monoLangDic, langOrder, finalFile, endianness, hasEmptyFile);
            }
            else 
            {
                Choice.MakeChoiceMulti_UHD_NS(monoLangDic, langOrder, finalFile, Is64bits, hasEmptyFile);
            }

            finalFile.Close();
        }

    }
}
