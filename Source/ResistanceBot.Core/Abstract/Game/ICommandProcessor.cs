using ResistanceBot.Core.Data;

namespace ResistanceBot.Core.Abstract.Game
{
	public interface ICommandProcessor
	{
		void ProcessCommand(GameCommand command);
	}
}