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
using System.Security.Cryptography;

namespace RemoteViewing.Vnc
{
    /// <summary>
    /// Provides helper methods which implement the VNC password challenge protocol.
    /// </summary>
    internal static class VncPasswordChallenge
    {
        /// <summary>
        /// Generates a 16-byte challenge.
        /// </summary>
        /// <returns>
        /// A ,<see cref="byte"/> array which contains the 16-byte challenge.
        /// </returns>
        public static byte[] GenerateChallenge()
        {
            var challenge = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(challenge);
            return challenge;
        }

        /// <summary>
        /// Calculates a response for a password challenge, using a password.
        /// </summary>
        /// <param name="challenge">
        /// The challenge received from the server.
        /// </param>
        /// <param name="password">
        /// The password to encrypt the challenge with.
        /// </param>
        /// <param name="response">
        /// The response to send back to the server.
        /// </param>
        public static void GetChallengeResponse(byte[] challenge, char[] password, byte[] response)
        {
            Throw.If.Null(password, "password");

            var passwordBytes = VncStream.EncodeString(password, 0, password.Length);
            using (new Utility.AutoClear(passwordBytes))
            {
                GetChallengeResponse(challenge, passwordBytes, response);
            }
        }

        /// <summary>
        /// Calculates a response for a password challenge, using a password.
        /// </summary>
        /// <param name="challenge">
        /// The challenge received from the server.
        /// </param>
        /// <param name="password">
        /// The password to encrypt the challenge with.
        /// </param>
        /// <param name="response">
        /// The response to send back to the server.
        /// </param>
        public static void GetChallengeResponse(byte[] challenge, byte[] password, byte[] response)
        {
            Throw.If.Null(challenge, "challenge").Null(password, "password").Null(response, "response");
            Throw.If.False(challenge.Length == 16, "Challenge must be 16 bytes.");
            Throw.If.False(response.Length == 16, "Response must be 16 bytes.");

            var key = new byte[8];
            using (new Utility.AutoClear(key))
            {
                Array.Copy(password, 0, key, 0, Math.Min(password.Length, key.Length));
                for (int i = 0; i < key.Length; i++)
                {
                    key[i] = ReverseBits(key[i]);
                }

                using (var des = new DESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB })
                using (var encryptor = des.CreateEncryptor())
                {
                    encryptor.TransformBlock(challenge, 0, 16, response, 0);
                }
            }
        }

        /// <summary>
        /// Reverses the bits of a <see cref="byte"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="byte"/> for which to reverse the bits.
        /// </param>
        /// <returns>
        /// The <paramref name="value"/> with the bits reversed.
        /// </returns>
        /// <seealso href="http://www.vidarholen.net/contents/junk/vnc.html"/>
        private static byte ReverseBits(byte @value)
        {
            byte outValue = 0;
            for (int i = 0; i < 8; i++)
            {
                if ((@value & (1 << i)) != 0)
                {
                    outValue |= (byte)(128 >> i);
                }
            }

            return outValue;
        }
    }
}
