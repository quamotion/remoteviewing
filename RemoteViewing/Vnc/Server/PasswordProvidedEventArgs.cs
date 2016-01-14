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
using System.Linq;

namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// Provides data for the <see cref="VncServerSession.PasswordProvided"/> event.
    /// </summary>
    public sealed class PasswordProvidedEventArgs : EventArgs
    {
        byte[] _challenge, _response;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordProvidedEventArgs"/> class.
        /// </summary>
        /// <param name="challenge">The VNC the server sent.</param>
        /// <param name="response">The bytes of the response from the client.</param>
        public PasswordProvidedEventArgs(byte[] challenge, byte[] response)
        {
            Throw.If.Null(challenge, "challenge").Null(response, "response");
            Throw.If.False(challenge.Length == 16, "Challenge must be 16 bytes.");
            Throw.If.False(response.Length == 16, "Response must be 16 bytes.");

            _challenge = challenge; _response = response;
        }

        /// <summary>
        /// Successfully authenticates the client.
        /// </summary>
        /// <returns>Always <c>true</c>.</returns>
        public bool Accept()
        {
            IsAuthenticated = true; return true;
        }

        /// <summary>
        /// Authenticates the client, if the password bytes match.
        /// </summary>
        /// <param name="password">The bytes of the password.</param>
        /// <returns><c>true</c> if authentication succeeded.</returns>
        public bool Accept(byte[] password)
        {
            Throw.If.Null(password, "password");

            var response = new byte[16];
            VncPasswordChallenge.GetChallengeResponse(_challenge, password, response);
            return Test(response);
        }

        /// <summary>
        /// Authenticates the client, if the password characters match.
        /// </summary>
        /// <param name="password">The characters of the password.</param>
        /// <returns><c>true</c> if authentication succeeded.</returns>
        public bool Accept(char[] password)
        {
            Throw.If.Null(password, "password");

            var response = new byte[16];
            VncPasswordChallenge.GetChallengeResponse(_challenge, password, response);
            return Test(response);
        }

        bool Test(byte[] response)
        {
            using (new Utility.AutoClear(response))
            {
                if (!_response.SequenceEqual(response)) { return false; }
                return Accept();
            }
        }

        /// <summary>
        /// <c>true</c> if the client has successfully authenticated.
        /// </summary>
        public bool IsAuthenticated
        {
            get;
            private set;
        }
    }
}
