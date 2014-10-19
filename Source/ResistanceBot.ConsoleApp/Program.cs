using ResistanceBot.Core.Classes;

namespace ResistanceBot.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{

			new ResistanceGameBot("irc.devhat.net", "spud", "rawr", "#resistance");
			while (true) ;
		}
	}
}
