using System;

namespace Smtp {
	internal sealed class SmtpCommand {
		public const string TERMINATION_SEQUENCE = "\r\n";
		public const string DATA_TERMINATION_SEQUENCE = "\r\n.\r\n";
		private const int MESSAGE_LENGTH = 4;

		private readonly string commandCode;
		private readonly string parameters;
		


		public SmtpCommand(string commandCode, string parameters) {
			if (string.IsNullOrEmpty(commandCode)) {
				throw new ArgumentNullException("commandCode");
			}

			this.commandCode = commandCode.ToUpperInvariant();
			this.parameters = parameters;
		}

		public string Parameters {
			get { 
				return this.parameters; 
			}
		}

		public string CommandCode {
			get { 
				return this.commandCode; 
			}
		}

		public static SmtpCommand Parse(string message) {
			if (message.Length < MESSAGE_LENGTH) {
				return null;
			}

			var commandCode = message.Substring(0, MESSAGE_LENGTH);

			string parameters = null;
			if (message.Length > MESSAGE_LENGTH) {
				parameters = message.Substring(MESSAGE_LENGTH, message.Length - MESSAGE_LENGTH).Trim();
			}

			return new SmtpCommand(commandCode, parameters);
		}
	}
}
