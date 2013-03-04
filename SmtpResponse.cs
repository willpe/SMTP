namespace Smtp {
	public sealed class SmtpReply {
		public const string TERMINATION_SEQUENCE = "\r\n";

		public static readonly SmtpReply Ok = new SmtpReply(ReplyCode.Ok, "OK");

		private readonly ReplyCode replyCode;
		private readonly string message;

		public SmtpReply(ReplyCode _replyCode, string _message) {
			replyCode = _replyCode;
			message = _message;
		}

		public ReplyCode ReplyCode { 
			get { 
				return replyCode; 
			} 
		}

		public string Message { 
			get { 
				return message; 
			} 
		}

		public override string ToString() {
			return ((int)replyCode) + " " + message + SmtpReply.TERMINATION_SEQUENCE;
		}
	}
}