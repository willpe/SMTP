using System;

namespace Smtp {
	internal sealed class SmtpCommand {
		public const string TERMINATION_SEQUENCE = "\r\n";
		public const string DATA_TERMINATION_SEQUENCE = "\r\n.\r\n";
		private const int MESSAGE_LENGTH = 4;

		private readonly string commandCode;
		private readonly string parameters;
		
		public SmtpCommand(string _commandCode, string _parameters) {
			if (string.IsNullOrEmpty(_commandCode)) {
				throw new ArgumentNullException("_commandCode");
			}

			commandCode = _commandCode.ToUpperInvariant();
			parameters = _parameters;
		}

		public string Parameters {
			get { 
				return parameters; 
			}
		}

		public string CommandCode {
			get { 
				return commandCode; 
			}
		}

		public static SmtpCommand Parse(string message) {
			if (message.Length < MESSAGE_LENGTH) {
				return null;
			}
			string commandCode = message.Substring(0, MESSAGE_LENGTH);
			string parameters = null;
			if (message.Length > MESSAGE_LENGTH) {
				parameters = message.Substring(MESSAGE_LENGTH, message.Length - MESSAGE_LENGTH).Trim();
			}
			return new SmtpCommand(commandCode, parameters);
		}
	}
}