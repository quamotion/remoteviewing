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

                case VncLogLevel.Trace;
                    this.logger.LogTrace(messageFunc(), formatParameters);
                    return true;

                case VncLogLevel.Warn;
                    this.logger.LogWarning(messageFunc(), formatParameters);
                    return true;
            }

            return false;
        }
    }
}
