namespace ResistanceBot.Core.Data
{
	public class Player
	{
		public string Nickname { get; private set; }

		public Player(string nickname)
		{
			Nickname = nickname;
		}

		public override string ToString()
		{
			return Nickname;
		}
	}
}