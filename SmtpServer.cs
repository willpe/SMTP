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
			IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, this.port);
			this.listenSocket.Bind(endpoint);
			this.listenSocket.Listen(255);
			this.listenSocket.BeginAccept(this.onAcceptingSocket, null);
		}

		public void Close() {
			this.listenSocket.Close();
		}

		private void onAcceptingSocket(IAsyncResult result) {
			try {
				SmtpSession session = new SmtpSession(this.listenSocket.EndAccept(result));
				session.MessageReceived += onSessionMessageReceived;
			} catch {
				Console.WriteLine("SMTP Session is destroyed");
				Console.ReadKey();
			} finally {
				listenSocket.BeginAccept(onAcceptingSocket, listenSocket);						
			}
		}

		private void onSessionMessageReceived(object sender, MessageReceivedEventArgs e) {
			if (this.MessageReceived != null) {
				this.MessageReceived(this, e);
			}
		}
	}
}