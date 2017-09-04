namespace RemoteViewing.Logging
{
    /// <summary>
    /// The log level.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Logs a trace message.
        /// </summary>
        Trace,

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        Debug,

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        Info,

        /// <summary>
        /// Logs a warning.
        /// </summary>
        Warn,

        /// <summary>
        /// Logs a non-fatal error.
        /// </summary>
        Error,

        /// <summary>
        /// Logs a fatal error.
        /// </summary>
        Fatal
    }
}
