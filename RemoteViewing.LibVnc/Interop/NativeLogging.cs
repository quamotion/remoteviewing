#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2020 Quamotion bvba
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

#if !NETSTANDARD2_0 && !NET45
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RemoteViewing.LibVnc.Interop
{
    /// <summary>
    /// This class contains the logic required to set a delegate which is called by libvncserver when logging.
    /// libvncserver logging uses a callback with a variable number of arguments, which is not supported by
    /// P/Invoke in .NET Core, we need to pass through a tiny layer of native code which handles the conversion.
    /// </summary>
    public class NativeLogging
    {
        /// <summary>
        /// The name of the libvncserver library.
        /// </summary>
        public const string ServerLibraryName = NativeMethods.LibraryName;

        /// <summary>
        /// The name of the libvnclogger library. This is the compatibility layer.
        /// </summary>
        public const string LoggerLibraryName = @"vnclogger";

        /// <summary>
        /// The calling convention used by the libvncserver and libvnclogger libraries.
        /// </summary>
        public const CallingConvention LibraryCallingConvention = CallingConvention.Cdecl;

        /// <summary>
        /// The address of the <c>rfblog</c> field in the <c>vncserver</c> library.
        /// </summary>
        public static readonly IntPtr RfbLog;

        /// <summary>
        /// The address of the <c>rfbErr</c> field in the <c>vncserver</c> library.
        /// </summary>
        public static readonly IntPtr RfbErr;

        /// <summary>
        /// The address of the <c>vnclogger</c> library.
        /// </summary>
        public static readonly IntPtr LoggerLibrary;

        /// <summary>
        /// The address of the <c>LogMessage</c> function in the <c>vnclogger</c> library.
        /// </summary>
        public static readonly IntPtr LogMessage;

        /// <summary>
        /// The address of the <c>LogError</c> function in the <c>vnclogger</c> library.
        /// </summary>
        public static readonly IntPtr LogError;

        /// <summary>
        /// The address of the <c>logCallback</c> field in the <c>vnclogger</c> library.
        /// </summary>
        public static readonly IntPtr LogCallback;

        /// <summary>
        /// An instance of the delegate which will be used by the <c>vnclogger</c> library.
        /// </summary>
        public static readonly LogCallbackDelegateDefinition LogCallbackDelegate;

        /// <summary>
        /// A pointer to <see cref="RfbLogCallback(int, IntPtr, int)"/>.
        /// </summary>
        public static readonly IntPtr LogCallbackPtr;

        /// <summary>
        /// Initializes static members of the <see cref="NativeLogging"/> class.
        /// </summary>
        static NativeLogging()
        {
            LoggerLibrary = NativeLibraryLoader.ResolveDll(LoggerLibraryName, typeof(NativeLogging).Assembly, null);
            LogMessage = NativeLibrary.GetExport(LoggerLibrary, "LogMessage");
            LogError = NativeLibrary.GetExport(LoggerLibrary, "LogError");
            LogCallback = NativeLibrary.GetExport(LoggerLibrary, "logCallback");

#if !NETSTANDARD2_0
            var vncServer = NativeLibraryLoader.ResolveDll(ServerLibraryName, typeof(LibVncServer).Assembly, null);
#else
            var vncServer = NativeLibrary.Load(ServerLibraryName, typeof(LibVncServer).Assembly, null);
#endif
            RfbLog = NativeLibrary.GetExport(vncServer, "rfbLog");
            RfbErr = NativeLibrary.GetExport(vncServer, "rfbErr");

            if (RfbLog != IntPtr.Zero)
            {
                Marshal.WriteIntPtr(RfbLog, NativeLogging.LogMessage);
            }

            if (RfbErr != IntPtr.Zero)
            {
                Marshal.WriteIntPtr(RfbErr, NativeLogging.LogError);
            }

            LogCallbackDelegate = new LogCallbackDelegateDefinition(RfbLogCallback);
            LogCallbackPtr = Marshal.GetFunctionPointerForDelegate(LogCallbackDelegate);
            Marshal.WriteIntPtr(NativeLogging.LogCallback, LogCallbackPtr);
        }

        /// <summary>
        /// The delegate used by the <c>logCallback</c> field in this <c>vnclogger</c> library.
        /// </summary>
        /// <param name="level">
        /// The level of the log message (0 for messages and 1 for errors).
        /// </param>
        /// <param name="message">
        /// A pointer to a <c>char*</c> which holds the message to log.
        /// </param>
        /// <param name="length">
        /// The length of the message to log.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LogCallbackDelegateDefinition(int level, IntPtr message, int length);

        public static ILogger Logger { get; set; }

        private static void RfbLogCallback(int level, IntPtr message, int length)
        {
            if (Logger == null)
            {
                return;
            }

            // Skip the terminating terminating newline character
            var text = Marshal.PtrToStringAnsi(message, length - 1);

            if (level == 1)
            {
                Logger.LogError(text);
            }
            else
            {
                Logger.LogInformation(text);
            }
        }
    }
}
#endif