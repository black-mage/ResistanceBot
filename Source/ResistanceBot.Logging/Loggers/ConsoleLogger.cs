using System;
using System.Collections.Generic;
using ResistanceBot.Logging.Abstract;
using ResistanceBot.Logging.Data;
using ResistanceBot.Logging.Providers;

namespace ResistanceBot.Logging.Loggers
{
	public class ConsoleLogger : ILogger
	{
		private static Dictionary<Severity,ConsoleColor> _colors = new Dictionary<Severity, ConsoleColor>
		{
			{Severity.INFO,ConsoleColor.White},
			{Severity.WARNING,ConsoleColor.Yellow},
			{Severity.ERROR,ConsoleColor.Red},
			{Severity.CRITICAL,ConsoleColor.Magenta}
		};
		public void Log(Severity severity, string message)
		{
			if (LoggerProvider.LoggingEnabled)
			{
				var timeStamp = DateTime.Now;
				var oldColor = Console.ForegroundColor;
				Console.ForegroundColor = _colors[severity];
				Console.WriteLine("{0}:{1} [{2}] {3}", timeStamp.ToShortDateString(), timeStamp.ToShortTimeString(),
				                  severity.ToString(), message);
				Console.ForegroundColor = oldColor;
			}

		}

		public void Log(string message)
		{
			Log(Severity.INFO,message);
		}
	}
}