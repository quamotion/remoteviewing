namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// Represents the different kinds of messages that can be exchanged between a VNC server and client.
    /// </summary>
    internal enum VncMessageType : byte
    {
        /// <summary>
        /// Sets the format in which pixel values should be sent in FramebufferUpdate messages.
        /// </summary>
        SetPixelFormat = 0,

        /// <summary>
        /// Sets the encoding types in which pixel data can be sent by the server.
        /// </summary>
        SetEncodings = 2,

        /// <summary>
        /// Notifies the server that the client is interested in the area of the framebuffer specified by
        /// x-position, y-position, width and height.
        /// </summary>
        FrameBufferUpdateRequest = 3,

        /// <summary>
        /// A key press or release.
        /// </summary>
        KeyEvent = 4,

        /// <summary>
        /// Indicates either pointer movement or a pointer button press or release.
        /// </summary>
        PointerEvent = 5,

        /// <summary>
        /// The client has new ISO 8859-1 (Latin-1) text in its cut buffer.
        /// </summary>
        ClientCutText = 6,

        /// <summary>
        /// This message informs the server to switch between only sending FramebufferUpdate messages as a
        /// result of a FramebufferUpdateRequest message, or sending FramebufferUpdate messages continuously.
        /// </summary>
        EnableContinuousUpdates = 150,

        /// <summary>
        /// A client supporting the Fence extension sends this to request a synchronisation of the data stream.
        /// </summary>
        ClientFence = 248,

        /// <summary>
        /// A client supporting the xvp extension sends this to request that the server initiate a clean shutdown,
        /// clean reboot or abrupt reset of the system whose framebuffer the client is displaying.
        /// </summary>
        Xvp = 250,

        /// <summary>
        /// Requests a change of desktop size.
        /// </summary>
        SetDesktopSize = 251
    }
}
