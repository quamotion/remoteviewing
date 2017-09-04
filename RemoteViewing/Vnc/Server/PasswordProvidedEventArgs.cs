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
        private readonly IVncPasswordChallenge passwordChallenge;
        private byte[] challenge;
        private byte[] response;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordProvidedEventArgs"/> class.
        /// </summary>
        /// <param name="challenge">The VNC the server sent.</param>
        /// <param name="response">The bytes of the response from the client.</param>
        /// <param name="passwordChallenge">
        /// A class which implements the <see cref="IVncPasswordChallenge"/> interface, which is able to validate the password.
        /// </param>
        public PasswordProvidedEventArgs(IVncPasswordChallenge passwordChallenge, byte[] challenge, byte[] response)
        {
            if (challenge == null)
            {
                throw new ArgumentNullException(nameof(challenge));
            }

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (challenge.Length != 16)
            {
                throw new ArgumentOutOfRangeException(nameof(challenge), "Challenge must be 16 bytes.");
            }

            if (response.Length != 16)
            {
                throw new ArgumentOutOfRangeException(nameof(response), "Response must be 16 bytes.");
            }

            if (passwordChallenge == null)
            {
                throw new ArgumentNullException(nameof(passwordChallenge));
            }

            this.challenge = challenge;
            this.response = response;
            this.passwordChallenge = passwordChallenge;
        }

        /// <summary>
        /// Gets a value indicating whether the client has successfully authenticated.
        /// </summary>
        /// <value>
        /// <c>true</c> if the client has successfully authenticated.
        /// </value>
        public bool IsAuthenticated
        {
            get;
            private set;
        }

        /// <summary>
        /// Successfully authenticates the client.
        /// </summary>
        /// <returns>Always <c>true</c>.</returns>
        public bool Accept()
        {
            this.IsAuthenticated = true;
            return true;
        }

        /// <summary>
        /// Authenticates the client, if the password bytes match.
        /// </summary>
        /// <param name="password">The bytes of the password.</param>
        /// <returns><c>true</c> if authentication succeeded.</returns>
        public bool Accept(byte[] password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var response = new byte[16];
            this.passwordChallenge.GetChallengeResponse(this.challenge, password, response);
            return this.Test(response);
        }

        /// <summary>
        /// Authenticates the client, if the password characters match.
        /// </summary>
        /// <param name="password">The characters of the password.</param>
        /// <returns><c>true</c> if authentication succeeded.</returns>
        public bool Accept(char[] password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var response = new byte[16];
            this.passwordChallenge.GetChallengeResponse(this.challenge, password, response);
            return this.Test(response);
        }

        private bool Test(byte[] response)
        {
            using (new Utility.AutoClear(response))
            {
                if (!this.response.SequenceEqual(response))
                {
                    return false;
                }

                return this.Accept();
            }
        }
    }
}
