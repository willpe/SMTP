using System;
using System.Net.Sockets;
using System.Text;

namespace Smtp {
	public sealed class SmtpSession {
		private static readonly SmtpReply welcomeMessage = new SmtpReply(ReplyCode.ServiceReady, "Simple Mail Transfer Service Ready");

		private readonly NetworkStream stream;

		private string currentCommand;
		private string sender;
		private bool isReadingData;
		private bool isClosed;
		private SmtpMessage currentMessage;

		internal SmtpSession(Socket socket) {
			stream = new NetworkStream(socket, true);
			sendReply(welcomeMessage);
			beginRead();
		}

		internal event EventHandler<MessageReceivedEventArgs> MessageReceived;

		private void beginRead() {
			byte[] buffer = new byte[512];
			stream.BeginRead(buffer, 0, buffer.Length, this.endRead, buffer);
		}

		private void endRead(IAsyncResult result) {
			byte[] buffer = (byte[])result.AsyncState;
			var count = this.stream.EndRead(result);

			var message = Encoding.UTF8.GetString(buffer, 0, count);
			this.currentCommand += message;

			if ((isReadingData == false) && (this.currentCommand.Contains(SmtpCommand.TERMINATION_SEQUENCE))) {
				this.processCurrentCommand();
			} else if ((isReadingData) && (this.currentCommand.Contains(SmtpCommand.DATA_TERMINATION_SEQUENCE))) {
				this.processData();
			}

			if (!this.isClosed) {
				this.beginRead();
			} else {
				this.stream.Dispose();
			}
		}

		private void processCurrentCommand() {
			var command = this.currentCommand;
			this.currentCommand = null;

			if ((command.EndsWith(SmtpCommand.TERMINATION_SEQUENCE) == false)) {
				var commandsLength = command.LastIndexOf(SmtpCommand.TERMINATION_SEQUENCE);
				this.currentCommand = command.Substring(commandsLength, command.Length - commandsLength);
				command = command.Substring(0, commandsLength);
			}

			var commands = command.Split(new[] { SmtpCommand.TERMINATION_SEQUENCE }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < commands.Length; i++) {
				var smtpCommand = SmtpCommand.Parse(commands[i]);
				SmtpReply reply = this.executeCommand(smtpCommand);
				sendReply(reply);
				Console.WriteLine(reply.Message, reply.ReplyCode.ToString());
			}
		}

		private void processData() {
			var dataLength = this.currentCommand.IndexOf(SmtpCommand.DATA_TERMINATION_SEQUENCE);
			var data = this.currentCommand.Substring(0, dataLength);
			this.endReadingData(data);
			this.currentCommand = this.currentCommand.Substring(dataLength + SmtpCommand.DATA_TERMINATION_SEQUENCE.Length, this.currentCommand.Length - (dataLength + SmtpCommand.DATA_TERMINATION_SEQUENCE.Length));
		}

		private SmtpReply executeCommand(SmtpCommand command) {
			switch (command.CommandCode) {
				case "HELO":
					Console.WriteLine("250 localhost is ready");
					return this.connect(command);
				case "EHLO":
					return connect(command);
				case "MAIL":
					return this.createMail(command);
				case "RCPT":
					return this.addRecipient(command);
				case "DATA":
					return this.startReadingData();
				case "RSET":
					return interrupt();
				case "QUIT":
					return this.quit();
			}
			throw new InvalidOperationException("Command Not Understood: " + command.CommandCode);
		}

		private SmtpReply connect(SmtpCommand command) {
			sender = command.Parameters;
			return SmtpReply.Ok;
		}

		private SmtpReply createMail(SmtpCommand command) {
			this.currentMessage = new SmtpMessage();
			this.currentMessage.From = command.Parameters;
			return SmtpReply.Ok;
		}

		private SmtpReply addRecipient(SmtpCommand command) {
			this.currentMessage.To.Add(command.Parameters);
			return SmtpReply.Ok;
		}

		private SmtpReply startReadingData() {
			this.isReadingData = true;
			return new SmtpReply(ReplyCode.StartMailInput, "Send the mail data, end with .");
		}

		private void endReadingData(string data) {
			this.isReadingData = false;
			this.currentMessage.Body = data;
			this.sendReply(SmtpReply.Ok);
			this.onMessageReceived();
		}

		private SmtpReply interrupt() {
			sender = null;
			return new SmtpReply(ReplyCode.Ok, "Proccess has interrupted");
		}
		
		private SmtpReply quit() {
			this.isClosed = true;
			return new SmtpReply(ReplyCode.ServiceClosing, "Service closing transmission channel");
		}

		

		private void sendReply(SmtpReply response) {
			byte[] sendBuffer = Encoding.UTF8.GetBytes(response.ToString());
			stream.Write(sendBuffer, 0, sendBuffer.Length);
		}

		private void onMessageReceived() {
			if (this.MessageReceived != null) {
				var e = new MessageReceivedEventArgs(this.currentMessage);
				this.MessageReceived(this, e);
			}
		}
	}
}
