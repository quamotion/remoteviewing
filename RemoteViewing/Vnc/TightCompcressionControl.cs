using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteViewing.Vnc
{
    [Flags]
    public enum TightCompcressionControl : byte
    {
        ResetStream0 = 1 << 0,
        ResetStream1 = 1 << 1,
        ResetStream2 = 1 << 2,
        ResetStream3 = 1 << 3,
        FillCompression = 1 << 7,
        JpegCompression = 1 << 7 | 1 << 4,
        PngCompression = 1 << 7 | 1 << 5
    }
}
