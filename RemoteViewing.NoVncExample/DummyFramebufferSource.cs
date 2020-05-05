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

        // Override the color of the framebuffer. When set to transparent, an animation will be displayed
        // instead.
        private Color color = Color.Transparent;

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

            var color = this.color == Color.Transparent ? Color.FromArgb(this.colors[0], this.colors[1], this.colors[2]) : this.color;

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
            // The following keys influence the framebuffer:
            // + doubles the size of the framebuffer
            // - custs the size of the framebuffer in half
            // r rotates the framebuffer 90 degrees
            // R sets the framebuffer to a _red_ screen
            // G sets the framebuffer to a _green_ screen
            // B sets the framebuffer to a _blue_ screen
            // A sets the framebuffer to an _animating_ screen
            //
            // The R, G, B functions can be useful to make sure the individual color channels come through correctly (e.g. not swapping R/B channels)
            // The A function can be useful to set the framerate
            if (e.Keysym == KeySym.Plus && !e.Pressed)
            {
                this.Width *= 2;
                this.Height *= 2;
            }

            if (e.Keysym == KeySym.Minus && !e.Pressed)
            {
                this.Width /= 2;
                this.Height /= 2;
            }

            if (e.Keysym == KeySym.r && !e.Pressed)
            {
                var temp = this.Width;
                this.Width = this.Height;
                this.Height = temp;
            }

            if (e.Keysym == KeySym.R && !e.Pressed)
            {
                this.color = Color.Red;
            }

            if (e.Keysym == KeySym.B && !e.Pressed)
            {
                this.color = Color.Blue;
            }

            if (e.Keysym == KeySym.G && !e.Pressed)
            {
                this.color = Color.Green;
            }

            if (e.Keysym == KeySym.A && !e.Pressed)
            {
                this.color = Color.Transparent;
            }
        }
    }
}
