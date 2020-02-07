#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com/software/remoteviewing>
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RemoteViewing.Vnc
{
    /// <summary>
    /// Provides methods for reading and sending VNC data over a <see cref="Stream"/>.
    /// </summary>
    internal sealed class VncStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VncStream"/> class.
        /// </summary>
        public VncStream()
        {
            this.SyncRoot = new object();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VncStream"/> class.
        /// </summary>
        /// <param name="stream">
        /// The underlying <see cref="Stream"/>.
        /// </param>
        public VncStream(Stream stream)
            : this()
        {
            this.Stream = stream;
        }

        /// <summary>
        /// Gets or sets the underlying <see cref="Stream"/>.
        /// </summary>
        public Stream Stream
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an <see cref="object"/> that can be used to synchronize access to the <see cref="VncStream"/>.
        /// </summary>
        public object SyncRoot
        {
            get;
            private set;
        }

        /// <summary>
        /// Decodes a sequence of bytes from the specified <see cref="byte"/> array into a <see cref="string"/>.
        /// </summary>
        /// <param name="buffer">
        /// The <see cref="byte"/> array containing the sequence of bytes to decode.
        /// </param>
        /// <param name="offset">
        /// The index of the first byte to decode.
        /// </param>
        /// <param name="count">
        /// The number of bytes to decode.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> containing the results of decoding the specified sequence of bytes.
        /// </returns>
        public static string DecodeString(byte[] buffer, int offset, int count)
        {
            return Encoding.GetEncoding("iso-8859-1").GetString(buffer, offset, count);
        }

        /// <summary>
        /// Decodes a sequence of bytes from the specified <see cref="char"/> array into a <see cref="string"/>.
        /// </summary>
        /// <param name="chars">
        /// The <see cref="char"/> array containing the sequence of bytes to decode.
        /// </param>
        /// <param name="offset">
        /// The index of the first byte to decode.
        /// </param>
        /// <param name="count">
        /// The number of bytes to decode.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> containing the results of decoding the specified sequence of bytes.
        /// </returns>
        public static byte[] EncodeString(char[] chars, int offset, int count)
        {
            return Encoding.GetEncoding("iso-8859-1").GetBytes(chars, offset, count);
        }

        /// <summary>
        /// Encodes all the characters in the specified <see cref="string"/> into a sequence of bytes.
        /// </summary>
        /// <param name="string">
        /// The <see cref="string"/> to encode.
        /// </param>
        /// <returns>
        /// A <see cref="byte"/> array containing the results of encoding the specified set of characters.
        /// </returns>
        public static byte[] EncodeString(string @string)
        {
            return Encoding.GetEncoding("iso-8859-1").GetBytes(@string);
        }

        /// <summary>
        /// Throws a <see cref="VncException"/> if a specific condition is not met.
        /// </summary>
        /// <param name="condition">
        /// The condition which should be met.
        /// </param>
        /// <param name="message">
        /// A <see cref="string"/> that describes the error.
        /// </param>
        /// <param name="reason">
        /// A <see cref="VncFailureReason"/> that describes the error.
        /// </param>
        public static void Require(bool condition, string message, Vnc.VncFailureReason reason)
        {
            if (!condition)
            {
                throw new Vnc.VncException(message, reason);
            }
        }

        /// <summary>
        /// Throws a <see cref="VncException"/> if a specific condition is not met.
        /// </summary>
        /// <param name="condition">
        /// The condition which should be met.
        /// </param>
        public static void SanityCheck(bool condition)
        {
            Require(condition, "Sanity check failed.", Vnc.VncFailureReason.SanityCheckFailed);
        }

        /// <summary>
        /// Closes the current stream and releases any resources
        /// associated with the current stream.
        /// </summary>
        public void Close()
        {
            var stream = this.Stream;
            if (stream != null)
            {
                stream.Dispose();
            }
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the
        /// stream by the number of bytes read.
        /// </summary>
        /// <param name="count">
        /// The number of bytes to read.
        /// </param>
        /// <returns>
        /// A <see cref="byte"/> array containing the bytes that have been read.
        /// </returns>
        public byte[] Receive(int count)
        {
            var buffer = new byte[count];
            this.Receive(buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the
        /// stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the specified byte array with
        /// the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/>- 1)
        /// replaced by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data
        /// read from the current stream.
        /// </param>
        /// <param name="count">
        /// The maximum number of bytes to be read from the current stream.
        /// </param>
        public void Receive(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count;)
            {
                int bytes = this.Stream.Read(buffer, offset + i, count - i);
                Require(bytes > 0, "Lost connection.", VncFailureReason.NetworkError);
                i += bytes;
            }
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte.
        /// </summary>
        /// <returns>
        /// The unsigned byte cast to an <see cref="int"/>.
        /// </returns>
        public byte ReceiveByte()
        {
            int value = this.Stream.ReadByte();
            Require(value >= 0, "Lost connection.", VncFailureReason.NetworkError);
            return (byte)value;
        }

        /// <summary>
        /// Reads a <see cref="VncRectangle"/> from the stream and advances the position within
        /// the stream by 8 bytes.
        /// </summary>
        /// <returns>
        /// The <see cref="VncRectangle"/> which was read from the stream.
        /// </returns>
        public VncRectangle ReceiveRectangle()
        {
            int x = this.ReceiveUInt16BE();
            int y = this.ReceiveUInt16BE();
            int w = this.ReceiveUInt16BE();
            int h = this.ReceiveUInt16BE();
            return new VncRectangle(x, y, w, h);
        }

        /// <summary>
        /// Reads a <see cref="string"/> from the stream.
        /// </summary>
        /// <param name="maxLength">
        /// The maximum length of the <see cref="string"/> to
        /// read.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> which was read from the stream.
        /// </returns>
        public string ReceiveString(int maxLength = 0xfff)
        {
            var length = (int)this.ReceiveUInt32BE();
            SanityCheck(length >= 0 && length <= maxLength);
            var value = DecodeString(this.Receive(length), 0, length);
            return value;
        }

        /// <summary>
        /// Reads a <see cref="ushort"/> in big-endian encoding from the stream and advances the
        /// position within the stream by two bytes.
        /// </summary>
        /// <returns>
        /// The <see cref="ushort"/> which was read from the stream.
        /// </returns>
        public ushort ReceiveUInt16BE()
        {
            return VncUtility.DecodeUInt16BE(this.Receive(2), 0);
        }

        /// <summary>
        /// Reads a <see cref="uint"/> in big-endian encoding from the stream and advances the
        /// position within the stream by two bytes.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/> which was read from the stream.
        /// </returns>
        public uint ReceiveUInt32BE()
        {
            return VncUtility.DecodeUInt32BE(this.Receive(4), 0);
        }

        /// <summary>
        /// Receives version information from the stream.
        /// </summary>
        /// <returns>
        /// The <see cref="Version"/>.
        /// </returns>
        public Version ReceiveVersion()
        {
            var version = Encoding.ASCII.GetString(this.Receive(12));
            var versionRegex = Regex.Match(
                    version,
                    @"^RFB (?<maj>[0-9]{3})\.(?<min>[0-9]{3})\n",
                    RegexOptions.Singleline | RegexOptions.CultureInvariant);
            Require(
                versionRegex.Success,
                "Not using VNC protocol.",
                VncFailureReason.WrongKindOfServer);

            int major = int.Parse(versionRegex.Groups["maj"].Value);
            int minor = int.Parse(versionRegex.Groups["min"].Value);
            return new Version(major, minor);
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this
        /// stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        /// The bytes to write to the stream.
        /// </param>
        public void Send(byte[] buffer)
        {
            this.Send(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.
        /// </param>
        /// <param name="count">
        /// The number of bytes to be written to the current stream.
        /// </param>
        public void Send(byte[] buffer, int offset, int count)
        {
            if (this.Stream == null)
            {
                return;
            }

            lock (this.SyncRoot)
            {
                var stream = this.Stream;
                if (stream == null)
                {
                    return;
                }

                try
                {
                    stream.Write(buffer, offset, count);
                }
                catch (ObjectDisposedException)
                {
                }
                catch (IOException)
                {
                }
            }
        }

        /// <summary>
        /// Writes a <see cref="byte"/> to the current position in the stream and advances the position within the stream by one byte.
        /// </summary>
        /// <param name="value">
        /// The <see cref="byte"/> to write to the stream.
        /// </param>
        public void SendByte(byte value)
        {
            this.Send(new[] { value });
        }

        /// <summary>
        /// Writes a <see cref="VncRectangle"/> to the current position in the stream and advances the position within the stream by 8 bytes.
        /// </summary>
        /// <param name="region">
        /// The <see cref="VncRectangle"/> to write to the stream.
        /// </param>
        public void SendRectangle(VncRectangle region)
        {
            var buffer = new byte[8];
            VncUtility.EncodeUInt16BE(buffer, 0, (ushort)region.X);
            VncUtility.EncodeUInt16BE(buffer, 2, (ushort)region.Y);
            VncUtility.EncodeUInt16BE(buffer, 4, (ushort)region.Width);
            VncUtility.EncodeUInt16BE(buffer, 6, (ushort)region.Height);
            this.Send(buffer);
        }

        /// <summary>
        /// Writes a <see cref="ushort"/> in big endian encoding to the current position in the stream and advances the position within the stream by two bytes.
        /// </summary>
        /// <param name="value">
        /// The <see cref="ushort"/> to write to the stream.
        /// </param>
        public void SendUInt16BE(ushort value)
        {
            this.Send(VncUtility.EncodeUInt16BE(value));
        }

        /// <summary>
        /// Writes a <see cref="uint"/> in big endian encoding to the current position in the stream and advances the position within the stream by four bytes.
        /// </summary>
        /// <param name="value">
        /// The <see cref="uint"/> to write to the stream.
        /// </param>
        public void SendUInt32BE(uint value)
        {
            this.Send(VncUtility.EncodeUInt32BE(value));
        }

        /// <summary>
        /// Writes a <see cref="string"/> in big endian encoding to the current position in the stream.
        /// </summary>
        /// <param name="string">
        /// The <see cref="string"/> to write to the stream.
        /// </param>
        /// <param name="includeLength">
        /// <see langword="true"/> to write the current length to the stream; otherwise,
        /// <see langword="false"/>.
        /// </param>
        public void SendString(string @string, bool includeLength = false)
        {
            var encodedString = EncodeString(@string);
            using (new Utility.AutoClear(encodedString))
            {
                if (includeLength)
                {
                    this.SendUInt32BE((uint)encodedString.Length);
                }

                this.Send(EncodeString(@string));
            }
        }

        /// <summary>
        /// Writes a <see cref="Version"/> in big endian encoding to the current position in the stream.
        /// </summary>
        /// <param name="version">
        /// The <see cref="Version"/> to write to the stream.
        /// </param>
        public void SendVersion(Version version)
        {
            this.SendString(string.Format("RFB {0:000}.{1:000}\n", version.Major, version.Minor));
        }
    }
}
