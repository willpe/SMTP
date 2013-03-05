using System.Collections.Generic;

namespace Smtp {
	public sealed class SmtpMessage {
		private readonly List<string> recipients;
		public SmtpMessage() {
			recipients = new List<string>();
		}
		public string From { get; set; }
		public List<string> Recipients {
			get { 
				return recipients; 
			}
		}
		public string Body { get; set; }
	}
}