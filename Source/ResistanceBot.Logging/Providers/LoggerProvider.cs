using ResistanceBot.Logging.Abstract;
using ResistanceBot.Logging.Loggers;

namespace ResistanceBot.Logging.Providers
{
	public static class LoggerProvider
	{

		public static ILogger GetLogger()
		{
			return new ConsoleLogger();
		}
	}
}