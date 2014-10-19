namespace ResistanceBot.Core.Data
{
	public class GameCommand
	{
		public string Sender { get; private set; }
		public string Command { get; private set; }
		public string[] Args { get; private set; }
		public bool IsPrivate { get; private set; }

		public GameCommand(string command, string[] args, string sender, bool isPrivate)
		{
			Command = command;
			IsPrivate = isPrivate;
			Sender = sender;
			Args = args;
		}
	}
}