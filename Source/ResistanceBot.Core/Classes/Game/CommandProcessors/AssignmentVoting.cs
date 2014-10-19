using System.Collections.Generic;
using System.Linq;
using ResistanceBot.Core.Abstract.Game;
using ResistanceBot.Core.Data;
using ResistanceBot.Logging.Abstract;
using ResistanceBot.Logging.Providers;

namespace ResistanceBot.Core.Classes.Game.CommandProcessors
{
	public class AssignmentVoting : ICommandProcessor
	{
		private ILogger _logger = LoggerProvider.GetLogger();

		private readonly ResistanceGame _game;
		private readonly string[] _proposedTeam;

		private readonly Dictionary<Player, bool> _votes;
		public AssignmentVoting(ResistanceGame game, string[] proposedTeam)
		{
			_game = game;
			_proposedTeam = proposedTeam;

			var proposalMessage = _proposedTeam.Aggregate((a,b)=> a+" "+b);
			_game.MessageChannel(string.Format("The following team has been proposed: {0}", proposalMessage));
			_game.PrivateMessageAllPlayers(string.Format("Please vote for the team proposed ({0}) by typing the vote command in the following format.", proposalMessage));
			_game.PrivateMessageAllPlayers(string.Format("'{0} vote yes/no'. The channel is required to link you with the correct game. Choose yes or no. This command is case insensitive.", _game.Channel));
			_game.MessageChannel(string.Format("The vote has failed {0} times in a row for this mission. If the vote fails 5 times in a row then the spies automatically win.",_game.FailCount));
			_votes = new Dictionary<Player, bool>();
	
		}

		public void ProcessCommand(GameCommand command)
		{
			if(command.IsPrivate == false)
			{
				return;
			}


			_logger.Log(command.Command);		
			var sender = command.Sender;

			if(_game.IsPlayerInGame(sender)==false)
			{
				_game.MessagePlayer(sender,"ERROR: You are not participating in this game.");
				return;
			}

			if(command.Command=="VOTE")
			{
				if(command.Args.Length!=1)
				{
					_game.MessagePlayer(sender,"ERROR: The vote command requires either a 'yes' or 'no' only. Case insensitive.");
					return;
				}

				if(command.Args[0].ToUpper()=="YES" || command.Args[0].ToUpper()=="NO")
				{
					var player = _game.GetPlayerByName(sender);
					bool vote = command.Args[0].ToUpper() == "YES";

					if(_votes.ContainsKey(player))
					{
						_votes[player] = vote;
					}
					else
					{
						_votes.Add(player,vote);
					}

					_game.MessagePlayer(sender,"Your vote has been counted.");

					if(_votes.Count == _game.Players.Count)
					{
						var yes = _votes.Where(y => y.Value);
						var no = _votes.Where(n => n.Value == false);

						var yesMsg = yes.Any() ? yes.Select(ym => ym.Key.Nickname).Aggregate((a, b) => a + " " + b) : "Nobody";
						var noMsg = no.Any()? no.Select(nm => nm.Key.Nickname).Aggregate((a, b) => a + " " + b): "Nobody";

						var msg = string.Format("Yes: {0}. No: {1}.", yesMsg, noMsg);
						_game.MessageChannel("Everybody has voted. Here are the results:");
						_game.MessageChannel(msg);

						if(yes.Count()>no.Count())
						{
							_game.MessageChannel("The resistance has voted in favor of sending the proposed team on the mission.");
							var playerList = _proposedTeam.Select(s => _game.GetPlayerByName(s)).ToList();
							_game.CurrentProcessor = new MissionVoting(playerList,_game);
						}
						else
						{
							_game.MessageChannel("The resistance has voted against sending the proposed team on the mission.");
							_game.TeamVoteFail();
						}
					}
				}
				else
				{
					_game.MessagePlayer(sender, "ERROR: The vote command requires either a 'yes' or 'no' only. Case insensitive.");
					
				}
			}
		}
	}
}