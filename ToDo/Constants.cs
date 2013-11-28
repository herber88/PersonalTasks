using System;
using System.IO;

namespace ToDo
{
	public static class Constants
	{
		public static class Database{
			public static string TrelloData = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "trelloData.db");
		}
	}
}

