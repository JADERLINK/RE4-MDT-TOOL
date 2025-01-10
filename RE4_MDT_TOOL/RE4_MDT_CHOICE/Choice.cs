using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SimpleEndianBinaryIO;
using RE4_MDT_PARSE;

namespace RE4_MDT_CHOICE
{
    internal static class Choice
    {
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

        public static void MakeIdxChoiceMDT(MultiLang multiLang, uint[] offsets, bool is64bits, string baseFileName, Endianness endianness = Endianness.LittleEndian) 
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

            int[] selecteds = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1};
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

            string[] langNames = new string[8];
            langNames[0] = "0_Japanese     ";
            langNames[1] = "1_English      ";
            langNames[2] = "2_French       ";
            langNames[3] = "3_German       ";
            langNames[4] = "4_Italian      ";
            langNames[5] = "5_Spanish      ";
            langNames[6] = "6_Chinese_zh_tw";
            langNames[7] = "9_Chinese_zh_cn";

            var idx = new FileInfo(baseFileName + ".idxchoicemdt").CreateText();
            idx.WriteLine("# RE4_MDT_CHOICE");
            idx.WriteLine("# by: JADERLINK");
            idx.WriteLine("# youtube.com/@JADERLINK");
            idx.WriteLine("# github.com/JADERLINK");
            idx.WriteLine();
            for (int i = 0; i < 8; i++)
            {
                string file = "null";
                if (multiLang.Langs[i].Offset.Length != 0)
                {
                    file = Path.GetFileName(baseFileName) + ".Table." + selecteds[i].ToString("D1") + ".mdt";
                }
                idx.WriteLine(langNames[i] + " : " + file);
            }
            idx.Close();

            foreach (var id in filesSelecteds)
            {
                if (multiLang.Langs[id].Offset.Length != 0)
                {
                    string outputFile = baseFileName + ".Table." + id.ToString("D1") + ".mdt";
                    var outputFileInfo = new FileInfo(outputFile);
                    var outStream = outputFileInfo.OpenWrite();
                    MakeMDT.MakeMono(multiLang.Langs[id], outStream, 0, is64bits, out _, endianness);
                    outStream.Close();
                }
            }


        }



    }
}
