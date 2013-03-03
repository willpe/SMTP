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
			this.stream = new NetworkStream(socket, true);
			this.SendReply(welcomeMessage);
			this.BeginRead();
		}

		internal event EventHandler<MessageReceivedEventArgs> MessageReceived;

		private void BeginRead() {
			var buffer = new byte[512];
			this.stream.BeginRead(buffer, 0, buffer.Length, this.EndRead, buffer);
		}

		private void EndRead(IAsyncResult result) {
			var buffer = (byte[])result.AsyncState;
			var count = this.stream.EndRead(result);

			var message = Encoding.UTF8.GetString(buffer, 0, count);
			this.currentCommand += message;

			if ((isReadingData == false) && (this.currentCommand.Contains(SmtpCommand.TERMINATION_SEQUENCE))) {
				this.ProcessCurrentCommand();
			} else if ((isReadingData) && (this.currentCommand.Contains(SmtpCommand.DATA_TERMINATION_SEQUENCE))) {
				this.ProcessData();
			}

			if (!this.isClosed) {
				this.BeginRead();
			} else {
				this.stream.Dispose();
			}
		}

		private void ProcessCurrentCommand() {
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
				var reply = this.ExecuteCommand(smtpCommand);
				this.SendReply(reply);
			}
		}

		private void ProcessData() {
			var dataLength = this.currentCommand.IndexOf(SmtpCommand.DATA_TERMINATION_SEQUENCE);
			var data = this.currentCommand.Substring(0, dataLength);
			this.EndReadingData(data);
			this.currentCommand = this.currentCommand.Substring(dataLength + SmtpCommand.DATA_TERMINATION_SEQUENCE.Length, this.currentCommand.Length - (dataLength + SmtpCommand.DATA_TERMINATION_SEQUENCE.Length));
		}

		private SmtpReply ExecuteCommand(SmtpCommand command) {
			switch (command.CommandCode) {
				case "HELO":
				case "EHLO":
					return this.Connect(command);

				case "MAIL":
					return this.CreateMail(command);

				case "RCPT":
					return this.AddRecipient(command);

				case "DATA":
					return this.StartReadingData(command);

				case "QUIT":
					return this.Quit(command);
			}
			throw new InvalidOperationException("Command Not Understood: " + command.CommandCode);
		}

		private SmtpReply Connect(SmtpCommand command) {
			this.sender = command.Parameters;
			return SmtpReply.Ok;
		}

		private SmtpReply CreateMail(SmtpCommand command) {
			this.currentMessage = new SmtpMessage();
			this.currentMessage.From = command.Parameters;
			return SmtpReply.Ok;
		}

		private SmtpReply AddRecipient(SmtpCommand command) {
			this.currentMessage.To.Add(command.Parameters);
			return SmtpReply.Ok;
		}

		private SmtpReply StartReadingData(SmtpCommand command) {
			this.isReadingData = true;
			return new SmtpReply(ReplyCode.StartMailInput, "Send the mail data, end with .");
		}

		private void EndReadingData(string data) {
			this.isReadingData = false;
			this.currentMessage.Body = data;
			this.SendReply(SmtpReply.Ok);
			this.OnMessageReceived();
		}

		private SmtpReply Quit(SmtpCommand command) {
			this.isClosed = true;
			return new SmtpReply(ReplyCode.ServiceClosing, "Service closing transmission channel");
		}

		private void SendReply(SmtpReply response) {
			var sendBuffer = Encoding.UTF8.GetBytes(response.ToString());
			this.stream.Write(sendBuffer, 0, sendBuffer.Length);
		}

		private void OnMessageReceived() {
			if (this.MessageReceived != null) {
				var e = new MessageReceivedEventArgs(this.currentMessage);
				this.MessageReceived(this, e);
			}
		}
	}
}
