using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SimpleEndianBinaryIO;
using RE4_MDT_PARSE;
using RE4_MDT_CHOICE_PARSE;
using RE4_MDT_EDIT_SHARED;

namespace RE4_MDT_EDIT_CHOICE
{
    internal static class RepackChoice
    {
        public static void ToRepack(FileInfo fileInfo, bool Is64bits, bool IsPS4, Endianness endianness, ChoiseMDT choiseMDT, bool isSplittedFiles)
        {
            var directory = Path.GetDirectoryName(fileInfo.FullName);
            var baseName = Path.GetFileNameWithoutExtension(fileInfo.Name);

            string[] FilesNames = IdxChoiceMDT.GetFilesNames(fileInfo);

            HashSet<string> validFiles = new HashSet<string>();
            Dictionary<int, string> langIds = new Dictionary<int, string>();
            for (int i = 0; i < FilesNames.Length; i++)
            {
                string temp = Path.ChangeExtension(FilesNames[i], "idxmdt");
                string txtmdt = Path.ChangeExtension(temp, "txtmdt");
                FilesNames[i] = null;

                if ((i < 6 || !(IsPS4 || endianness == Endianness.BigEndian))
                    && (temp != null && temp.Length > 0 && temp != "null" && temp != "null.idxmdt"))
                {
                    if (File.Exists(Path.Combine(directory, temp))
                        && (isSplittedFiles || File.Exists(Path.Combine(directory, txtmdt))))
                    {
                        FilesNames[i] = temp;
                        if (!validFiles.Contains(temp))
                        {
                            validFiles.Add(temp);
                            langIds.Add(i, temp);
                            Console.WriteLine("Loaded File: " + temp + $"   [Table.{i:D1}]");
                        }
                    }
                    else
                    {
                        if (!File.Exists(Path.Combine(directory, temp)))
                        {
                            Console.WriteLine("File does not exist: " + temp);
                        }
                        if (!File.Exists(Path.Combine(directory, txtmdt)))
                        {
                            Console.WriteLine("File does not exist: " + txtmdt);
                        }

                    }
                }
            }

            //carrega os arquivos.
            Dictionary<int, MonoLang> monoLangDic = new Dictionary<int, MonoLang>();
            foreach (var item in langIds)
            {
                Stream stream = null;
                try
                {
                    FileInfo fileInfoIdxmdt = new FileInfo(Path.Combine(directory, item.Value));
                    var _baseName = Path.GetFileNameWithoutExtension(item.Value);
                    var _lines = GetRepacked.GetLines(_baseName, directory, isSplittedFiles);
                    var _magic = GetRepacked.GetMagicFromIdxMdt(fileInfoIdxmdt);

                    var (charArr, offsetList) = RE4_MDT_EDIT.Repack.Encoder(_lines, choiseMDT.MdtEncodings[choiseMDT.MdtEncodingNames[item.Key]]);

                    monoLangDic.Add(item.Key, new MonoLang(_magic, charArr, offsetList));
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

            //pega o ordem correta e qual arquivo vai em qual offset
            int[] langOrder = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
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

            //verifica se existe arquivo sem texto
            bool hasEmptyFile = false;
            for (int i = 0; i < langOrder.Length; i++)
            {
                if (langOrder[i] < 0 && (i < 6 || !(IsPS4 || endianness == Endianness.BigEndian)))
                {
                    hasEmptyFile = true;
                }
            }

            //cria o arquivo mdt
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

            foreach (var item in monoLangDic)
            {
                Console.WriteLine($"Repackaged {item.Value.Offset.Length} entries in Table.{item.Key:D1};");
            }
        }

    }
}
