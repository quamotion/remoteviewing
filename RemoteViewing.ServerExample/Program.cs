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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using RemoteViewing.NoVncExample;
using RemoteViewing.Vnc;
using RemoteViewing.Vnc.Server;
using System;
using System.Net;
using System.Net.Sockets;

namespace RemoteViewing.ServerExample
{
    internal class Program
    {
        private static string password = "test";

        private static void HandleConnected(object sender, EventArgs e)
        {
            Console.WriteLine("Connected");
        }

        private static void HandleClosed(object sender, EventArgs e)
        {
            Console.WriteLine("Closed");
        }

        private static void HandlePasswordProvided(object sender, PasswordProvidedEventArgs e)
        {
            e.Accept(password.ToCharArray());
        }

        public static void Main(string[] args)
        {
            Console.WriteLine($"64-bit: {Environment.Is64BitProcess}");

            Console.WriteLine("Listening on local port 5900.");
            Console.WriteLine("Try to connect! The password is: {0}", password);

            using (var server = GetServer(useManaged: false))
            {
                server.Closed += HandleClosed;
                server.Connected += HandleConnected;
                server.PasswordProvided += HandlePasswordProvided;

                server.Start(new IPEndPoint(IPAddress.Loopback, 5900));

                Console.WriteLine("Hit ENTER to exit");
                Console.ReadLine();
            }
        }

        private static IVncServer GetServer(bool useManaged)
        {
            var framebufferSource = new DummyFramebufferSource();
            var logger = new ConsoleLogger("VNC", (s, l) => l <= LogLevel.Debug, true);

            return new VncServer(
                framebufferSource,
                framebufferSource,
                framebufferSource,
                logger);
        }
    }
}
