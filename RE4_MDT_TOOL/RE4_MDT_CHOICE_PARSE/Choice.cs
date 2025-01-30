using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SimpleEndianBinaryIO;
using RE4_MDT_PARSE;

namespace RE4_MDT_CHOICE_PARSE
{
    internal static class Choice
    {
        // to extract

        public static uint[] GetOffset_UHD_NS_BIG(Stream stream, Endianness endianness = Endianness.LittleEndian) 
        {
            EndianBinaryReader br = new EndianBinaryReader(stream, endianness);
            br.BaseStream.Position = 0;

            _ = br.ReadUInt32(); //amount unused
            uint[] MasterOffset = new uint[8];

            for (int i = 0; i < 8; i++)
            {
                MasterOffset[i] = br.ReadUInt32();
            }

            uint MinOffset = MasterOffset[0];
            for (int i = 0; i < 8; i++)
            {
                if (MasterOffset[i] < MinOffset && MasterOffset[i] != 0)
                {
                    MinOffset = MasterOffset[i];
                }
            }

            if (MinOffset < 0x24) // without chinese
            {
                MasterOffset[6] = 0;
                MasterOffset[7] = 0;
            }
            return MasterOffset;
        }

        public static uint[] GetOffset_PS4(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            br.BaseStream.Position = 0;

            _ = br.ReadUInt32(); //amount unused
            uint[] MasterOffset = new uint[8];
            MasterOffset[0] = br.ReadUInt32(); //Japanese
            MasterOffset[1] = br.ReadUInt32(); //English
            _ = br.ReadUInt32(); // padding
            MasterOffset[2] = br.ReadUInt32(); //French
            _ = br.ReadUInt32(); // padding
            MasterOffset[3] = br.ReadUInt32(); //German
            _ = br.ReadUInt32(); // padding
            MasterOffset[4] = br.ReadUInt32(); //Italian
            _ = br.ReadUInt32(); // padding
            MasterOffset[5] = br.ReadUInt32(); //Spanish
            _ = br.ReadUInt32(); // padding
            MasterOffset[6] = 0; // it does not have
            MasterOffset[7] = 0; // it does not have
            return MasterOffset;
        }

        public static (int[] selecteds, int[] filesSelecteds) GetSelectedFromOffset(uint[] offsets) 
        {
            // <offset, IDs>
            Dictionary<uint, List<int>> Pairs = new Dictionary<uint, List<int>>();

            for (int i = 0; i < offsets.Length; i++)
            {
                if (offsets[i] != 0 && !Pairs.ContainsKey(offsets[i]))
                {
                    Pairs.Add(offsets[i], new List<int>());
                }
                if (offsets[i] != 0)
                {
                    Pairs[offsets[i]].Add(i);
                }
            }

            int[] selecteds = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
            int[] filesSelecteds = new int[Pairs.Count];
            int index = 0;
            foreach (var item in Pairs)
            {
                filesSelecteds[index] = item.Value[0];
                index++;
                foreach (var it in item.Value)
                {
                    selecteds[it] = item.Value[0];
                }
            }

            return (selecteds, filesSelecteds);
        }

        // to repack

        public static void MakeChoiceMulti_UHD_NS(Dictionary<int, MonoLang> monoLangDic, int[] langOrder, Stream stream, bool is64bits, bool hasEmptyFile)
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

        public static void MakeChoiceMulti_PS4(Dictionary<int, MonoLang> monoLangDic, int[] langOrder, Stream stream, bool hasEmptyFile)
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

        public static void MakeChoiceMulti_UHD_BIG(Dictionary<int, MonoLang> monoLangDic, int[] langOrder, Stream stream, Endianness endianness, bool hasEmptyFile)
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
