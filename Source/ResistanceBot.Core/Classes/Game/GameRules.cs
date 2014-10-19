using System.Collections.Generic;
using ResistanceBot.Core.Data;

namespace ResistanceBot.Core.Classes.Game
{
	public class GameRules
	{
		public static Dictionary<int,int> SpyCount = new Dictionary<int, int>{{5,2},{6,2},{7,3},{8,3},{9,3},{10,4}};
 		
		public static Dictionary<int,List<Mission>> MissionSets = new Dictionary<int, List<Mission>> 
		{
			{5,new List<Mission> { new Mission(2),new Mission(3),new Mission(2),new Mission(3),new Mission(3)}},
			{6,new List<Mission> { new Mission(2),new Mission(3),new Mission(4),new Mission(3),new Mission(4)}},
			{7,new List<Mission> { new Mission(2),new Mission(3),new Mission(3),new Mission(4,2),new Mission(4)}},
			{8,new List<Mission> { new Mission(3),new Mission(4),new Mission(4),new Mission(5,2),new Mission(5)}},
			{9,new List<Mission> { new Mission(3),new Mission(4),new Mission(4),new Mission(5,2),new Mission(5)}},
			{10,new List<Mission> { new Mission(3),new Mission(4),new Mission(4),new Mission(5,2),new Mission(5)}}
		}; 
	}
}