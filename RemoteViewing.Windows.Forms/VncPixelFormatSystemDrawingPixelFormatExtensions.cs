using System;
using System.Drawing.Imaging;
using RemoteViewing.Vnc;

namespace RemoteViewing.Windows.Forms
{
    internal static class VncPixelFormatSystemDrawingPixelFormatExtensions
    {

        internal static PixelFormat ToSystemDrawingPixelFormat(this VncPixelFormat vncPixelFormat)
        {
            if (vncPixelFormat.Equals(VncPixelFormat.RGB16))
            {
                return PixelFormat.Format16bppRgb565;
            }

            if (vncPixelFormat.Equals(VncPixelFormat.RGB32))
            {
                return PixelFormat.Format32bppRgb;
            }

            throw new NotSupportedException($"PixelFormat not supported: {vncPixelFormat}");
        }

        internal static VncPixelFormat ToVncPixelFormat(this PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format16bppRgb565:
                    return VncPixelFormat.RGB16;

                case PixelFormat.Format32bppRgb:
                    return VncPixelFormat.RGB32;

                default:
                    throw new NotSupportedException($"The pixelformat '{pixelFormat}' is not supported.");
            }
        }
    }
}
