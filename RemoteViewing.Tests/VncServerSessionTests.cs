using RemoteViewing.Vnc;
using RemoteViewing.Vnc.Server;
using System.IO;
using System.Text;
using Xunit;
using Moq;

namespace RemoteViewing.Tests
{
    public class VncServerSessionTests
    {
        [Fact]
        public void FramebufferSendChangesTest()
        {
            var session = new VncServerSession();
            var framebufferSourceMock = new Mock<IVncFramebufferSource>();
            var framebuffer = new VncFramebuffer("My Framebuffer", width: 0x4f4, height: 0x3c1, pixelFormat: VncPixelFormat.RGB32);
            framebufferSourceMock
                .Setup(f => f.Capture())
                .Returns(
                () =>
                {
                    return framebuffer;
                });
            session.FramebufferUpdateRequest = new FramebufferUpdateRequest(false, new VncRectangle(0, 0, 100, 100));
            session.SetFramebufferSource(framebufferSourceMock.Object);
            session.FramebufferSendChanges();

            Assert.Equal(framebuffer, session.Framebuffer);

            framebufferSourceMock = new Mock<IVncFramebufferSource>();
            framebufferSourceMock
                .Setup(f => f.Capture())
                .Throws(new IOException());

            session.FramebufferUpdateRequest = new FramebufferUpdateRequest(false, new VncRectangle(0, 0, 100, 100));
            session.SetFramebufferSource(framebufferSourceMock.Object);
            
            // should not throw and exception.
            session.FramebufferSendChanges();

            Assert.Equal(framebuffer, session.Framebuffer);
        }
    }
}
