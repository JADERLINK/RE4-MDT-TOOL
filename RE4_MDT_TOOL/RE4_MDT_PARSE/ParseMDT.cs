using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using SimpleEndianBinaryIO;

namespace RE4_MDT_PARSE
{
    internal static class ParseMDT
    {
        public static MonoLang ParseMono(Stream stream, long startOffset, long endOffset, bool is64bits, Endianness endianness = Endianness.LittleEndian) 
        {
            EndianBinaryReader br = new EndianBinaryReader(stream, endianness);
            br.BaseStream.Position = startOffset;

            uint magic = br.ReadUInt32();
            uint amount = br.ReadUInt32();

            List<uint> Offsets = new List<uint>();

            for (int i = 0; i < amount; i++)
            {
                uint offset = br.ReadUInt32();
                if (is64bits)
                {
                    br.ReadUInt32();
                }

                Offsets.Add(offset);
            }

            uint MinOffset = 0;
            uint maxOffset;

            if (amount != 0)
            {
                MinOffset = Offsets[0];
                maxOffset = Offsets[0];
                for (int i = 0; i < Offsets.Count; i++)
                {
                    if (Offsets[i] < MinOffset)
                    {
                        MinOffset = Offsets[i];
                    }

                    if (Offsets[i] > maxOffset)
                    {
                        maxOffset  = Offsets[i];
                    }
                }

                if (maxOffset + startOffset > endOffset)
                {
                    throw new NotSupportedException("Language text outside the allowed space!");
                }

            }

            for (int i = 0; i < Offsets.Count; i++)
            {
                Offsets[i] -= MinOffset;
            }

            uint StartLangBlock = (uint)(startOffset + MinOffset);
            int LenghtLangBlock = (int)(endOffset - StartLangBlock);

            br.BaseStream.Position = StartLangBlock;
            byte[] arr = br.ReadBytes(LenghtLangBlock);
            List<ushort> CharArr = new List<ushort>();
            for (int i = 0; i < arr.Length; i+=2)
            {
                CharArr.Add(EndianBitConverter.ToUInt16(arr, i, endianness));
            }

            if (CharArr.Count != 0)
            {
                while (true)
                {
                    ushort last = CharArr.LastOrDefault();
                    if (last == 0 && CharArr.Count != 0)
                    {
                        CharArr.RemoveAt(CharArr.Count - 1);
                    }
                    else
                    {
                        CharArr.Add(0);
                        break;
                    }
                }
            }

            return new MonoLang(magic, CharArr.ToArray(), Offsets.ToArray());
        }

        public static MultiLang Parse_UHD_NS(Stream stream, bool is64bits, Endianness endianness = Endianness.LittleEndian) 
        {
            EndianBinaryReader br = new EndianBinaryReader(stream, endianness);
            br.BaseStream.Position = 0;

            uint amount = br.ReadUInt32();

            if (amount != 6)
            {
                throw new NotSupportedException("The file magic must be 0x00000006");
            }

            //--------------

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

            //--------------

            MonoLang[] langs = Parse(stream, MasterOffset, is64bits, endianness);
            return new MultiLang(langs);
        }

        public static MultiLang Parse_PS4(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            br.BaseStream.Position = 0;

            uint amount = br.ReadUInt32();

            if (amount != 6)
            {
                throw new NotSupportedException("The file magic must be 0x00000006");
            }

            //--------------

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

            //--------------

            MonoLang[] langs = Parse(stream, MasterOffset, true, Endianness.LittleEndian);
            return new MultiLang(langs);
        }

        private static MonoLang[] Parse(Stream stream, uint[] MasterOffset, bool is64bits, Endianness endianness)
        {
            (uint StartOffset, uint EndOffset)[] FinalOffset = new (uint StartOffset, uint EndOffset)[8];
            var order = MasterOffset.OrderBy(x => x).ToArray();

            for (int i = 0; i < 8; i++)
            {
                FinalOffset[i].StartOffset = MasterOffset[i];
                FinalOffset[i].EndOffset = (uint)stream.Length;
                for (int e = 0; e < order.Length; e++)
                {
                    if (order[e] < FinalOffset[i].EndOffset && order[e] > FinalOffset[i].StartOffset)
                    {
                        FinalOffset[i].EndOffset = order[e];
                    }
                }
            }

            //--------------

            MonoLang[] langs = new MonoLang[8];
            for (int i = 0; i < 8; i++)
            {
                langs[i] = new MonoLang(0, new ushort[0], new uint[0]);

                if (FinalOffset[i].StartOffset != 0)
                {
                    langs[i] = ParseMono(stream, FinalOffset[i].StartOffset, FinalOffset[i].EndOffset, is64bits, endianness);
                }
            }

            return langs;
        }

    }
}
