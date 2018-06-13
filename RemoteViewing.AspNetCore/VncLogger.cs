#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2017 Quamotion bvba <http://www.quamotion.mobi/>
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

using Microsoft.Extensions.Logging;
using RemoteViewing.Logging;
using System;
using VncLogLevel = RemoteViewing.Logging.LogLevel;

namespace RemoteViewing.AspNetCore
{
    /// <summary>
    /// A default implementation of the <see cref="ILog"/> interface which wraps around the ASP.NET Core <see cref="ILogger"/> interface.
    /// </summary>
    public class VncLogger : ILog
    {
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncLogger"/> interface.
        /// </summary>
        /// <param name="logger">
        /// The ASP.NET Core logger to use when processing log messages.
        /// </param>
        public VncLogger(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public bool Log(VncLogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
        {
            switch (logLevel)
            {
                case VncLogLevel.Debug:
                    this.logger.LogDebug(messageFunc(), formatParameters);
                    return true;

                case VncLogLevel.Error:
                    this.logger.LogError(messageFunc(), formatParameters);
                    return true;

                case VncLogLevel.Fatal:
                    this.logger.LogCritical(messageFunc(), formatParameters);
                    return true;

                case VncLogLevel.Info:
                    this.logger.LogInformation(messageFunc(), formatParameters);
                    return true;

                case VncLogLevel.Trace:
                    this.logger.LogTrace(messageFunc(), formatParameters);
                    return true;

                case VncLogLevel.Warn:
                    this.logger.LogWarning(messageFunc(), formatParameters);
                    return true;
            }

            return false;
        }
    }
}
