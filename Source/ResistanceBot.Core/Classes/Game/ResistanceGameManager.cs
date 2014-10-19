using System;
using System.Collections.Generic;
using System.Linq;
using ResistanceBot.Core.Extensions;
using ResistanceBot.Logging.Abstract;
using ResistanceBot.Logging.Data;
using ResistanceBot.Logging.Providers;

namespace ResistanceBot.Core.Classes.Game
{
	public class ResistanceGameManager
	{
		private ILogger _logger = LoggerProvider.GetLogger();

		public ResistanceGameBot IrcBot { get; private set; }

		public Dictionary<string, ResistanceGame> _runningGames;

		public ResistanceGameManager(ResistanceGameBot ircBot)
		{
			IrcBot = ircBot;
			_runningGames = new Dictionary<string, ResistanceGame>();
		}

		public bool IsGameRunning(string channel)
		{
			return _runningGames.ContainsKey(channel);

		}

		public void EndGame(string channel)
		{
			if (_runningGames.ContainsKey(channel))
				_runningGames.Remove(channel);
		}
		public ResistanceGame GetByChannel(string channel)
		{
			if (_runningGames.ContainsKey(channel))
				return _runningGames[channel];
			return null;
		}

		public bool TryStartNewGame(string channel,string[] players)
		{
			if(players.Duplicates().Any())
			{
				IrcBot.SendMessage(channel,"ERROR: Duplicate nickname detected. This game needs unique players only.");
				return false;
			}
			if (_runningGames.ContainsKey(channel))
			{
				IrcBot.SendMessage(channel, "ERROR: This channel already has a game of the resistance running.");
				return false;
			}
			
			if (_runningGames.ContainsKey(channel))
			{
				IrcBot.SendMessage(channel, "ERROR: This channel already has a game of the resistance running.");
				return false;
			}
			if(Array.IndexOf(players,IrcBot.Nickname)!= -1)
			{
				IrcBot.SendMessage(channel,"ERROR: I cannot play this game.");
				return false;
			}

			if(IrcBot.AreUsersInChannel(channel, players))
			{
				if(players.Length<5)
				{
					IrcBot.SendMessage(channel,"ERROR: Cannot start the game with less than 5 players");
					return false;
				}
				else if (players.Length>10)
				{
					IrcBot.SendMessage(channel,"ERROR: Cannot start the game with more than 10 players");
					return false;
				}
			}
			else
			{
				IrcBot.SendMessage(channel,"ERROR: Some of the provided nicknames are invalid");
				return false;
			}



			_runningGames.Add(channel,new ResistanceGame(channel,players,IrcBot) );
			return true;
		}
	}
}