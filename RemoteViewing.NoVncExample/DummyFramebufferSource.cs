using RemoteViewing.Vnc;
using RemoteViewing.Windows.Forms;
using System;
using System.Drawing;

namespace RemoteViewing.NoVncExample
{
    /// <summary>
    /// A dummy framebuffer source, which shows a rectangle with changing colors. Used to verify the noVNC middleware is
    /// working correctly.
    /// </summary>
    public class DummyFramebufferSource : IVncFramebufferSource
    {
        private readonly Random random = new Random();

        private byte[] colors = new byte[3];
        private sbyte[] increments = new sbyte[3] { 5, 5, 5 };

        private VncFramebuffer framebuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyFramebufferSource"/> class.
        /// </summary>
        public DummyFramebufferSource()
        {
        }

        /// <inheritdoc/>
        public VncFramebuffer Capture()
        {
            int colorIndex = this.random.Next(this.colors.Length);

            if (this.colors[colorIndex] + this.increments[colorIndex] < byte.MinValue
                || this.colors[colorIndex] + this.increments[colorIndex] > byte.MaxValue)
            {
                this.increments[colorIndex] *= -1;
            }

            this.colors[colorIndex] = (byte)(this.colors[colorIndex] + this.increments[colorIndex]);

            var color = Color.FromArgb(this.colors[0], this.colors[1], this.colors[2]);

            using (Bitmap image = new Bitmap(400, 400))
            using (Graphics gfx = Graphics.FromImage(image))
            using (SolidBrush brush = new SolidBrush(color))
            {
                gfx.FillRectangle(brush, 0, 0, image.Width, image.Height);

                if (this.framebuffer == null
                    || this.framebuffer.Width != image.Width
                    || this.framebuffer.Height != image.Height)
                {
                    this.framebuffer = new VncFramebuffer("Quamotion", image.Width, image.Height, new VncPixelFormat());
                }

                lock (this.framebuffer.SyncRoot)
                {
                    VncBitmap.CopyToFramebuffer(
                        image,
                        new VncRectangle(0, 0, image.Width, image.Height),
                        this.framebuffer,
                        0,
                        0);
                }

                return this.framebuffer;
            }
        }
    }
}
