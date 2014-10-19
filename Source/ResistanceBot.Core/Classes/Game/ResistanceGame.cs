using System;
using System.Collections.Generic;
using System.Linq;
using ResistanceBot.Core.Abstract.Game;
using ResistanceBot.Core.Classes.Game.CommandProcessors;
using ResistanceBot.Core.Data;

namespace ResistanceBot.Core.Classes.Game
{
	public class ResistanceGame
	{
		private string orderMessage;
		public List<Player> Players { get; private set; }
		public List<Player> Good { get; private set; }
		public List<Player> Bad { get; private set; } 
		public ResistanceGameBot IrcBot { get; private set; }
		public int FailCount { get; private set; }

		public int ResistanceScore { get; private set; }
		public int SpyScore { get; private set; }
		protected bool Ended { get; private set; }

		public ICommandProcessor CurrentProcessor { get; set; }


		private string _channel;


		private int _currentLeader=0;
		private  int currentMission = 0;

		public string Channel { get { return _channel; } }

		public void MessageChannel(string message)
		{
			IrcBot.SendMessage(_channel,message);
		}

		public void MessagePlayer(string player,string message)
		{
			if(IsPlayerInGame(player))
			{
				IrcBot.SendMessage(player,message);
			}
		}

		public void PrivateMessageAllPlayers(string message)
		{
			foreach(var player in Players.Select(p => p.Nickname))
			{
				IrcBot.SendMessage(player,message);
			}
		}

		public bool IsPlayerASpy(Player player)
		{
			return Bad.Contains(player);
		}

		public Player GetPlayerByName(string player)
		{
			return Players.SingleOrDefault(p => p.Nickname == player);
		}

		public bool IsPlayerInGame(string player)
		{
			return Players.Any(p => p.Nickname == player);
		}

		public ResistanceGame(string channel,IEnumerable<string> players,ResistanceGameBot bot)
		{
			IrcBot = bot;
			Players = new List<Player>();
			_channel = channel;
			foreach(var player in players)
				Players.Add(new Player(player));

			



			AssignPlayerRoles();
			StartNextMission();
		
		}


		public void StartNextMission()
		{
			FailCount = 0;
			var mission = GameRules.MissionSets[Players.Count][currentMission];
			var players = mission.PlayersRequired;
			var failCards = mission.FailCardsRequired;
			CurrentProcessor = new MissionAssignment(this,Players[_currentLeader],failCards,players,currentMission+1);
		}

		public void TeamVoteFail()
		{
			NextLeader();
			FailCount++;
			if(FailCount==5)
				GameOver(true);

			var mission = GameRules.MissionSets[Players.Count][currentMission];
			var players = mission.PlayersRequired;
			var failCards = mission.FailCardsRequired;
			CurrentProcessor = new MissionAssignment(this,Players[_currentLeader],failCards,players,currentMission+1);
		}

		private void NextLeader()
		{
			_currentLeader++;
			if (_currentLeader == Players.Count)
				_currentLeader = 0;
		}

		public void ProcessCommand(GameCommand command)
		{
			CurrentProcessor.ProcessCommand(command);
		}

		private void AssignPlayerRoles()
		{
			var random = new Random();

			var playerCount = Players.Count;

			for(var r=0;r<playerCount;r++)
			{
				var i = random.Next(0, playerCount);
				var temp = Players[i];
				Players[i] = Players[r];
				Players[r] = temp;
			}
			var spyCount = GameRules.SpyCount[playerCount];

			

			Good = new List<Player>();
			Bad = new List<Player>();

			
			for(var x=0;x<spyCount;x++)
			{
				while(true)
				{
					var i = random.Next(0, playerCount);
					if (!Bad.Contains(Players[i]))
					{
						Bad.Add(Players[i]);
						break;
					}

				}
			}

			Good = Players.Where(p => !Bad.Contains(p)).ToList();

			orderMessage = Players.Select(p => p.ToString()).Aggregate((a, b) => a + " " + b);

			IrcBot.SendMessage(_channel, "Welcome to The Resistance :D");
			IrcBot.SendMessage(_channel, string.Format("This is a {0} player game so there will be {1} spies and {2} loyal resistance members.", playerCount, spyCount, playerCount - spyCount));
			IrcBot.SendMessage(_channel, string.Format("The turn will rotate in the following order: {0}", orderMessage));

			foreach(var spy in Bad)
			{
				var spies = Bad.Where(s => s != spy).Select(n => n.Nickname).Aggregate((a,b)=>a+", "+b);
				var msg = string.Format("You are a spy. The other spy(s) are: {0}.", spies);
				MessagePlayer(spy.Nickname,msg);
			}
		}

		public void FinishMission(int failVotes)
		{
			var mission = GameRules.MissionSets[Players.Count][currentMission];
			var players = mission.PlayersRequired;
			var failCards = mission.FailCardsRequired;

			MessageChannel(string.Format("Results for mission {0}.", currentMission+1));
			MessageChannel(string.Format("Successes: {0}, Fails: {1}.", players - failVotes, failVotes));
			if(failVotes >= failCards)
			{
				MessageChannel("The spies have won this mission and gain a point.");
				SpyScore++;
			}
			else
			{
				MessageChannel("The resistance has won this mission and gains a point.");
				ResistanceScore++;
			}

			
			if(SpyScore==3)
			{
				//Spies wins
				GameOver(true);
				return;
			}
			if(ResistanceScore==3)
			{
				//Resistance wins
				GameOver(false);
				return;
			}

			FailCount = 0;
			currentMission++;
			NextLeader();
			StartNextMission();
		}

		public void ShowScores()
		{
			MessageChannel(string.Format("Score: Resistance: {0}, Spies: {1}. First to 3 wins.", ResistanceScore, SpyScore));
		}


		public void GameOver(bool didSpysWin)
		{
			MessageChannel("Game over");
			
			
				
			
			if (didSpysWin)
			{
				MessageChannel("The resistance has fallen.");
				MessageChannel(string.Format("The spies won the game. {0} points to {1}", SpyScore, ResistanceScore));
				
			}
			else
			{
				MessageChannel("The resistance is victorious. The spies have been exposed.");
				MessageChannel(string.Format("The resistance won the game. {0} points to {1}", ResistanceScore, SpyScore));
			}
			var spies = Bad.Select(n => n.Nickname).Aggregate((a, b) => a + ", " + b);
			var msg = string.Format("The spies in this game were: {0}.", spies);
			MessageChannel(msg);
			IrcBot.EndGame(_channel);
		}
	}
}