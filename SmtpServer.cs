//---------------------------------------------------------------------------------
// Copyright (c) 2012, Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//---------------------------------------------------------------------------------

namespace Smtp
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    public sealed class SmtpServer
    {
        private readonly Socket listenSocket;
        private readonly int port;

        public SmtpServer(int port)
        {
            this.port = port;
            this.listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.WelcomeMessage = "Simple Mail Transfer Service Ready";
        }

        public int Port
        {
            get { return this.port; }
        }

        public string WelcomeMessage { get; set; }

        public void Open()
        {
            var endpoint = new IPEndPoint(IPAddress.Any, this.port);

            this.listenSocket.Bind(endpoint);
            this.listenSocket.Listen(255);
            this.listenSocket.BeginAccept(this.OnAcceptingSocket, null);
        }

        public void Close()
        {
            this.listenSocket.Close();
        }

        private void OnAcceptingSocket(IAsyncResult result)
        {
            var acceptedSocket = this.listenSocket.EndAccept(result);
            var session = new SmtpSession(acceptedSocket);

            this.listenSocket.BeginAccept(this.OnAcceptingSocket, null);
        }
    }
}
