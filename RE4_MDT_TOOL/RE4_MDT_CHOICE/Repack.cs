using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using SimpleEndianBinaryIO;
using RE4_MDT_PARSE;

namespace RE4_MDT_CHOICE
{
    internal static class Repack
    {
        private static string[] GetFilesNames(FileInfo configFileInfo)
        {
            string[] Files = new string[8] {"", "", "", "", "", "", "", "" };

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

        public static void ToRepack(FileInfo fileInfo, bool Is64bits, bool IsPS4, Endianness endianness) 
        {
            var diretory = Path.GetDirectoryName(fileInfo.FullName);
            var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            var outputFile = Path.Combine(diretory, name);

            string[] FilesNames = GetFilesNames(fileInfo);

            HashSet<string> validFiles = new HashSet<string>();
            for (int i = 0; i < FilesNames.Length; i++)
            {
                if (i < 6 || ! (IsPS4 || endianness == Endianness.BigEndian))
                {
                    if (FilesNames[i] != null && FilesNames[i].Length > 0 && FilesNames[i] != "null")
                    {
                        if (File.Exists(Path.Combine(diretory, FilesNames[i])))
                        {
                            if (!validFiles.Contains(FilesNames[i]))
                            {
                                validFiles.Add(FilesNames[i]);
                                Console.WriteLine("Loaded File: " + FilesNames[i]);
                            }
                        }
                        else
                        {
                            FilesNames[i] = null;
                            Console.WriteLine("File does not exist: " + FilesNames[i]);
                        }
                    }
                    else
                    {
                        FilesNames[i] = null;
                    }
                }
                else
                {
                    FilesNames[i] = null;
                }
            }

            //carrega os arquivos.
            string[] langIds = validFiles.ToArray();
            Dictionary<int, MonoLang> monoLangDic = new Dictionary<int, MonoLang>();
            for (int i = 0; i < langIds.Length; i++)
            {
                Stream stream = null;
                try
                {
                    var info = new FileInfo(Path.Combine(diretory, langIds[i]));
                    stream = info.OpenRead();
                    var monoLang = ParseMDT.ParseMono(stream, 0, info.Length, Is64bits, endianness);
                    if (monoLang.Offset.Length != 0)
                    {
                        monoLangDic.Add(i, monoLang);
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

            //pega o ordem correta e qual arquivo vai em qual offset
            int[] langOrder = new int[8] {-1, -1, -1, -1, -1, -1, -1, -1 };
            for (int i = 0; i < FilesNames.Length; i++)
            {
                if (FilesNames[i] != null)
                {
                    for (int j = 0;j < langIds.Length; j++)
                    {
                        if (FilesNames[i] == langIds[j] && monoLangDic.ContainsKey(j))
                        {
                            langOrder[i] = j;
                        }
                    }
                }
            }

            //cria o arquivo mdt
            bool hasEmptyFile = false;
            for (int i = 0; i < langOrder.Length; i++)
            {
                if (langOrder[i] < 0 && (i < 6 || !(IsPS4 || endianness == Endianness.BigEndian)))
                {
                    hasEmptyFile = true;
                }
            }

            Stream finalFile = new FileInfo(outputFile + ".mdt").Create();

            if (IsPS4)
            {
                MakeSingleMulti_PS4(monoLangDic, langOrder, finalFile, hasEmptyFile);
            }
            else if (endianness == Endianness.BigEndian)
            {
                MakeSingleMulti_UHD_BIG(monoLangDic, langOrder, finalFile, endianness, hasEmptyFile);
            }
            else 
            {
                MakeSingleMulti_UHD_NS(monoLangDic, langOrder, finalFile, Is64bits, hasEmptyFile);
            }

            finalFile.Close();
        }


        public static void MakeSingleMulti_UHD_NS(Dictionary<int, MonoLang> monoLangDic, int[] langOrder, Stream stream, bool is64bits, bool hasEmptyFile)
        {
            // monoLangDic ID, Offset
            Dictionary<int, uint> OffsetDic = new Dictionary<int, uint>();

            BinaryWriter bw = new BinaryWriter(stream);
            bw.BaseStream.Position = 0;
            byte[] header = new byte[is64bits ? 0x30 : 0x24];
            header[0] = 0x6;
            bw.Write(header);
            if (hasEmptyFile)
            {
                bw.Write(is64bits ? new byte[0x10] : new byte[0x0C]); //empty file
            }
            else if (is64bits == false)
            {
                bw.Write(new byte[0x0C]);
            }


            foreach (var item in monoLangDic)
            {
                OffsetDic.Add(item.Key, (uint)bw.BaseStream.Position);
                MakeMDT.MakeMono(item.Value, stream, bw.BaseStream.Position, is64bits, out long endOffset);
                bw.BaseStream.Position = endOffset;
            }


            uint emptyOffset = (uint)header.Length;
            uint offsetToOffset = 4;
            for (int i = 0; i < 8; i++)
            {
                bw.BaseStream.Position = offsetToOffset;
                if (OffsetDic.ContainsKey(langOrder[i]))
                {
                    bw.Write(OffsetDic[langOrder[i]]);
                }
                else 
                {
                    bw.Write(emptyOffset);
                }
                offsetToOffset += 4;
            }
            
        }

        public static void MakeSingleMulti_PS4(Dictionary<int, MonoLang> monoLangDic, int[] langOrder, Stream stream, bool hasEmptyFile)
        {
            // monoLangDic ID, Offset
            Dictionary<int, uint> OffsetDic = new Dictionary<int, uint>();

            BinaryWriter bw = new BinaryWriter(stream);
            bw.BaseStream.Position = 0;
            byte[] header = new byte[0x30];
            header[0] = 0x6;
            bw.Write(header);
            if (hasEmptyFile)
            {
                bw.Write(new byte[0x10]); //empty file
            }


            foreach (var item in monoLangDic)
            {
                OffsetDic.Add(item.Key, (uint)bw.BaseStream.Position);
                MakeMDT.MakeMono(item.Value, stream, bw.BaseStream.Position, true, out long endOffset);
                bw.BaseStream.Position = endOffset;
            }


            uint emptyOffset = (uint)header.Length;
            uint offsetToOffset = 4; 
            for (int i = 0; i <= 5; i++)
            {
                bw.BaseStream.Position = offsetToOffset;

                if (OffsetDic.ContainsKey(langOrder[i]))
                {
                    bw.Write(OffsetDic[langOrder[i]]);
                }
                else
                {
                    bw.Write(emptyOffset);
                }

                if (i != 0)
                {
                    offsetToOffset += 8;
                }
                else
                {
                    offsetToOffset += 4;
                }
            }
        }

        public static void MakeSingleMulti_UHD_BIG(Dictionary<int, MonoLang> monoLangDic, int[] langOrder, Stream stream, Endianness endianness, bool hasEmptyFile)
        {
            // monoLangDic ID, Offset
            Dictionary<int, uint> OffsetDic = new Dictionary<int, uint>();

            EndianBinaryWriter bw = new EndianBinaryWriter(stream, endianness);
            bw.BaseStream.Position = 0;
            byte[] header = new byte[0x1C];
            EndianBitConverter.GetBytes((uint)0x6, endianness).CopyTo(header, 0);
            bw.Write(header);
            if (hasEmptyFile)
            {
                bw.Write(new byte[0x14]); //empty file
            }
            else 
            {
                bw.Write(new byte[0x04]);
            }


            foreach (var item in monoLangDic)
            {
                OffsetDic.Add(item.Key, (uint)bw.BaseStream.Position);
                MakeMDT.MakeMono(item.Value, stream, bw.BaseStream.Position, false, out long endOffset, endianness);
                bw.BaseStream.Position = endOffset;
            }


            uint emptyOffset = (uint)header.Length;
            uint offsetToOffset = 4;
            for (int i = 0; i <= 5; i++)
            {
                bw.BaseStream.Position = offsetToOffset;
                if (OffsetDic.ContainsKey(langOrder[i]))
                {
                    bw.Write(OffsetDic[langOrder[i]]);
                }
                else
                {
                    bw.Write(emptyOffset);
                }
                offsetToOffset += 4;
            }

        }
    }
}
