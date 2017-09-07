#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2017 Quamotion bvba
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

namespace RemoteViewing.Vnc
{
    /// <summary>
    /// Provides helper methods for working with <see cref="KeySym"/> value.
    /// </summary>
    [CLSCompliant(false)]
    public static class KeySymHelpers
    {
        /// <summary>
        /// Determines whether a <see cref="KeySym"/> value represents an ASCII character.
        /// </summary>
        /// <param name="key">
        /// The <see cref="KeySym"/> key for which to determine whether it is an ASCII character.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="key"/> represents an ASCII character;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsAscii(KeySym key)
        {
            return key >= KeySym.Space && key <= KeySym.AsciiTilde;
        }

        /// <summary>
        /// Converts a <see cref="KeySym"/> value to its <see cref="char"/> equivalent.
        /// </summary>
        /// <param name="key">
        /// The <see cref="KeySym"/> value to convert.
        /// </param>
        /// <returns>
        /// The equivalent <see cref="char"/> value.
        /// </returns>
        public static char ToChar(KeySym key)
        {
            if (key >= KeySym.Space && key <= KeySym.AsciiTilde)
            {
                return (char)(byte)key;
            }

            // Unicode values have the 0x1000000 flag set
            if (((uint)key & 0x1000000) == 0x1000000)
            {
                return (char)((ushort)key & 0xFFFF);
            }

            throw new ArgumentOutOfRangeException(nameof(key));
        }

        /// <summary>
        /// Converts a <see cref="char"/> to its equivalent <see cref="KeySym"/> representation.
        /// </summary>
        /// <param name="c">
        /// The chracter to convert.
        /// </param>
        /// <returns>
        /// The equivalent <see cref="KeySym"/> value.
        /// </returns>
        public static KeySym FromChar(char c)
        {
            if (c >= ' ' && c <= '~')
            {
                return KeySym.Space + (int)c - (int)' ';
            }
            else
            {
                return (KeySym)(0x1000000 | (int)c);
            }
        }
    }
}
