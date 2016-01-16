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

using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using RemoteViewing.Vnc;
using RemoteViewing.Vnc.Server;
using RemoteViewing.Windows.Forms.Server;

namespace RemoteViewing.ServerExample
{
    class Program
    {
        static string Password = "test";
        static VncServerSession Session;

        static void HandleConnected(object sender, EventArgs e)
        {
            Console.WriteLine("Connected");
        }

        static void HandleConnectionFailed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection Failed");
        }

        static void HandleClosed(object sender, EventArgs e)
        {
            Console.WriteLine("Closed");
        }

        static void HandlePasswordProvided(object sender, PasswordProvidedEventArgs e)
        {
            e.Accept(Password.ToCharArray());
        }

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Listening on local port 5900.");
            Console.WriteLine("Try to connect! The password is: {0}", Password);

            // Wait for a connection.
            var listener = new TcpListener(IPAddress.Any, 5900);
            listener.Start();
            var client = listener.AcceptTcpClient();

            // Set up a framebuffer and options.
            var options = new VncServerSessionOptions();
            options.AuthenticationMethod = AuthenticationMethod.Password;

            // Create a session.
            Session = new VncServerSession();
            Session.Connected += HandleConnected;
            Session.ConnectionFailed += HandleConnectionFailed;
            Session.Closed += HandleClosed;
            Session.PasswordProvided += HandlePasswordProvided;
            Session.SetFramebufferSource(new VncScreenFramebufferSource("Hello World", Screen.PrimaryScreen));
            Session.Connect(client.GetStream(), options);

            // Let's go.
            Application.Run();
        }
    }
}
