using ResistanceBot.Logging.Data;

namespace ResistanceBot.Logging.Abstract
{
	public interface ILogger
	{
		void Log(Severity severity, string message);
		void Log(string message);

	}
}