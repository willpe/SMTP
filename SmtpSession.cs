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
			int count = stream.EndRead(result);

			string message = Encoding.UTF8.GetString(buffer, 0, count);
			currentCommand += message;

			if ((isReadingData == false) && (currentCommand.Contains(SmtpCommand.TERMINATION_SEQUENCE))) {
				processCurrentCommand();
			} else if ((isReadingData) && (currentCommand.Contains(SmtpCommand.DATA_TERMINATION_SEQUENCE))) {
				processData();
			}

			if (isClosed == false) {
				beginRead();
			} else {
				stream.Dispose();
			}
		}

		private void processCurrentCommand() {
			string command = this.currentCommand;
			currentCommand = null;
			if ((command.EndsWith(SmtpCommand.TERMINATION_SEQUENCE) == false)) {
				int commandsLength = command.LastIndexOf(SmtpCommand.TERMINATION_SEQUENCE);
				currentCommand = command.Substring(commandsLength, command.Length - commandsLength);
				command = command.Substring(0, commandsLength);
			}
			string[] commands = command.Split(new[] { SmtpCommand.TERMINATION_SEQUENCE }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < commands.Length; i++) {
				SmtpCommand smtpCommand = SmtpCommand.Parse(commands[i]);
				SmtpReply reply = executeCommand(smtpCommand);
				sendReply(reply);
				Console.WriteLine(reply.Message, reply.ReplyCode.ToString());
			}
		}

		private void processData() {
			int dataLength = currentCommand.IndexOf(SmtpCommand.DATA_TERMINATION_SEQUENCE);
			endReadingData(currentCommand.Substring(0, dataLength));
			currentCommand = currentCommand.Substring(dataLength + SmtpCommand.DATA_TERMINATION_SEQUENCE.Length, currentCommand.Length - (dataLength + SmtpCommand.DATA_TERMINATION_SEQUENCE.Length));
		}

		private SmtpReply executeCommand(SmtpCommand command) {
			switch (command.CommandCode) {
				case "EHLO":
					return connect(command);
				case "MAIL":
					return createMail(command);
				case "RCPT":
					return addRecipient(command);
				case "DATA":
					return startReadingData();
				case "RSET":
					return interrupt();
				case "QUIT":
					return quit();
			}
			throw new InvalidOperationException("Command Not Understood: " + command.CommandCode);
		}

		private SmtpReply connect(SmtpCommand command) {
			sender = command.Parameters;
			return SmtpReply.Ok;
		}

		private SmtpReply createMail(SmtpCommand command) {
			currentMessage = new SmtpMessage();
			currentMessage.From = command.Parameters;
			return SmtpReply.Ok;
		}

		private SmtpReply addRecipient(SmtpCommand command) {
			currentMessage.Recipients.Add(command.Parameters);
			return SmtpReply.Ok;
		}

		private SmtpReply startReadingData() {
			isReadingData = true;
			return new SmtpReply(ReplyCode.StartMailInput, "Send the mail data, end with .");
		}

		private void endReadingData(string data) {
			isReadingData = false;
			currentMessage.Body = data;
			sendReply(SmtpReply.Ok);
			onMessageReceived();
		}

		private SmtpReply interrupt() {
			sender = null;
			return new SmtpReply(ReplyCode.Ok, "Proccess has interrupted");
		}
		
		private SmtpReply quit() {
			isClosed = true;
			return new SmtpReply(ReplyCode.ServiceClosing, "Service closing transmission channel");
		}

		private void sendReply(SmtpReply response) {
			byte[] sendBuffer = Encoding.UTF8.GetBytes(response.ToString());
			stream.Write(sendBuffer, 0, sendBuffer.Length);
		}

		private void onMessageReceived() {
			if (MessageReceived != null) {
				MessageReceivedEventArgs e = new MessageReceivedEventArgs(currentMessage);
				MessageReceived(this, e);
			}
		}
	}
}