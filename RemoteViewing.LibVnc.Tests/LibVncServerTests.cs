using Microsoft.Extensions.Logging;
using Moq;
using RemoteViewing.Vnc;
using Xunit;

namespace RemoteViewing.LibVnc.Tests
{
    public class LibVncServerTests
    {
        /// <summary>
        /// A simple unit test which will initialize a new instance of the <see cref="LibVncServer"/> class.
        /// Makes sure both the <c>vncserver</c> and <c>vnclogging</c> native libraries can be loaded correctly.
        /// </summary>
        [Fact]
        public void Constructor_Works()
        {
            var fbs = new Mock<IVncFramebufferSource>();
            var keyboard = new Mock<IVncRemoteKeyboard>();
            var controller = new Mock<IVncRemoteController>();

            var logger = new Mock<ILogger>();

            var server = new LibVncServer(fbs.Object, keyboard.Object, controller.Object, logger.Object);
            Assert.NotNull(server);
        }
    }
}
