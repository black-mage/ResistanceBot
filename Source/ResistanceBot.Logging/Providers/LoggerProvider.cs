using ResistanceBot.Logging.Abstract;
using ResistanceBot.Logging.Loggers;

namespace ResistanceBot.Logging.Providers
{
	public class LoggerProvider
	{
		public static bool LoggingEnabled = true;


		private static ILogger _logger;

		public static ILogger GetLogger()
		{
			return _logger ?? new ConsoleLogger();
		}

		public static void SetProvidedLogger(ILogger logger)
		{
			if (logger != null)
				_logger = logger;
		}
	}
}