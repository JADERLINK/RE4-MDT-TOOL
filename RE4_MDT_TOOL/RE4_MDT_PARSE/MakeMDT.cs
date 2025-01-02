using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SimpleEndianBinaryIO;

namespace RE4_MDT_PARSE
{
    internal class MakeMDT
    {
        public static void MakeMono(MonoLang monoLang, Stream stream, long startOffset, bool is64bits, out long endOffset, Endianness endianness = Endianness.LittleEndian) 
        {
            EndianBinaryWriter bw = new EndianBinaryWriter(stream, endianness);
            bw.BaseStream.Position = startOffset;

            //calculos
            uint OffsetSize = is64bits ? 8u : 4u;
            uint Start = (uint)(8 + (monoLang.Offset.Length * OffsetSize));

            uint fullLength = (uint)(Start + (monoLang.CharArr.Length * 2));
            uint rest = fullLength % 16;
            uint div = fullLength / 16;
            div += rest != 0 ? 1u : 0u;
            fullLength = div * 16;

            //arruma offset
            uint[] finalOffsets = (uint[])monoLang.Offset.Clone();
            for (int i = 0; i < finalOffsets.Length; i++)
            {
                finalOffsets[i] += Start;
            }

            //conteudo final
            byte[] final = new byte[fullLength];
            EndianBinaryWriter fbw = new EndianBinaryWriter(new MemoryStream(final), endianness);
            fbw.BaseStream.Position = 0;
            fbw.Write((uint)monoLang.Magic);
            fbw.Write((uint)finalOffsets.Length);

            for (int i = 0; i < finalOffsets.Length; i++)
            {
                fbw.Write((uint)finalOffsets[i]);
                if (is64bits)
                {
                    fbw.Write((uint)0);
                }
            }

            for (int i = 0; i < monoLang.CharArr.Length; i++)
            {
                fbw.Write((ushort)monoLang.CharArr[i]);
            }

            fbw.Close();
            bw.Write(final);
            endOffset = bw.BaseStream.Position;
        }

        public static void Make_UHD_NS(MultiLang multiLang, Stream stream, bool is64bits, Endianness endianness = Endianness.LittleEndian) 
        {
            EndianBinaryWriter bw = new EndianBinaryWriter(stream, endianness);
            bw.BaseStream.Position = 0;
            byte[] header = new byte[is64bits ? 0x30 : 0x24];
            EndianBitConverter.GetBytes((uint)0x6, endianness).CopyTo(header, 0);
            bw.Write(header);

            uint offsetToOffset = 4;
            uint OffsetToSet = (uint)header.Length;
            long endOffset = 0;

            for (int i = 0; i < 8; i++)
            {
                bw.BaseStream.Position = offsetToOffset;
                bw.Write(OffsetToSet);

                if (multiLang.Langs.Length > i)
                {
                    MakeMono(multiLang.Langs[i], stream, OffsetToSet, is64bits, out endOffset, endianness);
                }

                offsetToOffset += 4;
                OffsetToSet = (uint)endOffset;
            }

            //alinhamento
            bw.BaseStream.Position = bw.BaseStream.Length;
            long fileLength = bw.BaseStream.Position;
            long parts = (fileLength / 16);
            long rest = (fileLength % 16);
            parts += rest != 0 ? 1 : 0;
            long dif = (parts * 16) - fileLength;
            byte[] padding = new byte[dif];
            bw.Write(padding);
        }

        public static void Make_PS4(MultiLang multiLang, Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);
            bw.BaseStream.Position = 0;
            byte[] header = new byte[0x30];
            header[0] = 0x6;
            header[4] = 0x30;
            bw.Write(header);

            uint offsetToOffset = 4;
            uint OffsetToSet = (uint)header.Length;
            long endOffset = 0;

            for (int i = 0; i < 6; i++) // apenas 6 idiomas
            {
                bw.BaseStream.Position = offsetToOffset;
                bw.Write(OffsetToSet);

                if (multiLang.Langs.Length > i)
                {
                    MakeMono(multiLang.Langs[i], stream, OffsetToSet, true, out endOffset);
                }
                
                OffsetToSet = (uint)endOffset;

                if (i > 0)
                {
                    offsetToOffset += 8;
                }
                else 
                {
                    offsetToOffset += 4;
                } 
            }
        }

        public static void Make_UHD_BIG(MultiLang multiLang, Stream stream, Endianness endianness = Endianness.LittleEndian)
        {
            EndianBinaryWriter bw = new EndianBinaryWriter(stream, endianness);
            bw.BaseStream.Position = 0;
            byte[] header = new byte[0x1C];
            EndianBitConverter.GetBytes((uint)0x6, endianness).CopyTo(header, 0);
            bw.Write(header);

            uint offsetToOffset = 4;
            uint OffsetToSet = (uint)header.Length;
            long endOffset = 0;

            for (int i = 0; i < 6; i++) // apenas 6 idiomas
            {
                bw.BaseStream.Position = offsetToOffset;
                bw.Write(OffsetToSet);

                if (multiLang.Langs.Length > i)
                {
                    MakeMono(multiLang.Langs[i], stream, OffsetToSet, false, out endOffset, endianness);
                }

                offsetToOffset += 4;
                OffsetToSet = (uint)endOffset;
            }

            //alinhamento
            bw.BaseStream.Position = bw.BaseStream.Length;
            long fileLength = bw.BaseStream.Position;
            long parts = (fileLength / 16);
            long rest = (fileLength % 16);
            parts += rest != 0 ? 1 : 0;
            long dif = (parts * 16) - fileLength;
            byte[] padding = new byte[dif];
            bw.Write(padding);
        }

    }
}
