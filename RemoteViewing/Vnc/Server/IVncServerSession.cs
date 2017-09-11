namespace RemoteViewing.Vnc.Server
{
    public interface IVncServerSession
    {
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
