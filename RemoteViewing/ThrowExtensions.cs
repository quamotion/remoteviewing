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
using System.Collections.Generic;

namespace RemoteViewing
{
    /// <summary>
    /// Provides extension methods for the <see cref="Throw"/> class.
    /// </summary>
    internal static class ThrowExtensions
    {
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if a <paramref name="condition"/> is <see langword="true"/>.
        /// </summary>
        /// <param name="self">
        /// An instance of the <see cref="Throw"/> class.
        /// </param>
        /// <param name="condition">
        /// The condition that should be <see langword="true"/>.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that caused the current exception.
        /// </param>
        /// <returns>
        /// The instance of the <see cref="Throw"/> class which passed to this method
        /// as <paramref name="self"/> (can be <see langword="null"/>).
        /// </returns>
        public static Throw True(this Throw self, bool condition, string paramName)
        {
            if (condition)
            {
                throw new ArgumentException(paramName);
            }

            return null;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if a <paramref name="condition"/> is <see langword="false"/>.
        /// </summary>
        /// <param name="self">
        /// An instance of the <see cref="Throw"/> class.
        /// </param>
        /// <param name="condition">
        /// The condition that should be <see langword="false"/>.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that caused the current exception.
        /// </param>
        /// <returns>
        /// The instance of the <see cref="Throw"/> class which passed to this method
        /// as <paramref name="self"/> (can be <see langword="null"/>).
        /// </returns>
        public static Throw False(this Throw self, bool condition, string paramName)
        {
            if (!condition)
            {
                throw new ArgumentException(paramName);
            }

            return null;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if a <paramref name="value"/> is negative.
        /// </summary>
        /// <param name="self">
        /// An instance of the <see cref="Throw"/> class.
        /// </param>
        /// <param name="value">
        /// The value that should be negative.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that caused the current exception.
        /// </param>
        /// <returns>
        /// The instance of the <see cref="Throw"/> class which passed to this method
        /// as <paramref name="self"/> (can be <see langword="null"/>).
        /// </returns>
        public static Throw Negative(this Throw self, int value, string paramName)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("paramName");
            }

            return null;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if a <paramref name="value"/>
        /// is <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value which is being validated.
        /// </typeparam>
        /// <param name="self">
        /// An instance of the <see cref="Throw"/> class.
        /// </param>
        /// <param name="value">
        /// The value which should not be <see langword="null"/>.
        /// </param>
        /// <returns>
        /// The instance of the <see cref="Throw"/> class which passed to this method
        /// as <paramref name="self"/> (can be <see langword="null"/>).
        /// </returns>
        public static Throw Null<T>(this Throw self, T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            return null;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if a <paramref name="value"/>
        /// is <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value which is being validated.
        /// </typeparam>
        /// <param name="self">
        /// An instance of the <see cref="Throw"/> class.
        /// </param>
        /// <param name="value">
        /// The value which should not be <see langword="null"/>.
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter that caused the current exception.
        /// </param>
        /// <returns>
        /// The instance of the <see cref="Throw"/> class which passed to this method
        /// as <paramref name="self"/> (can be <see langword="null"/>).
        /// </returns>
        public static Throw Null<T>(this Throw self, T value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return null;
        }

        /// <summary>
        /// Throws if an <paramref name="offset"/> is out of range for a <paramref name="buffer"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of items stored in the <paramref name="buffer"/>.
        /// </typeparam>
        /// <param name="self">
        /// An instance of the <see cref="Throw"/> class.
        /// </param>
        /// <param name="buffer">
        /// The buffer from which an element is being retrieved.
        /// </param>
        /// <param name="offset">
        /// The offset at which an element is being retrieved.
        /// </param>
        /// <param name="count">
        /// The total number of elements in the <paramref name="buffer"/>.
        /// </param>
        /// <returns>
        /// The instance of the <see cref="Throw"/> class which passed to this method
        /// as <paramref name="self"/> (can be <see langword="null"/>).
        /// </returns>
        public static Throw OutOfRange<T>(this Throw self, IList<T> buffer, int offset, int count)
        {
            Throw.If.Null(buffer, "buffer");
            if (offset < 0 || offset > buffer.Count)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (count < 0 || count > buffer.Count - offset)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            return null;
        }

        /// <summary>
        /// Throws a new <see cref="Vnc.VncException"/> if a <paramref name="condition"/> is <see langword="false"/>.
        /// </summary>
        /// <param name="self">
        /// An instance of the <see cref="Throw"/> class.
        /// </param>
        /// <param name="condition">
        /// The condition which should be <see langword="true"/>.
        /// </param>
        /// <param name="message">
        /// A message that describes the error.
        /// </param>
        /// <param name="reason">
        /// A <see cref="Vnc.VncFailureReason"/> that describes the error.
        /// </param>
        /// <returns>
        /// The instance of the <see cref="Throw"/> class which passed to this method
        /// as <paramref name="self"/> (can be <see langword="null"/>).
        /// </returns>
        public static Throw VncRequires(this Throw self, bool condition, string message, Vnc.VncFailureReason reason)
        {
            if (!condition)
            {
                throw new Vnc.VncException(message, reason);
            }

            return null;
        }
    }
}
