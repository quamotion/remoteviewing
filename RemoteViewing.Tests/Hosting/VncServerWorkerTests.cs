﻿using Moq;
using RemoteViewing.Hosting;
using RemoteViewing.Vnc.Server;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace RemoteViewing.Tests.Hosting
{
    public class VncServerWorkerTests
    {
        [Fact]
        public void Constructor_ValidateArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new VncServerWorker(null, new VncServerOptions()));
            Assert.Throws<ArgumentNullException>(() => new VncServerWorker(Mock.Of<IVncServer>(), null));
            Assert.Throws<ArgumentNullException>(() => new VncServerWorker(null, null));
        }

        [Fact]
        public async Task StartStop_Works()
        {
            var server = new Mock<IVncServer>(MockBehavior.Strict);
            var options = new VncServerOptions();

            var worker = new VncServerWorker(server.Object, options);

            server.Setup(s => s.StartAsync(new IPEndPoint(IPAddress.Loopback, 5900), default)).Returns(Task.CompletedTask).Verifiable();
            await worker.StartAsync(default).ConfigureAwait(false);
            server.Verify();

            server.Setup(s => s.StopAsync(default)).Returns(Task.CompletedTask).Verifiable();
            await worker.StopAsync(default).ConfigureAwait(false);
            server.Verify();
        }

        [Fact]
        public async Task StartStopReverse_Works()
        {
            var server = new Mock<IVncServer>(MockBehavior.Strict);
            var options = new VncServerOptions() { Address = "10.0.0.0", Port = 1234, Reverse = true };

            var worker = new VncServerWorker(server.Object, options);

            server.Setup(s => s.StartReverseAsync(new IPEndPoint(IPAddress.Parse("10.0.0.0"), 1234), default)).Returns(Task.CompletedTask).Verifiable();
            await worker.StartAsync(default).ConfigureAwait(false);
            server.Verify();

            server.Setup(s => s.StopAsync(default)).Returns(Task.CompletedTask).Verifiable();
            await worker.StopAsync(default).ConfigureAwait(false);
            server.Verify();
        }
    }
}
