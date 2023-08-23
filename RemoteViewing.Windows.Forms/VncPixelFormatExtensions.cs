using System;
using System.Drawing.Imaging;
using RemoteViewing.Vnc;

namespace RemoteViewing.Windows.Forms
{
    internal static class VncPixelFormatExtensions
    {

        internal static PixelFormat ToSystemDrawingPixelFormat(this VncPixelFormat vncPixelFormat)
        {
            if (vncPixelFormat.Equals(VncPixelFormat.RGB16))
                return PixelFormat.Format16bppRgb565;

            if (vncPixelFormat.Equals(VncPixelFormat.RGB32))
                return PixelFormat.Format32bppRgb;

            throw new NotSupportedException($"PixelFormat not supported: {vncPixelFormat}");
        }
    }
}
