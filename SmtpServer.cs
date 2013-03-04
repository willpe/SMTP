using System;
using System.Net;
using System.Net.Sockets;

namespace Smtp {
	public sealed class SmtpServer {
		private readonly Socket listenSocket;
		private readonly int port;

		public SmtpServer(int _port) {
			port = _port;
			listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		public event EventHandler<MessageReceivedEventArgs> MessageReceived;

		public int Port {
			get { 
				return this.port; 
			}
		}

		public void Open() {
			IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
			listenSocket.Bind(endpoint);
			listenSocket.Listen(255);
			listenSocket.BeginAccept(this.onAcceptingSocket, listenSocket);
		}

		public void Close() {
			this.listenSocket.Close();
		}

		private void onAcceptingSocket(IAsyncResult result) {
			try {
				SmtpSession session = new SmtpSession(listenSocket.EndAccept(result));
				session.MessageReceived += onSessionMessageReceived;
			} catch {
				Console.WriteLine("SMTP Session is destroyed");
				Console.ReadKey();
			} finally {
				listenSocket.BeginAccept(onAcceptingSocket, listenSocket);						
			}
		}

		private void onSessionMessageReceived(object sender, MessageReceivedEventArgs e) {
			if (MessageReceived != null) {
				MessageReceived(this, e);
			}
		}
	}
}