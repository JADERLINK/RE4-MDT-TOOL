using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RE4_MDT_PARSE;
using SimpleEndianBinaryIO;

namespace RE4_MDT_SINGLE_MULTI
{
    internal class MakeSingleMulti
    {
        public static void MakeSingleMulti_UHD_NS(MonoLang monoLang, Stream stream, bool is64bits) 
        {
            BinaryWriter bw = new BinaryWriter(stream);
            bw.BaseStream.Position = 0;
            byte[] header = new byte[is64bits ? 0x30 : 0x24];
            header[0] = 0x6;
            bw.Write(header);

            uint offsetToOffset = 4;

            bw.BaseStream.Position = offsetToOffset;
            bw.Write((uint)header.Length);
            bw.Write(is64bits ? new byte[0x10] : new byte[0x0C]); // 0_Japanese
            uint OffsetToSet = is64bits ? 0x40u : 0x30u;
            for (int i = 1; i <= 5; i++) // 1 até 5
            {
                offsetToOffset += 4;
                bw.BaseStream.Position = offsetToOffset;
                bw.Write(OffsetToSet);
            }
            offsetToOffset += 4;
            bw.BaseStream.Position = offsetToOffset;
            bw.Write((uint)header.Length); //6_Chinese_zh_tw
            offsetToOffset += 4;
            bw.BaseStream.Position = offsetToOffset;
            bw.Write((uint)header.Length); //9_Chinese_zh_cn

            bw.BaseStream.Position = OffsetToSet;
            MakeMDT.MakeMono(monoLang, stream, OffsetToSet, is64bits, out _);
        }

        public static void MakeSingleMulti_PS4(MonoLang monoLang, Stream stream) 
        {
            BinaryWriter bw = new BinaryWriter(stream);
            bw.BaseStream.Position = 0;
            byte[] header = new byte[0x40];
            header[0] = 0x6;
            header[4] = 0x30; // 0_Japanese
            bw.Write(header);

            uint offsetToOffset = 8;
            uint OffsetToSet = 0x40;
            for (int i = 1; i <= 5; i++) // apenas 5 idiomas
            {
                bw.BaseStream.Position = offsetToOffset;
                bw.Write(OffsetToSet);
                offsetToOffset += 8;
            }

            bw.BaseStream.Position = OffsetToSet;
            MakeMDT.MakeMono(monoLang, stream, OffsetToSet, true, out _);
        }

        public static void MakeSingleMulti_UHD_BIG(MonoLang monoLang, Stream stream, Endianness endianness) 
        {
            EndianBinaryWriter bw = new EndianBinaryWriter(stream, endianness);
            bw.BaseStream.Position = 0;
            byte[] header = new byte[0x1C];
            EndianBitConverter.GetBytes((uint)0x6, endianness).CopyTo(header, 0);
            bw.Write(header);

            uint offsetToOffset = 4;

            bw.BaseStream.Position = offsetToOffset;
            bw.Write((uint)0x1C);
            bw.Write(new byte[0x14]); // 0_Japanese
            uint OffsetToSet = 0x30u;
            for (int i = 1; i <= 5; i++) // 1 até 5
            {
                offsetToOffset += 4;
                bw.BaseStream.Position = offsetToOffset;
                bw.Write(OffsetToSet);
            }

            bw.BaseStream.Position = OffsetToSet;
            MakeMDT.MakeMono(monoLang, stream, OffsetToSet, false, out _, endianness);
        }
    }
}
