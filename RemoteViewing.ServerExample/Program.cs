#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com/software/remoteviewing>
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RemoteViewing.Hosting;
using RemoteViewing.LibVnc;
using RemoteViewing.NoVncExample;
using RemoteViewing.Vnc;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace RemoteViewing.ServerExample
{
    internal class Program
    {
        private static string password = "test";
        private static int port = 5900;

        public static Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand();

            rootCommand.Description = "Remote Viewing VNC Server Sample";

            rootCommand.SetHandler(() => CreateHostBuilder(args).Build().Run());

            var builder = new CommandLineBuilder(rootCommand);
            builder.UseDefaults();
            return builder.Build().InvokeAsync(args);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var framebufferSource = new DummyFramebufferSource();
                    services.AddSingleton<IVncFramebufferSource>(framebufferSource);
                    services.AddSingleton<IVncRemoteController>(framebufferSource);
                    services.AddSingleton<IVncRemoteKeyboard>(framebufferSource);

                    services.AddLogging();
                    services.AddVncServer<LibVncServer>(
                        new VncServerOptions()
                        {
                            Port = port,
                            Password = password,
                            Address = "127.0.0.1",
                        });
                });
    }
}
