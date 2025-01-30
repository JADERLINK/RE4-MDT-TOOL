using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RE4_MDT_EDIT
{
    internal class Repack
    {
        public static (ushort[] charArr, uint[] offsetList) Encoder(string[] lines, MdtEncoding mdtEncoding) 
        {
            List<ushort> charArrList = new List<ushort>();
            List<uint> offsetList = new List<uint>();

            for (int i = 0; i < lines.Length; i++)
            {
                var charsLine = EncoderLine(lines[i], mdtEncoding, i);

                offsetList.Add((uint)charArrList.Count * 2);
                charArrList.AddRange(charsLine);
            }

            return (charArrList.ToArray(), offsetList.ToArray());
        }

        private static ushort[] EncoderLine(string line, MdtEncoding mdtEncoding, int entryID)
        {
            List<ushort> CharArr = new List<ushort>();
            StringBuilder sbLine = new StringBuilder(line, 1024);

            // Pré processamento Replace
            if (mdtEncoding.Replace.Length != 0)
            {
                for (int r = mdtEncoding.Replace.Length -1; r >= 0; r--)
                {
                    sbLine.Replace(mdtEncoding.Replace[r].outText, mdtEncoding.Replace[r].inText);
                }
            }

            bool InCmd = false;
            StringBuilder cmd = new StringBuilder(1024);

            foreach (var ichar in sbLine.ToString())
            {
                if (ichar == 0x09 || ichar == 0x0D || ichar == 0x0A) // tab and newline
                {
                    continue;
                }

                else if (InCmd)
                {
                    if (ichar == mdtEncoding.CmdEndChar)
                    {
                        cmd.Append(ichar);
                        InCmd = false;

                        string sCMD = cmd.ToString().ToLowerInvariant();
                        string _0x = mdtEncoding.CmdStartChar + "0x";
                        string _hx = mdtEncoding.CmdStartChar + "hx";

                        if (sCMD.StartsWith(_0x) || sCMD.StartsWith(_hx)) // hex value
                        {
                            ushort value = ushort.MaxValue;
                            string sValue = sCMD.Replace(_0x, "").Replace(_hx, "").Replace(mdtEncoding.CmdEndChar.ToString(), "");

                            try
                            {
                                value = ushort.Parse(sValue, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            catch (Exception)
                            {
                                throw new MdtEncodingException("The hexadecimal value in the command is invalid: " + sCMD + ", EntryID: " + entryID.ToString("D4"));
                            }

                            CharArr.Add(value);
                        }
                        else if (mdtEncoding.InvCharsetListCMD.ContainsKey(sCMD))
                        {
                            CharArr.Add(mdtEncoding.InvCharsetListCMD[sCMD]);
                        }
                        else
                        {
                            throw new MdtEncodingException("The command does not exist in MdtEncoding, EntryID: " + entryID.ToString("D4") + ", CMD: " + sCMD);
                        }

                        cmd.Clear();
                    }
                    else
                    {
                        cmd.Append(ichar);
                    }
                }

                else if (ichar == mdtEncoding.CmdStartChar)
                {
                    cmd.Append(ichar);
                    InCmd = true;
                }

                else  // char
                {
                    if (mdtEncoding.InvCharsetListChars.ContainsKey(ichar))
                    {
                        CharArr.Add(mdtEncoding.InvCharsetListChars[ichar]);
                    }
                    else 
                    {
                        throw new MdtEncodingException("The character does not exist in MdtEncoding, EntryID: " + entryID.ToString("D4") + ", Char: " + ichar);
                    }
                }
            }

            if (cmd.Length != 0)
            {
                throw new MdtEncodingException("The last command did not have the command terminator character, EntryID: " + entryID.ToString("D4") + ", CMD: " + cmd.ToString());
            }

            // validação 0x00 e 0x01
            if (CharArr.Count != 0)
            {
                while (true)
                {
                    ushort last = CharArr.LastOrDefault();
                    if ((last == 0 || last == 1) && CharArr.Count != 0)
                    {
                        CharArr.RemoveAt(CharArr.Count - 1);
                    }
                    else
                    {
                        break;
                    }
                }
                while (true)
                {
                    if (CharArr.Count != 0)
                    {
                        ushort first = CharArr[0];
                        if (first == 0)
                        {
                            CharArr.RemoveAt(0);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }

                }
            }

            CharArr.Add(1);
            CharArr.Insert(0, 0);

            return CharArr.ToArray();
        }

    }
}
