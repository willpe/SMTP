namespace Smtp {
	public sealed class SmtpReply {
		public const string TerminationSequence = "\r\n";

		public static readonly SmtpReply Ok = new SmtpReply(ReplyCode.Ok, "OK");

		private readonly ReplyCode replyCode;
		private readonly string message;

		public SmtpReply(ReplyCode replyCode, string message) {
			this.replyCode = replyCode;
			this.message = message;
		}

		public ReplyCode ReplyCode { 
			get { 
				return this.replyCode; 
			} 
		}

		public string Message { 
			get { 
				return this.message; 
			} 
		}

		public override string ToString() {
			return ((int)this.replyCode) + " " + this.message + SmtpReply.TerminationSequence;
		}
	}
}