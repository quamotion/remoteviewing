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
    public class DummyFramebufferSource : IVncFramebufferSource, IVncRemoteKeyboard, IVncRemoteController
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
        public bool SupportsResizing => true;

        // The width and height of the framebuffer source.
        // These values can be changed by the client

        /// <summary>
        /// Gets or sets the width of the framebuffer.
        /// </summary>
        public int Width
        { get; set; } = 200;

        /// <summary>
        /// Gets or sets the height of the framebuffer.
        /// </summary>
        public int Height
        { get; set; } = 400;

        /// <summary>
        /// Gets or sets a string to add to the test message displayed on the screen.
        /// </summary>
        public string TextSuffix
        {
            get;
            set;
        }

        /// <inheritdoc/>
        public ExtendedDesktopSizeStatus SetDesktopSize(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                return ExtendedDesktopSizeStatus.Prohibited;
            }

            this.Width = width;
            this.Height = height;
            return ExtendedDesktopSizeStatus.Success;
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

            using (Bitmap image = new Bitmap(this.Width, this.Height))
            using (Graphics gfx = Graphics.FromImage(image))
            using (SolidBrush brush = new SolidBrush(color))
            using (var font = new Font(FontFamily.GenericSansSerif, 12.0f, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                gfx.FillRectangle(brush, 0, 0, image.Width, image.Height);

                var text = $"RemoteViewing {ThisAssembly.AssemblyInformationalVersion}. {this.TextSuffix}";
                var size = gfx.MeasureString(text, font);

                var position = new PointF((image.Width - size.Width) / 2.0f, (image.Height - size.Height) / 2.0f);

                gfx.DrawString(
                    text,
                    font,
                    Brushes.Black,
                    position);

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

        /// <inheritdoc/>
        public void HandleTouchEvent(object sender, PointerChangedEventArgs e)
        {
            this.TextSuffix = $"Mouse at ({e.X},{e.Y})";
        }

        /// <inheritdoc/>
        public void HandleKeyEvent(object sender, KeyChangedEventArgs e)
        {
            if (e.Keysym == KeySym.Plus && e.Pressed == false)
            {
                this.Width *= 2;
                this.Height *= 2;
            }

            if (e.Keysym == KeySym.Minus && e.Pressed == false)
            {
                this.Width /= 2;
                this.Height /= 2;
            }

            if (e.Keysym == KeySym.r && e.Pressed == false)
            {
                var temp = this.Width;
                this.Width = this.Height;
                this.Height = temp;
            }
        }
    }
}
