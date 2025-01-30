using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace RE4_MDT_EDIT_CHOICE
{
    internal class ChoiseMDT
    {
        public string[] MdtEncodingNames { get; }

        public Dictionary<string, RE4_MDT_EDIT.MdtEncoding> MdtEncodings { get; }

        public ChoiseMDT(string[] mdtEncodingNames, Dictionary<string, RE4_MDT_EDIT.MdtEncoding> mdtEncodings)
        {
            MdtEncodingNames = mdtEncodingNames;
            MdtEncodings = mdtEncodings;
        }

        public static ChoiseMDT GetChoiseMDT(string jsonContent, bool hasChinese) 
        {
            string[] langNames = new string[8];
            langNames[0] = "0_Japanese";
            langNames[1] = "1_English";
            langNames[2] = "2_French";
            langNames[3] = "3_German";
            langNames[4] = "4_Italian";
            langNames[5] = "5_Spanish";
            langNames[6] = "6_Chinese_zh_tw";
            langNames[7] = "9_Chinese_zh_cn";

            Dictionary<string, RE4_MDT_EDIT.MdtEncoding> MdtEncodings = new Dictionary<string, RE4_MDT_EDIT.MdtEncoding>();
            string[] arr = new string[8] {"","","","","","","",""};

            JObject oDoc;
            try
            {
                oDoc = JObject.Parse(jsonContent);
            }
            catch (Exception ex)
            {
                throw new JsonLoadException("The json file is invalid. It is poorly formatted.", ex);
            }

            if (oDoc["ChoiceEncodingList"] == null)
            {
                throw new JsonLoadException("The json file does not contain \"MdtEncodingList\" tag.");
            }

            JObject oList = (JObject)oDoc["ChoiceEncodingList"];

            for (int i = 0; i < 8; i++)
            {
                if (i < 6 || hasChinese)
                {
                    if (oList[langNames[i]] == null)
                    {
                        throw new JsonLoadException($"The json file does not contain \"{langNames[i]}\" tag.");
                    }

                    try
                    {
                        arr[i] = oList[langNames[i]].ToString().ToLowerInvariant();
                        if (!MdtEncodings.ContainsKey(arr[i]))
                        {
                            MdtEncodings.Add(arr[i], null);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new JsonLoadException($"Error getting {langNames[i]} field.", ex);
                    }
                }
            }

            var keys = MdtEncodings.Keys.ToArray();
            foreach (var key in keys)
            {
                string jsonContentI = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, key), Encoding.UTF8);
                MdtEncodings[key] = RE4_MDT_EDIT.MdtEncodingLoader.ParseFromJson(jsonContentI);
            }

            return new ChoiseMDT(arr, MdtEncodings);
        }

    }
}
