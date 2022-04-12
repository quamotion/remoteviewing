namespace RemoteViewing.Hosting
{
    /// <summary>
    /// Options that control how a VNC Server is created.
    /// </summary>
    public class VncServerOptions
    {
        /// <summary>
        /// Gets or sets the password used to authenticate the user (if any).
        /// </summary>
        public string Password { get; set; } = null;

        /// <summary>
        /// Gets or sets the address at which the VNC server should listen.
        /// </summary>
        public string Address { get; set; } = "127.0.0.1";

        /// <summary>
        /// Gets or sets the port at which the VNC server should listen.
        /// </summary>
        public int Port { get; set; } = 5900;

        /// <summary>
        /// Gets or sets a value indicating whether to initiate a reverse VNC connection.
        /// </summary>
        public bool Reverse { get; set; } = false;
    }
}
