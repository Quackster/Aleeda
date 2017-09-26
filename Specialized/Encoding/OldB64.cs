using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Aleeda
{
    public class OldB64
    {
        /// <summary>
        /// Encodes an integer to a Base64 string.
        /// </summary>
        /// <param name="i">The integer to encode.</param>
        public static string Encode(int i)
        {
            try
            {
                string s = "";
                for (int x = 1; x <= 2; x++)
                    s += (char)((byte)(64 + (i >> 6 * (2 - x) & 0x3f)));

                return s;
            }
            catch { return ""; }
        }
        /// <summary>
        /// Decodes a Base64 string to to an integer.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        public static int Decode(string s)
        {
            char[] val = s.ToCharArray();
            try
            {
                int intTot = 0;
                int y = 0;
                for (int x = (val.Length - 1); x >= 0; x--)
                {
                    int intTmp = (int)(byte)((val[x] - 64));
                    if (y > 0)
                        intTmp = intTmp * (int)(Math.Pow(64, y));
                    intTot += intTmp;
                    y++;
                }
                return intTot;
            }
            catch { return 0; }
        }
        /// <summary>
        /// Returns the first parameter of a string with a Base64 header.
        /// </summary>
        /// <param name="messageContent">The content to get the parameter off.</param>
        public static string getFirstParameter(string messageContent)
        {
            try { return messageContent.Substring(2, Decode(messageContent.Substring(0, 2))); }
            catch { return ""; }
        }
        /// <summary>
        /// Searches for a certain parameter in the message content. If the parameter is not found or any errors occur, then "" is returned.
        /// </summary>
        /// <param name="messageContent">The message content to search the parameter in.</param>
        /// <param name="paramID">The ID of the parameter to search.</param>
        public static string getParameter(string messageContent, int paramID)
        {
            try
            {
                int Cycles = 0;
                float Max = messageContent.Length / 4;
                while (Cycles <= Max)
                {
                    int cycID = Decode(messageContent.Substring(0, 2));
                    int v = Decode(messageContent.Substring(2, 2));
                    if (cycID == paramID)
                        return messageContent.Substring(4, v);

                    messageContent = messageContent.Substring(v + 4);
                    Cycles++;
                }
                return "";
            }
            catch { return ""; }
        }
        /// <summary>
        /// Gets all parameters from a string encoded with Base64 headers, and returns it as a string array.
        /// </summary>
        /// <param name="messageContent">The content to get the parameters off.</param>
        public static string[] getParameters(string messageContent)
        {
            try
            {
                ArrayList res = new ArrayList();
                while (messageContent != "")
                {
                    int v = Decode(messageContent.Substring(0, 2));
                    res.Add(messageContent.Substring(2, v));
                    messageContent = messageContent.Substring(2 + v);
                }
                return (string[])res.ToArray(typeof(string));
            }
            catch { return new string[0]; }
        }
    }
}
