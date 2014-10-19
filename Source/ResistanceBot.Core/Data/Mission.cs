namespace ResistanceBot.Core.Data
{
	public class Mission
	{
		public int PlayersRequired { get; private set; }
		public int FailCardsRequired { get; private set; }

		public Mission(int playersRequired, int failCardsRequired=1)
		{
			FailCardsRequired = failCardsRequired;
			PlayersRequired = playersRequired;
		}
	}
}