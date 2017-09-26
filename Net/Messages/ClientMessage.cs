﻿using System;
using System.Text;

using Aleeda.Specialized.Encoding;

namespace Aleeda.Net.Messages
{
    /// <summary>
    /// Represents a Habbo client > server protocol structured message, providing methods to identify and 'read' the message.
    /// </summary>
    public class ClientMessage : IHabboMessage
    {
                #region Fields
        /// <summary>
        /// The ID of this message as an unsigned 32 bit integer.
        /// </summary>
        private readonly uint fID;
        /// <summary>
        /// The content of this message as a byte array.
        /// </summary>
        private readonly byte[] fContent;
        /// <summary>
        /// The current index in the content array, used when reading the message.
        /// </summary>
        private int fContentCursor;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the ID of this message as an unsigned 32 bit integer.
        /// </summary>
        public uint ID
        {
            get
            {
                return fID;
            }
        }
        /// <summary>
        /// Gets the header of this message, by Base64 encoding the message ID to a 2 byte string.
        /// </summary>
        public string GetHeader()
        {
            return AleedaEnvironment.GetDefaultEncoding().GetString(Base64Encoding.Encodeuint(fID, 2));
        }
        /// <summary>
        /// Gets the length of the content in this message.
        /// </summary>
        public int GetContentLength()
        {
            return fContent.Length;
        }
        /// <summary>
        /// Gets the amount of unread content bytes.
        /// </summary>
        public int GetRemainingContent()
        {
            return (fContent.Length - fContentCursor);
        }

        public int contentLength
        {
            get
            {
                return fContent.Length - fContentCursor;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a ClientMessage object for a given message ID and a given content byte array.
        /// </summary>
        /// <param name="ID">The ID of the message as an unsigned 32 bit integer.</param>
        /// <param name="bzContent">The content as a byte array. If null is supplied, an empty byte array will be created.</param>
        public ClientMessage(uint ID, byte[] bzContent)
        {
            if (bzContent == null)
                bzContent = new byte[0];

            fID = ID;
            fContent = bzContent;
            fContentCursor = 0;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Resets the client message to it's state when it was constructed by resetting the content reader cursor. This allows to re-read read data.
        /// </summary>
        public void Reset()
        {
            fContentCursor = 0;
        }
        /// <summary>
        /// Advances the content cursor by a given amount of bytes.
        /// </summary>
        /// <param name="n">The amount of bytes to 'skip'.</param>
        public void Advance(int n)
        {
            fContentCursor += n;
        }
        /// <summary>
        /// Returns the total content of this message as a string.
        /// </summary>
        /// <returns>String</returns>
        public string GetContentString()
        {
            return Encoding.Default.GetString(fContent);
        }
        /// <summary>
        /// Returns the header and total content of this message as a string.
        /// </summary>
        public string GetFullString()
        {
            return this.GetHeader() + GetContentString();
        }

        /// <summary>
        /// Reads a given amount of bytes from the remaining message content and returns it in a byte array. The reader cursor is incremented during reading.
        /// </summary>
        /// <param name="numBytes">The amount of bytes to read, advance and return. If there is less remaining data than this value, all remaining data will be read.</param>
        /// <returns>byte[]</returns>
        public byte[] ReadBytes(int numBytes)
        {
            if (numBytes > this.GetRemainingContent())
                numBytes = this.GetRemainingContent();

            byte[] bzData = new byte[numBytes];
            for (int x = 0; x < numBytes; x++)
            {
                bzData[x] = fContent[fContentCursor++];
            }

            return bzData;
        }
        /// <summary>
        /// Reads a given amount of bytes from the remaining message content and returns it in a byte array. The reader cursor does not increment during reading.
        /// </summary>
        /// <param name="numBytes">The amount of bytes to read, advance and return. If there is less remaining data than this value, all remaining data will be read.</param>
        /// <returns>byte[]</returns>
        public byte[] ReadBytesFreezeCursor(int numBytes)
        {
            if (numBytes > this.GetRemainingContent())
                numBytes = this.GetRemainingContent();

            byte[] bzData = new byte[numBytes];
            for (int x = 0, y = fContentCursor; x < numBytes; x++, y++)
            {
                bzData[x] = fContent[y];
            }

            return bzData;
        }
        /// <summary>
        /// Reads a length-prefixed (Base64) value from the message and returns it as a byte array.
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] ReadPrefixedValue()
        {
            Int32 Length = Base64Encoding.DecodeInt32(this.ReadBytes(2));
            return this.ReadBytes(Length);
        }

        /// <summary>
        /// Reads a Base64 boolean and returns it. False is returned if there is no remaining content.
        /// </summary>
        /// <returns>Boolean</returns>
        public Boolean PopBase64Boolean()
        {
            return (this.GetRemainingContent() > 0 && fContent[fContentCursor++] == Base64Encoding.POSITIVE);
        }

        public Int32 PopInt32()
        {
            return Base64Encoding.DecodeInt32(this.ReadBytes(2));
        }
        public UInt32 PopUInt32()
        {
            return (UInt32)PopInt32();
        }

        public string PopFixedString()
        {
            return PopFixedString(AleedaEnvironment.GetDefaultEncoding());
        }

        public string PopFixedString(Encoding encoding)
        {
            return encoding.GetString(ReadPrefixedValue()).Replace(Convert.ToChar(1), ' ');
        }
        /// <summary>
        /// Reads a length prefixed string 32 bit integer from the message content and tries to parse it to integer. No exceptions are thrown if parsing fails.
        /// </summary>
        /// <returns>Int32</returns>
        public Int32 PopPrefixedInt32()
        {
            Int32 i;
            String s = PopFixedString(Encoding.UTF8);
            Int32.TryParse(s, out i);

            return i;
        }
        /// <summary>
        /// Reads a length prefixed string 32 bit unsigned integer from the message content and tries to parse it to integer. No exceptions are thrown if parsing fails.
        /// </summary>
        /// <returns>Int32</returns>
        /// <seealso>PopPrefixedInt32</seealso>
        public uint PopPrefixedUInt32()
        {
            return (uint)PopPrefixedInt32();
        }

        /// <summary>
        /// Reads a wire format boolean and returns it. False is returned if there is no remaining content.
        /// </summary>
        /// <returns>Boolean</returns>
        public Boolean PopWiredBoolean()
        {
            return (this.GetRemainingContent() > 0 && fContent[fContentCursor++] == WireEncoding.POSITIVE);
        }
        /// <summary>
        /// Reads the next wire encoded 32 bit integer from the message content and advances the reader cursor.
        /// </summary>
        /// <returns>Int32</returns>
        public Int32 PopWiredInt32()
        {
            if (this.GetRemainingContent() == 0)
                return 0;

            byte[] bzData = ReadBytesFreezeCursor(WireEncoding.MAX_INTEGER_BYTE_AMOUNT);
            Int32 totalBytes = 0;
            Int32 i = WireEncoding.DecodeInt32(bzData, out totalBytes);
            fContentCursor += totalBytes;

            return i;
        }
        /// <summary>
        /// Reads the next wire encoded unsigned 32 bit integer from the message content and advances the reader cursor.
        /// </summary>
        /// <returns>Int32</returns>
        /// <see>PopWiredInt32()</see>
        public uint PopWiredUInt32()
        {
            return (uint)PopWiredInt32();
        }
        public string PopEncodeInt32()
        {
            return wireEncoding.encodeVL64(PopWiredInt32());
        }
        #endregion
    }
}
