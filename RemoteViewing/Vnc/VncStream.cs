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
    sealed class VncStream
    {
        public VncStream()
        {
            SyncRoot = new object();
        }

        public void Close()
        {
            var stream = Stream;
            if (stream != null) { stream.Close(); }
        }

        public byte[] Receive(int count)
        {
            var buffer = new byte[count];
            Receive(buffer, 0, buffer.Length);
            return buffer;
        }

        public void Receive(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; )
            {
                int bytes = Stream.Read(buffer, offset + i, count - i);
                Require(bytes > 0, "Lost connection.", VncFailureReason.NetworkError);
                i += bytes;
            }
        }

        public byte ReceiveByte()
        {
            int value = Stream.ReadByte();
            Require(value >= 0, "Lost connection.", VncFailureReason.NetworkError);
            return (byte)value;
        }

        public VncRectangle ReceiveRectangle()
        {
            int x = ReceiveUInt16BE();
            int y = ReceiveUInt16BE();
            int w = ReceiveUInt16BE();
            int h = ReceiveUInt16BE();
            return new VncRectangle(x, y, w, h);
        }

        public string ReceiveString(int maxLength = 0xfff)
        {
            var length = (int)ReceiveUInt32BE(); SanityCheck(length >= 0 && length <= maxLength);
            var value = DecodeString(Receive(length), 0, length);
            return value;
        }

        public ushort ReceiveUInt16BE()
        {
            return VncUtility.DecodeUInt16BE(Receive(2), 0);
        }

        public uint ReceiveUInt32BE()
        {
            return VncUtility.DecodeUInt32BE(Receive(4), 0);
        }

        public Version ReceiveVersion()
        {
            var version = Encoding.ASCII.GetString(Receive(12));
            var versionRegex = Regex.Match(version, @"^RFB (?<maj>[0-9]{3})\.(?<min>[0-9]{3})\n",
                               RegexOptions.Singleline | RegexOptions.CultureInvariant);
            Require(versionRegex.Success, "Not using VNC protocol.",
                    VncFailureReason.WrongKindOfServer);

            int major = int.Parse(versionRegex.Groups["maj"].Value);
            int minor = int.Parse(versionRegex.Groups["min"].Value);
            return new Version(major, minor);
        }

        public void Send(byte[] buffer)
        {
            Send(buffer, 0, buffer.Length);
        }

        public void Send(byte[] buffer, int offset, int count)
        {
            if (Stream == null) { return; }

            lock (SyncRoot)
            {
                var stream = Stream;
                if (stream == null) { return; }

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

        public void SendByte(byte value)
        {
            Send(new[] { value });
        }

        public void SendRectangle(VncRectangle region)
        {
            var buffer = new byte[8];
            VncUtility.EncodeUInt16BE(buffer, 0, (ushort)region.X);
            VncUtility.EncodeUInt16BE(buffer, 2, (ushort)region.Y);
            VncUtility.EncodeUInt16BE(buffer, 4, (ushort)region.Width);
            VncUtility.EncodeUInt16BE(buffer, 6, (ushort)region.Height);
            Send(buffer);
        }

        public void SendUInt16BE(ushort value)
        {
            Send(VncUtility.EncodeUInt16BE(value));
        }

        public void SendUInt32BE(uint value)
        {
            Send(VncUtility.EncodeUInt32BE(value));
        }

        public void SendString(string @string, bool includeLength = false)
        {
            var encodedString = EncodeString(@string);
            using (new Utility.AutoClear(encodedString))
            {
                if (includeLength) { SendUInt32BE((uint)encodedString.Length); }
                Send(EncodeString(@string));
            }
        }

        public void SendVersion(Version version)
        {
            SendString(string.Format("RFB {0:000}.{1:000}\n", version.Major, version.Minor));
        }

        public static string DecodeString(byte[] buffer, int offset, int count)
        {
            return Encoding.GetEncoding("iso-8859-1").GetString(buffer, offset, count);
        }

        public static byte[] EncodeString(char[] chars, int offset, int count)
        {
            return Encoding.GetEncoding("iso-8859-1").GetBytes(chars, offset, count);
        }

        public static byte[] EncodeString(string @string)
        {
            return Encoding.GetEncoding("iso-8859-1").GetBytes(@string);
        }

        public static void Require(bool condition, string message, Vnc.VncFailureReason reason)
        {
            if (!condition) { throw new Vnc.VncException(message, reason); }
        }

        public static void SanityCheck(bool condition)
        {
            Require(condition, "Sanity check failed.", Vnc.VncFailureReason.SanityCheckFailed);
        }

        public Stream Stream
        {
            get;
            set;
        }

        public object SyncRoot
        {
            get;
            private set;
        }
    }
}
