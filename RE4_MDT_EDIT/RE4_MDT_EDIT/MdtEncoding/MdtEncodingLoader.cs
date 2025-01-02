using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RE4_MDT_EDIT
{
    internal static class MdtEncodingLoader
    {
        public static MdtEncoding ParseFromJson(string jsonContent)
        {
            MdtEncoding encoding = null;
            string InfoTitle = null;
            string InfoAuthor = null;
            string InfoDescription = null;
            char ConfigCmdStartChar = '\0';
            char ConfigCmdEndChar = '\0';

            //CharsetLists:
            Dictionary<ushort, string> CharsetList = new Dictionary<ushort, string>();
            Dictionary<ushort, string> ColorList = new Dictionary<ushort, string>();
            Dictionary<ushort, string> ExtraCharset = new Dictionary<ushort, string>();
            Dictionary<string, ushort> AkaCharset = new Dictionary<string, ushort>();
            // Additional lists
            Dictionary<string, ushort> InvCharsetListCMD = new Dictionary<string, ushort>();
            Dictionary<char, ushort> InvCharsetListChars = new Dictionary<char, ushort>();

            JObject oDoc;
            try
            {
                oDoc = JObject.Parse(jsonContent);
            }
            catch (Exception ex)
            {
                throw new MdtJsonLoadException("The json file is invalid. It is poorly formatted.", ex);
            }

            if (oDoc["Encoding"] == null)
            {
                throw new MdtJsonLoadException("The json file does not contain \"Encoding\" tag.");
            }

            JObject oEncoding = (JObject)oDoc["Encoding"];

            if (oEncoding["Info"] == null)
            {
                throw new MdtJsonLoadException("The json file does not contain \"Info\" tag.");
            }

            JObject oInfo = (JObject)oEncoding["Info"];

            try
            {
                InfoTitle = oInfo["Title"].ToString();
            }
            catch (Exception ex)
            {
                throw new MdtJsonLoadException("Error getting Title field.", ex);
            }

            try
            {
                InfoAuthor = oInfo["Author"].ToString(); //optional
            }
            catch (Exception)
            {
            }

            try
            {
                InfoDescription = oInfo["Description"].ToString(); //optional
            }
            catch (Exception)
            {
            }

            if (oEncoding["Config"] == null)
            {
                throw new MdtJsonLoadException("The json file does not contain \"Config\" tag.");
            }

            JObject oConfig = (JObject)oEncoding["Config"];

            if (oConfig["CmdStartChar"] == null)
            {
                throw new MdtJsonLoadException("The json file does not contain \"CmdStartChar\" tag.");
            }

            if (oConfig["CmdEndChar"] == null)
            {
                throw new MdtJsonLoadException("The json file does not contain \"CmdEndChar\" tag.");
            }

            Func<string, string, char> CmdCharValidation = (string CmdCharValue, string CmdCharName) =>
            {
                if (CmdCharValue.Length == 0)
                {
                    throw new MdtEncodingException($"The content of the \"{CmdCharName}\" tag cannot be empty.");
                }
                else if (CmdCharValue.Length > 1)
                {
                    throw new MdtEncodingException($"The content of the \"{CmdCharName}\" tag cannot have more than one character.");
                }
                char toChar = CmdCharValue[0];
                if (toChar < 32 || toChar == 127 || char.IsControl(toChar))
                {
                    throw new MdtEncodingException($"[{CmdCharName}] The char content is an invalid character: {toChar}");
                }
                else if (char.IsDigit(toChar))
                {
                    throw new MdtEncodingException($"[{CmdCharName}] The char content cannot be a number: {toChar}");
                }
                else if (char.IsWhiteSpace(toChar))
                {
                    throw new MdtEncodingException($"[{CmdCharName}] The char content cannot be a white space: {toChar}");
                }
                char toUpper = CmdCharValue.ToUpperInvariant()[0];
                char toLower = CmdCharValue.ToLowerInvariant()[0];
                if (toUpper != toLower || toChar != toLower)
                {
                    throw new MdtEncodingException($"[{CmdCharName}] The char content cannot vary from upper to lower case: {toChar}");
                }

                return toLower;
            };

            try
            {
                string sConfigCmdStartChar = oConfig["CmdStartChar"].ToString();
                ConfigCmdStartChar = CmdCharValidation(sConfigCmdStartChar, "CmdStartChar");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            try
            {
                string sConfigCmdEndChar = oConfig["CmdEndChar"].ToString();
                ConfigCmdEndChar = CmdCharValidation(sConfigCmdEndChar, "CmdEndChar");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (oEncoding["CharsetList"] == null)
            {
                throw new MdtJsonLoadException("The json file does not contain \"CharsetList\" tag.");
            }

            JToken oCharsetList = oEncoding["CharsetList"];
            JEnumerable<JToken> Children;

            try
            {
                Children = oCharsetList.Children();
            }
            catch (Exception ex)
            {
                throw new MdtJsonLoadException("The content of the tag \"CharsetList\" is invalid.", ex);
            }

            Action<ushort, string, string, bool> valueValidation = (ushort Code, string value, string exceptionName, bool CheckInvCharset) => {
                //value validation
                if (value.Length == 0)
                {
                    throw new MdtEncodingException($"[{exceptionName}] The content of the char cannot be empty: {Code:X4}={value}");
                }
                else if (value.ToLowerInvariant().StartsWith(ConfigCmdStartChar.ToString())) // cmd char
                {
                    string ValueLower = value.ToLowerInvariant();
                    if (ValueLower.EndsWith(ConfigCmdEndChar.ToString()) == false || ValueLower.Length == 1)
                    {
                        throw new MdtEncodingException($"[{exceptionName}] A char sequence must start and end with \"CmdStartChar\" and \"CmdEndChar\" respectively: {Code:X4}={value}");
                    }
                    string _0x = ConfigCmdStartChar + "0x";
                    if (ValueLower.StartsWith(_0x))
                    {
                        throw new MdtEncodingException($"[{exceptionName}] A character sequence cannot begin with {_0x}: {Code:X4}={value}");
                    }
                    string _hx = ConfigCmdStartChar + "hx";
                    if (ValueLower.StartsWith(_hx))
                    {
                        throw new MdtEncodingException($"[{exceptionName}] A character sequence cannot begin with {_hx}: {Code:X4}={value}");
                    }
                    for (int i = 1; i < ValueLower.Length -1; i++)
                    {
                        if (ValueLower[i] == ConfigCmdEndChar)
                        {
                            throw new MdtEncodingException($"[{exceptionName}] A character sequence cannot have the command terminator in the middle of the sequence: {Code:X4}={value}");
                        }
                        else if (value[0] < 32 || value[0] == 127 || char.IsControl(value[0]))
                        {
                            throw new MdtEncodingException($"[{exceptionName}] A character sequence has a char with an invalid character: {Code:X4}={value}");
                        }
                    }

                    if (CheckInvCharset)
                    {
                        if (InvCharsetListCMD.ContainsKey(ValueLower))
                        {
                            throw new MdtEncodingException($"[{exceptionName}] Character command cannot be repeated in \"CharsetList\": {Code:X4}={value}");
                        }

                        InvCharsetListCMD.Add(ValueLower, Code);
                    }
                    else 
                    {
                        if (!InvCharsetListCMD.ContainsKey(ValueLower))
                        {
                            throw new MdtEncodingException($"[{exceptionName}] Character command has to be repeated in \"CharsetList\": {Code:X4}={value}");
                        }
                    }
                  
                }
                else // single char
                {
                    if (value.Length > 1)
                    {
                        throw new MdtEncodingException($"[{exceptionName}] The content of the char cannot have more than one character, unless it is a command character: {Code:X4}={value}");
                    }
                    value = value[0].ToString();
                    if (value[0] < 32 || value[0] == 127 || char.IsControl(value[0]))
                    {
                        throw new MdtEncodingException($"[{exceptionName}] The char content is an invalid character: {Code:X4}={value}");
                    }
                    if (value[0] == ConfigCmdStartChar)
                    {
                        throw new MdtEncodingException($"[{exceptionName}] Character cannot be equal to the character defined in \"CmdStartChar\": {Code:X4}={value}");
                    }

                    if (CheckInvCharset)
                    {
                        if (InvCharsetListChars.ContainsKey(value[0]))
                        {
                            throw new MdtEncodingException($"[{exceptionName}] Character cannot be repeated in \"CharsetList\": {Code:X4}={value}");
                        }

                        InvCharsetListChars.Add(value[0], Code);
                    }
                    else 
                    {
                        if (!InvCharsetListChars.ContainsKey(value[0]))
                        {
                            throw new MdtEncodingException($"[{exceptionName}] Character has to be repeated in \"CharsetList\": {Code:X4}={value}");
                        }
                    }

                }

            };

            foreach (var item in Children)
            {
                if (item is JProperty prop)
                {
                    string name = prop.Name;
                    string value = prop.Value.ToString();
                    ushort Code = ushort.MaxValue;

                    try
                    {
                        Code = ushort.Parse(name, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        throw new MdtEncodingException("[CharsetList] The character code is not a valid hexadecimal number: " + name);
                    }

                    //code validation
                    if (CharsetList.ContainsKey(Code))
                    {
                        throw new MdtEncodingException("Character code cannot be repeated in \"CharsetList\": " + Code.ToString("X4"));
                    }

                    valueValidation(Code, value, "CharsetList", true);
                    CharsetList.Add(Code, value);
                }
                else
                {
                    throw new MdtJsonLoadException("The content inside the \"CharsetList\" tag is invalid: " + item.ToString());
                }
            }


            if (oEncoding["ExtraCharset"] != null) //optional section
            {
                JToken oExtraList = oEncoding["ExtraCharset"];
                JEnumerable<JToken> ExtraChildren;
                try
                {
                    ExtraChildren = oExtraList.Children();
                }
                catch (Exception ex)
                {
                    throw new MdtJsonLoadException("The content of the tag \"ExtraCharset\" is invalid.", ex);
                }

                foreach (var item in ExtraChildren)
                {
                    if (item is JProperty prop)
                    {
                        string name = prop.Name;
                        string value = prop.Value.ToString();
                        ushort Code = ushort.MaxValue;

                        try
                        {
                            Code = ushort.Parse(name, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                            throw new MdtEncodingException("[ExtraCharset] The character code is not a valid hexadecimal number: " + name);
                        }

                        //code validation
                        if (CharsetList.ContainsKey(Code))
                        {
                            throw new MdtEncodingException("[ExtraCharset] Character code cannot be repeated in \"CharsetList\": " + Code.ToString("X4"));
                        }
                        else if (ExtraCharset.ContainsKey(Code))
                        {
                            throw new MdtEncodingException("[ExtraCharset] Character code cannot be repeated in \"ExtraCharset\": " + Code.ToString("X4"));
                        }

                        valueValidation(Code, value, "ExtraCharset", false);
                        ExtraCharset.Add(Code, value);
                    }
                    else
                    {
                        throw new MdtJsonLoadException("The content inside the \"ExtraCharset\" tag is invalid: " + item.ToString());
                    }
                }
            }

            if (oEncoding["ColorList"] != null) //optional section
            {
                JToken oColorList = oEncoding["ColorList"];
                JEnumerable<JToken> ColorChildren;
                try
                {
                    ColorChildren = oColorList.Children();
                }
                catch (Exception ex)
                {
                    throw new MdtJsonLoadException("The content of the tag \"ColorList\" is invalid.", ex);
                }

                foreach (var item in ColorChildren)
                {
                    if (item is JProperty prop)
                    {
                        string name = prop.Name;
                        string value = prop.Value.ToString();
                        ushort Code = ushort.MaxValue;

                        try
                        {
                            Code = ushort.Parse(name, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                            throw new MdtEncodingException("[ColorList] The character code is not a valid hexadecimal number: " + name);
                        }

                        //code validation
                        if (ColorList.ContainsKey(Code))
                        {
                            throw new MdtEncodingException("Character code cannot be repeated in \"ColorList\": " + Code.ToString("X4"));
                        }

                        valueValidation(Code, value, "ColorList", true);
                        ColorList.Add(Code, value);
                    }
                    else
                    {
                        throw new MdtJsonLoadException("The content inside the \"ColorList\" tag is invalid: " + item.ToString());
                    }
                }
            }

            if (oEncoding["AkaCharset"] != null) //optional section
            {
                JToken oAkaCharset = oEncoding["AkaCharset"];
                JEnumerable<JToken> AkaChildren;
                try
                {
                    AkaChildren = oAkaCharset.Children();
                }
                catch (Exception ex)
                {
                    throw new MdtJsonLoadException("The content of the tag \"AkaCharset\" is invalid.", ex);
                }

                foreach (var item in AkaChildren)
                {
                    if (item is JProperty prop)
                    {
                        string sCode =  prop.Value.ToString();
                        string value = prop.Name;
                        ushort Code = ushort.MaxValue;

                        try
                        {
                            Code = ushort.Parse(sCode, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                            throw new MdtEncodingException("[AkaCharset] The character code is not a valid hexadecimal number: " + sCode);
                        }

                        valueValidation(Code, value, "AkaCharset", true);
                        AkaCharset.Add(value, Code);
                    }
                    else
                    {
                        throw new MdtJsonLoadException("The content inside the \"AkaCharset\" tag is invalid: " + item.ToString());
                    }
                }
            }



            encoding = new MdtEncoding();
            encoding.InfoTitle = InfoTitle ?? "";
            encoding.InfoAuthor = InfoAuthor ?? "";
            encoding.InfoDescription = InfoDescription ?? "";
            encoding.CmdStartChar = ConfigCmdStartChar;
            encoding.CmdEndChar = ConfigCmdEndChar;
            encoding.CharsetList = CharsetList;
            encoding.ColorList = ColorList;
            encoding.AkaCharset = AkaCharset;
            encoding.ExtraCharset = ExtraCharset;
            encoding.InvCharsetListCMD = InvCharsetListCMD;
            encoding.InvCharsetListChars = InvCharsetListChars;

            return encoding;
        }



    }
}
