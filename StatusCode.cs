namespace Smtp {
	public enum ReplyCode {
		SystemStatus = 211,
		HelpMessage = 214,
		ServiceReady = 220,
		ServiceClosing = 221,
		Ok = 250,
		MessageForwarded = 251,
		StartMailInput = 354,
		ServiceNotAvailable = 421,
		MailboxBusy = 450,
		LocalProcessingError = 451,
		InsufficientStorage = 452,
		CommandUnrecognized = 500,
		CommandArgumentError = 501,
		CommandNotImplemented = 502,
		BadCommandSequence = 503,
		CommandParameterNotImplemented = 504,
		MailboxNotFound = 550,
		UserNotLocal = 551,
		ExceededStorageAllocation = 552,
		MailboxSyntaxIncorrect = 553,
		TransactionFailed = 554
	}
}