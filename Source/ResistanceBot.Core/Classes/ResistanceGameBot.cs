using System;
using System.Linq;
using Meebey.SmartIrc4net;
using ResistanceBot.Core.Classes.Game;
using ResistanceBot.Core.Data;
using ResistanceBot.Logging.Abstract;
using ResistanceBot.Logging.Data;
using ResistanceBot.Logging.Providers;

namespace ResistanceBot.Core.Classes
{
	public class ResistanceGameBot
	{
		private readonly ILogger _logger = LoggerProvider.GetLogger();
		private readonly ResistanceGameManager _resistanceGameManager;
	    private string Server { get; set; }
	    private string Channel { get; set; }
		public string Nickname { get; private set; }
	    private string Username { get; set; }

		private readonly IrcClient _irc;
		

		public ResistanceGameBot(string server, string nickname, string username, string channel)
		{
			
			Channel = channel;
			Nickname = nickname;
			Server = server;
			Username = username;

			_irc = new IrcClient();
			_irc.SendDelay = 20;
			_irc.ActiveChannelSyncing = true;

			_irc.OnConnected += IrcOnConnected;
			_irc.OnJoin += IrcOnJoin;
			_irc.OnChannelMessage +=IrcOnChannelMessage;
			_irc.OnQueryMessage += IrcOnQueryMessage;
			_irc.Connect(Server,6667);

			_resistanceGameManager = new ResistanceGameManager(this);
			_irc.Listen();
		}

		public void SendMessage(string dest, string message)
		{
			_irc.RfcPrivmsg(dest,message);
		}

		public bool AreUsersInChannel(string channel,params String[] names)
		{
			var channelInfo = _irc.GetChannel(channel);
			foreach(var name in names)
			{
				if (!channelInfo.Users.Contains(name))
					return false;
			}
			return true;
		}

		public void EndGame(string channel)
		{
			_resistanceGameManager.EndGame(channel);
		}
		private void IrcOnQueryMessage(object sender, IrcEventArgs e)
		{
			_logger.Log(string.Format("QUERY - {0}: {1}",e.Data.Nick, e.Data.Message));
			var message = e.Data.Message;
			if (message.StartsWith("#"))
			{
				

				var command = message.Split(' ');
				var channel= command[0];
				var keyword = command[1].ToUpper();
				var args = command.Skip(2).ToArray();


				var game = _resistanceGameManager.GetByChannel(channel);
				if (game!=null)
				{
					var gameCommand = new GameCommand(keyword, args, e.Data.Nick, true);
					game.ProcessCommand(gameCommand);
				}

			}
		}

		private void IrcOnChannelMessage(object sender, IrcEventArgs e)
		{
			_logger.Log(string.Format("{0} - {1}: {2}",e.Data.Channel,e.Data.Nick,e.Data.Message));
			var message = e.Data.Message;

			if(message.StartsWith("!"))
			{
				message = message.TrimStart('!');

				var command = message.Split(' ');
				var keyword = command[0].ToUpper();
				var args = command.Skip(1).ToArray();

				if(keyword == "START" && _resistanceGameManager.IsGameRunning(e.Data.Channel)==false)
				{
					_resistanceGameManager.TryStartNewGame(e.Data.Channel,args);
				}
				else
				{
					var gameCommand = new GameCommand(keyword,args,e.Data.Nick, false);
					var game = _resistanceGameManager.GetByChannel(e.Data.Channel);

					if (game != null)
					{
						game.ProcessCommand(gameCommand);
					}
				}
			}
		}

		void IrcOnJoin(object sender, JoinEventArgs e)
		{
			_logger.Log(string.Format("{0} joined channel '{1}'", e.Who,e.Channel));
		}

		void IrcOnConnected(object sender, System.EventArgs e)
		{
			_logger.Log(string.Format("Connected to '{0}'", Server));

			try
			{
				_irc.Login(Nickname, Username);
			}
			catch(Exception exception)
			{
				_logger.Log(Severity.CRITICAL,exception.Message);
			}
			_irc.RfcJoin(Channel);
		}



		

		
	}
}