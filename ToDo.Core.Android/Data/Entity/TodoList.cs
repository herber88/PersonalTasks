using System;

namespace ToDo.Core.iOS
{
	public class TodoList
	{
		public string Title {get;set;}
		public string[] AvailableStatuses { get; set; }

		public TodoList ()
		{
		}
	}
}

