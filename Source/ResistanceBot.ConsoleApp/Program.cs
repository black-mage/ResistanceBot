using ResistanceBot.Core.Classes;
using ResistanceBot.Logging.Providers;

namespace ResistanceBot.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			LoggerProvider.LoggingEnabled = true;
			new ResistanceGameBot("irc.devhat.net", "spud", "rawr", "#resistance");
			while (true) ;
		}
	}
}
