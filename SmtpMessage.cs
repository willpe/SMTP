using System.Collections.Generic;

namespace Smtp {
	public sealed class SmtpMessage {
		private readonly ICollection<string> to;

		public SmtpMessage() {
			this.to = new List<string>();
		}

		public string From { get; set; }

		public ICollection<string> To {
			get { 
				return this.to; 
			}
		}

		public string Body { get; set; }
	}
}