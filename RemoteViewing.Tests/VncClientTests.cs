using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RemoteViewing.Vnc;
using RemoteViewing.Windows.Forms;

namespace RemoteViewing.Tests
{
    [TestFixture]
    internal class VncClientTests
    {
        public const string Address = "192.168.0.23";

        [Test]
        public void Test()
        {
            using (var vncClient = new VncClient())
            {
                vncClient.Connect(Address, 5900,
                    new VncClientConnectOptions {OnDemandMode = true, PasswordRequiredCallback = c => "100".ToArray()});

                var width = 200;
                var height = 300;
                var copy =  vncClient.GetFramebuffer(100, 100, width, height);
                var bitmap = this.CreateBitmap(copy, width, height);
                bitmap.Save(@"C:\TestOutput\Test.bmp", ImageFormat.Bmp);
                Assert.AreNotEqual(Color.Black.ToArgb(), bitmap.GetPixel(10, 10).ToArgb());
               
            }

        }

        private Bitmap CreateBitmap(Action<IntPtr, int> copyAction, int width, int heigth)
        {
            var bitmap = new Bitmap(width, heigth);
            var winformsRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var data = bitmap.LockBits(winformsRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            try
            {
                copyAction(data.Scan0, data.Stride);
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
            return bitmap;
        }
    }
}
