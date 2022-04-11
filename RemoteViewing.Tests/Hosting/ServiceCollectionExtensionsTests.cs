using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RemoteViewing.Hosting;
using RemoteViewing.Vnc;
using Xunit;

namespace RemoteViewing.Tests.Hosting
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddVncServer_Default()
        {
            ServiceCollection s = new ServiceCollection();
            Assert.Same(s, s.AddVncServer());
            s.AddLogging();
            s.AddSingleton<ILoggerProvider>(NullLoggerProvider.Instance);
            s.AddSingleton<IVncFramebufferSource>(Mock.Of<IVncFramebufferSource>());
            s.AddSingleton<IVncRemoteController>(Mock.Of<IVncRemoteController>());
            s.AddSingleton<IVncRemoteKeyboard>(Mock.Of<IVncRemoteKeyboard>());

            var provider = s.BuildServiceProvider();

            var worker = provider.GetRequiredService<IHostedService>();
        }
    }
}
