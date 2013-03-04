using System;
using System.Net.Mail;

namespace Smtp {
	public static class Program {
		private const int PORT = 25;
		public static void Main() {
			var server = new SmtpServer(PORT);
			server.Open();
			server.MessageReceived += OnMessageReceived;

			using (var client = new SmtpClient("localhost", PORT)) {
				client.Send("kalashnikovisme@gmail.com", "kalashnikovisme@gmail.com", "Hello World", "Dear World,\r\n\r\nHello!\r\n\r\nWill");
			}

			Console.ReadLine();
			server.Close();
		}

		private static void OnMessageReceived(object sender, MessageReceivedEventArgs e) {
			Console.WriteLine(e.Message.From);
			Console.WriteLine(string.Join(", ", e.Message.To));
			Console.WriteLine();
			Console.WriteLine(e.Message.Body);
		}
	}
}