using System.Linq;
using ResistanceBot.Core.Abstract.Game;
using ResistanceBot.Core.Data;
using ResistanceBot.Core.Extensions;

namespace ResistanceBot.Core.Classes.Game.CommandProcessors
{
	public class MissionAssignment : ICommandProcessor
	{
		private readonly ResistanceGame _game;
		private Player _leader;
		private readonly int _failCardsRequired;
		private readonly int _playersRequired;
		private readonly int _missionNumber;

		public MissionAssignment(ResistanceGame game, Player leader, int failCardsRequired, int playersRequired, int missionNumber)
		{
			_game = game;
			_leader = leader;
			_failCardsRequired = failCardsRequired;
			_playersRequired = playersRequired;
			_missionNumber = missionNumber;

			_game.ShowScores();
			_game.MessageChannel(string.Format("Mission {0}. {1} resistance members are required for this mission. {2} fail vote is needed for this mission to fail.",_missionNumber,_playersRequired,_failCardsRequired));
			_game.MessageChannel(string.Format("{0} is the current team leader. Propose a team using the syntax '!PROPOSE <name> <name>'",leader));
		}

		public void ProcessCommand(GameCommand command)
		{
			if (command.IsPrivate == true)
				return;

			if(command.Sender != _leader.Nickname && command.Sender != "boom")
			{
				_game.MessageChannel(string.Format("Only {0} can propose a team.", _leader.Nickname));
				return;
			}
			if(command.Command=="PROPOSE")
			{
				
				var proposal = command.Args;

				if(proposal.Length!=_playersRequired)
				{
					_game.MessageChannel(string.Format("ERROR: This mission requires {0} players.", _playersRequired));
					return;
				}
				
				if (proposal.Duplicates().Any())
				{
					_game.MessageChannel("ERROR: Duplicate nickname(s) detected. Mission assignment must consist of unique player names only.");
					return;
				}

				foreach(var player in proposal)
				{
					if(_game.Players.Select(p => p.Nickname).Contains(player) == false)
					{
						_game.MessageChannel("ERROR: Some of the names you picked are not in the game.");
						return;
					}
				}

				//_game.MessageChannel("Proposal valid");
				_game.CurrentProcessor = new AssignmentVoting(_game,proposal);
			}

		}
	}
}