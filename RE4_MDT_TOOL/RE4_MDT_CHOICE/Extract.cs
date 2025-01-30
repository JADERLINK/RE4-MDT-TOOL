using System;
using System.Collections.Generic;
using System.Text;
using SimpleEndianBinaryIO;
using RE4_MDT_PARSE;
using RE4_MDT_CHOICE_PARSE;
using System.IO;

namespace RE4_MDT_CHOICE
{
    internal static class Extract
    {
        public static void ToExtract(FileInfo fileInfo, bool Is64bits, bool IsPS4, Endianness endianness) 
        {
            var directory = Path.GetDirectoryName(fileInfo.FullName);
            var baseName = Path.GetFileNameWithoutExtension(fileInfo.Name);
            var outputFile = Path.Combine(directory, baseName);

            FileStream stream = null;
            MultiLang multiLang = null;
            uint[] offsets = null;

            try
            {
                stream = fileInfo.OpenRead();
                if (IsPS4)
                {
                    multiLang = ParseMDT.Parse_PS4(stream);
                    offsets = Choice.GetOffset_PS4(stream);
                }
                else
                {
                    multiLang = ParseMDT.Parse_UHD_NS(stream, Is64bits, endianness);
                    offsets = Choice.GetOffset_UHD_NS_BIG(stream, endianness);
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

            var (selecteds, filesSelecteds) = Choice.GetSelectedFromOffset(offsets);

            IdxChoiceMDT.MakeIdxChoiceMDT(multiLang, selecteds, baseName, directory, "mdt");

            foreach (var id in filesSelecteds)
            {
                if (multiLang.Langs[id].Offset.Length != 0)
                {
                    string outputFileTable = outputFile + ".Table." + id.ToString("D1") + ".mdt";
                    var outputFileInfo = new FileInfo(outputFileTable);
                    var outStream = outputFileInfo.OpenWrite();
                    MakeMDT.MakeMono(multiLang.Langs[id], outStream, 0, Is64bits, out _, endianness);
                    outStream.Close();
                }
            }

        }

    }
}
