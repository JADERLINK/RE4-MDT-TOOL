using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using RE4_MDT_PARSE;

namespace RE4_MDT_EDIT
{
    internal class Extract
    {
        public static string[] Extract_All(MonoLang monoLang, MdtEncoding mdtEncoding, bool useVisualNewLine)
        {
            int amount = monoLang.Offset.Length;
            ushort[][] lines = new ushort[amount][];

            //etapa 1
            for (int i = 0; i < amount; i++)
            {
                int startoffset = (int)monoLang.Offset[i];
                int endOffset = monoLang.CharArr.Length * 2;
                for (int ii = 0; ii < amount; ii++)
                {
                    if (monoLang.Offset[ii] < endOffset && monoLang.Offset[ii] > startoffset)
                    {
                        endOffset = (int)monoLang.Offset[ii];
                    }
                }

                int byteLenght = (endOffset - startoffset);

                var CharArr = monoLang.CharArr.Skip(startoffset / 2).Take(byteLenght / 2).ToList();

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

                lines[i] = CharArr.ToArray();
            }

            //etapa 2
            string[] res = new string[amount];

            for (int i = 0; i < amount; i++)
            {
                StringBuilder sb = new StringBuilder(1024);

                bool NextByteIsColor = false;
                bool NextByteIsSecondParameter = false;

                foreach (var iCode in lines[i])
                {
                    if (NextByteIsColor)
                    {
                        if (mdtEncoding.ColorList.ContainsKey(iCode))
                        {
                            sb.Append(mdtEncoding.ColorList[iCode]);
                        }
                        else 
                        {
                            InsertHexValue(sb, mdtEncoding, iCode);
                        }
                        NextByteIsColor = false;
                    }
                    else if (NextByteIsSecondParameter)
                    {
                        InsertHexValue(sb, mdtEncoding, iCode);
                        NextByteIsSecondParameter = false;
                    }
                    else 
                    {
                        if (mdtEncoding.CharsetList.ContainsKey(iCode))
                        {
                            sb.Append(mdtEncoding.CharsetList[iCode]);
                        }
                        else if (mdtEncoding.ExtraCharset.ContainsKey(iCode))
                        {
                            sb.Append(mdtEncoding.ExtraCharset[iCode]);
                        }
                        else 
                        {
                            InsertHexValue(sb, mdtEncoding, iCode);
                        }

                        if (NextByteIsSecondParameter == false && NextByteIsSecondParameter == false)
                        {

                            if (iCode == 0x06) // color
                            {
                                NextByteIsColor = true;
                            }

                            else if (iCode == 0x02 
                                  || iCode == 0x05 
                                  || iCode == 0x09 
                                  || iCode == 0x0b 
                                  || iCode == 0x0c 
                                  || iCode == 0x0d 
                                  || iCode == 0x0f 
                                  || iCode == 0x11 
                                  || iCode == 0x12)
                            {
                                NextByteIsSecondParameter = true;
                            }

                            else if (useVisualNewLine && iCode == 0x03) //Newline
                            {
                                sb.AppendLine();
                            }

                            else if (useVisualNewLine && iCode == 0x04) //Newpage
                            {
                                sb.AppendLine();
                                sb.AppendLine();
                            }

                        }

                    }

                }

                if (NextByteIsColor || NextByteIsSecondParameter)
                {
                    InsertHexValue(sb, mdtEncoding, 0x01);
                }

                res[i] = sb.ToString();
            }

            return res;
        }

        private static void InsertHexValue(StringBuilder sb, MdtEncoding mdtEncoding, ushort iCode) 
        {
            sb.Append(mdtEncoding.CmdStartChar + "0x" + iCode.ToString("X1") + mdtEncoding.CmdEndChar);
        }
    }
}
