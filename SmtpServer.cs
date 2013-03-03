using System;
using System.Net;
using System.Net.Sockets;

namespace Smtp {
	public sealed class SmtpServer {
		private readonly Socket listenSocket;
		private readonly int port;

		public SmtpServer(int port) {
			this.port = port;
			this.listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		public event EventHandler<MessageReceivedEventArgs> MessageReceived;

		public int Port {
			get { 
				return this.port; 
			}
		}

		public void Open() {
			var endpoint = new IPEndPoint(IPAddress.Any, this.port);
			this.listenSocket.Bind(endpoint);
			this.listenSocket.Listen(255);
			this.listenSocket.BeginAccept(this.OnAcceptingSocket, null);
		}

		public void Close() {
			this.listenSocket.Close();
		}

		private void OnAcceptingSocket(IAsyncResult result) {
			var acceptedSocket = this.listenSocket.EndAccept(result);
			var session = new SmtpSession(acceptedSocket);
			session.MessageReceived += this.OnSessionMessageReceived;

			this.listenSocket.BeginAccept(this.OnAcceptingSocket, null);
		}

		private void OnSessionMessageReceived(object sender, MessageReceivedEventArgs e) {
			if (this.MessageReceived != null) {
				this.MessageReceived(this, e);
			}
		}
	}
}