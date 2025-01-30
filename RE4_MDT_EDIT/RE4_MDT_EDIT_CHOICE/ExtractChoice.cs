using System;
using System.Collections.Generic;
using System.Text;
using SimpleEndianBinaryIO;
using RE4_MDT_PARSE;
using RE4_MDT_CHOICE_PARSE;
using RE4_MDT_EDIT_SHARED;
using System.IO;

namespace RE4_MDT_EDIT_CHOICE
{
    internal static class ExtractChoice
    {
        public static void ToExtract(FileInfo fileInfo, bool Is64bits, bool IsPS4, Endianness endianness, ChoiseMDT choiseMDT, bool isSplittedFiles)
        {
            var directory = Path.GetDirectoryName(fileInfo.FullName);
            var baseName = Path.GetFileNameWithoutExtension(fileInfo.Name);

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

            //---

            var (selecteds, filesSelecteds) = Choice.GetSelectedFromOffset(offsets);

            IdxChoiceMDT.MakeIdxChoiceMDT(multiLang, selecteds, baseName, directory, "idxmdt");

            //---

            foreach (var id in filesSelecteds)
            {
                if (multiLang.Langs[id].Offset.Length != 0)
                {
                    var lines = RE4_MDT_EDIT.Extract.Extract_All(multiLang.Langs[id], choiseMDT.MdtEncodings[choiseMDT.MdtEncodingNames[id]], isSplittedFiles);
                    MonoLangParsed lang = new MonoLangParsed(multiLang.Langs[id].Magic, lines, choiseMDT.MdtEncodings[choiseMDT.MdtEncodingNames[id]].InfoTitle);

                    string outputName = baseName + ".Table." + id.ToString("D1");
                    MakeExtracted.MakeMonoFiles(lang, outputName, directory, isSplittedFiles);

                    Console.WriteLine($"Extracted {lang.Lines.Length} entries in Table.{id:D1};");
                }
            }
        }

    }
}
