#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com/software/remoteviewing>
Copyright (c) 2017 Frederik Carlier <https://github.com/qmfrederik/remoteviewing>
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

using RemoteViewing.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// A common interface for a VNC server session.
    /// </summary>
    public interface IVncServerSession
    {
        /// <summary>
        /// Occurs when a key has been pressed or released.
        /// </summary>
        event EventHandler<KeyChangedEventArgs> KeyChanged;

        /// <summary>
        /// Occurs on a mouse movement, button click, etc.
        /// </summary>
        event EventHandler<PointerChangedEventArgs> PointerChanged;

        /// <summary>
        /// Occurs when the VNC client has successfully connected to the server.
        /// </summary>
        event EventHandler Connected;

        /// <summary>
        /// Occurs when the VNC client has failed to connect to the server.
        /// </summary>
        event EventHandler ConnectionFailed;

        /// <summary>
        /// Occurs when the VNC client is disconnected.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Occurs when the VNC client provides a password.
        /// Respond to this event by accepting or rejecting the password.
        /// </summary>
        event EventHandler<PasswordProvidedEventArgs> PasswordProvided;

        /// <summary>
        /// Gets or sets the <see cref="ILog"/> logger to use when logging.
        /// </summary>
        ILog Logger { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IVncPasswordChallenge"/> to use when authenticating clients.
        /// </summary>
        IVncPasswordChallenge PasswordChallenge { get; set; }

        /// <summary>
        /// Gets or sets the max rate to send framebuffer updates at, in frames per second.
        /// </summary>
        /// <remarks>
        /// The default is 15.
        /// </remarks>
        double MaxUpdateRate { get; set; }

        /// <summary>
        /// Gets or sets a function which initializes a new <see cref="IVncFramebufferCache"/> for use by
        /// this <see cref="VncServerSession"/>.
        /// </summary>
        Func<VncFramebuffer, ILog, IVncFramebufferCache> CreateFramebufferCache { get; set; }

        /// <summary>
        /// Gets information about the client's most recent framebuffer update request.
        /// This may be <see langword="null"/> if the client has no framebuffer request queued.
        /// </summary>
        FramebufferUpdateRequest FramebufferUpdateRequest { get; }

        /// <summary>
        /// Gets a lock which should be used before performing any framebuffer updates.
        /// </summary>
        object FramebufferUpdateRequestLock { get; }

        /// <summary>
        /// Gets a list of encodings supported by the client.
        /// </summary>
        IReadOnlyList<VncEncoding> ClientEncodings { get; }

        /// <summary>
        /// Starts a session with a VNC client.
        /// </summary>
        /// <param name="stream">The stream containing the connection.</param>
        /// <param name="options">Session options, if any.</param>
        void Connect(Stream stream, VncServerSessionOptions options = null);

        /// <summary>
        /// Sets the framebuffer source.
        /// </summary>
        /// <param name="source">The framebuffer source, or <see langword="null"/> if you intend to handle the framebuffer manually.</param>
        void SetFramebufferSource(IVncFramebufferSource source);

        /// <summary>
        /// Begins a manual framebuffer update.
        /// </summary>
        /// <remarks>
        /// Do not call this method without holding <see cref="IVncServerSession.FramebufferUpdateRequestLock"/>.
        /// </remarks>
        void FramebufferManualBeginUpdate();

        /// <summary>
        /// Queues an update for the specified region.
        /// </summary>
        /// <remarks>
        /// Do not call this method without holding <see cref="IVncServerSession.FramebufferUpdateRequestLock"/>.
        /// </remarks>
        /// <param name="region">The region to invalidate.</param>
        void FramebufferManualInvalidate(VncRectangle region);

        /// <summary>
        /// Queues an update for each of the specified regions.
        /// </summary>
        /// <remarks>
        /// Do not call this method without holding <see cref="IVncServerSession.FramebufferUpdateRequestLock"/>.
        /// </remarks>
        /// <param name="regions">The regions to invalidate.</param>
        void FramebufferManualInvalidate(VncRectangle[] regions);

        /// <summary>
        /// Queues an update for the entire framebuffer.
        /// </summary>
        /// <remarks>
        /// Do not call this method without holding <see cref="VncServerSession.FramebufferUpdateRequestLock"/>.
        /// </remarks>
        void FramebufferManualInvalidateAll();

        /// <summary>
        /// Completes a manual framebuffer update.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the operation completed successfully; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// Do not call this method without holding <see cref="VncServerSession.FramebufferUpdateRequestLock"/>.
        /// </remarks>
        bool FramebufferManualEndUpdate();
    }
}
