using System.Collections.Generic;
using System.Linq;
using ResistanceBot.Core.Abstract.Game;
using ResistanceBot.Core.Data;
using ResistanceBot.Logging.Abstract;
using ResistanceBot.Logging.Providers;

namespace ResistanceBot.Core.Classes.Game.CommandProcessors
{
	public class MissionVoting : ICommandProcessor
	{
		private ILogger _logger = LoggerProvider.GetLogger();
		private List<Player> _playersOnMission;
		private ResistanceGame _game;
		private readonly Dictionary<Player,bool> _votes;

		public MissionVoting(List<Player> playersOnMission, ResistanceGame game)
		{
			_playersOnMission = playersOnMission;
			_game = game;
			_votes = new Dictionary<Player, bool>();
			_game.MessageChannel("The mission is underway. Waiting for participants to vote.");
			foreach(var player in _playersOnMission)
			{
				_game.MessagePlayer(player.Nickname,"Please vote for thie misssion to succeed or fail.");
				_game.MessagePlayer(player.Nickname,string.Format("'{0} vote succeed/fail'. The channel is required to link you with the correct game. Choose yes or no. This command is case insensitive.", _game.Channel));
			}
		}

		public void ProcessCommand(GameCommand command)
		{
			if(command.IsPrivate == false)
			{
				return;
			}

			var sender = command.Sender;
			if(command.Command == "VOTE")
			{
				if(command.Args.Length!=1)
				{
					_game.MessagePlayer(sender,"ERROR: The vote command requires either a 'succeed' or 'fail' only. Case insensitive.");
					return;
				}

				if (command.Args[0].ToUpper() == "SUCCEED" || command.Args[0].ToUpper() == "FAIL")
				{
					var player = _game.GetPlayerByName(sender);

					if (!_playersOnMission.Contains(player))
					{
						_game.MessagePlayer(sender, "ERROR: You are not on this mission.");
						return;
					}
					bool vote = command.Args[0].ToUpper() != "FAIL";

					if (vote == false)
					{
						if (!_game.IsPlayerASpy(player))
						{
							_game.MessagePlayer(sender, "ERROR: You are a member of the resistance and MUST vote for the mission to succeed");
							return;
						}
					}

					if (_votes.ContainsKey(player))
					{
						_game.MessagePlayer(sender, "ERROR: You have already voted on this mission.");
						return;

					}

					_logger.Log(string.Format("{0} has voted {1}", sender, vote));
					_votes.Add(player, vote);

					if (_votes.Count == _playersOnMission.Count)
					{
						_game.FinishMission(_votes.Count(v => v.Value == false));
					}

					_game.MessagePlayer(sender,"Your vote has been counted");
				}

			}
		}
	}
}