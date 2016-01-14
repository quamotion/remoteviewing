#region License
/*
RemoteViewing VNC Client Library for .NET
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com>
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
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace RemoteViewing.Vnc
{
    partial class VncClient
    {
        void NegotiateSecurity()
        {
            int count = ReceiveByte();
            if (count == 0)
            {
                string message = ReceiveString().Trim('\0');
                Require(false, message, VncFailureReason.ServerOfferedNoAuthenticationMethods);
            }

            var types = new List<VncSecurityType>();
            for (int i = 0; i < count; i++) { types.Add((VncSecurityType)ReceiveByte()); }

            if (types.Contains(VncSecurityType.None))
            {
                Send(new[] { (byte)VncSecurityType.None });
            }
            else if (types.Contains(VncSecurityType.Vnc))
            {
                if (_options.Password == null)
                {
                    OnPasswordRequired(new PasswordRequiredEventArgs(_options));
                    Require(_options.Password != null, "Password required.",
                            VncFailureReason.PasswordRequired);
                }

                Send(new[] { (byte)VncSecurityType.Vnc });

                var challenge = Receive(16);
                var response = new byte[16];
                var password = _options.Password;

                var key = new byte[8];
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(password);

                try
                {
                    Array.Copy(bytes, 0, key, 0, Math.Min(bytes.Length, key.Length));
                    for (int i = 0; i < key.Length; i++) { key[i] = ReverseBits(key[i]); }

                    using (var des = new DESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB })
                    using (var encryptor = des.CreateEncryptor())
                    {
                        encryptor.TransformBlock(challenge, 0, 16, response, 0);
                        Send(response);
                    }
                }
                finally
                {
                    Array.Clear(bytes, 0, bytes.Length);
                    Array.Clear(key, 0, key.Length);
                    Array.Clear(response, 0, response.Length);
                }
            }
            else
            {
                Require(false, "No supported authentication methods.",
                        VncFailureReason.NoSupportedAuthenticationMethods);
            }

            uint status = VncUtility.DecodeUInt32BE(Receive(4), 0);
            if (status != 0)
            {
                string message = ReceiveString().Trim('\0');
                Require(false, message, VncFailureReason.AuthenticationFailed);
            }
        }

        // See http://www.vidarholen.net/contents/junk/vnc.html.
        static byte ReverseBits(byte @value)
        {
            byte outValue = 0;
            for (int i = 0; i < 8; i++)
            {
                if (0 != (@value & (1 << i))) { outValue |= (byte)(128 >> i); }
            }
            return outValue;
        }
    }
}
