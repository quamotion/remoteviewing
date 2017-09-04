#region License
/*
RemoteViewing VNC Client/Server Library for .
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com/software/remoteviewing>
Copyright (c) 2017 Frederik Carlier <http://github.com/qmfrederik/remoteviewing>
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

namespace RemoteViewing.Vnc
{
    /// <summary>
    /// A common interface for classes which implement the VNC password challenge protocol.
    /// </summary>
    public interface IVncPasswordChallenge
    {
        /// <summary>
        /// Generates a 16-byte challenge.
        /// </summary>
        /// <returns>
        /// A <see cref="byte"/> array which contains the 16-byte challenge.
        /// </returns>
        byte[] GenerateChallenge();

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
        void GetChallengeResponse(byte[] challenge, char[] password, byte[] response);

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
        void GetChallengeResponse(byte[] challenge, byte[] password, byte[] response);
    }
}
