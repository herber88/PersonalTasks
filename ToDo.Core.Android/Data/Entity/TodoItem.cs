using System;

namespace ToDo.Core.iOS
{
	public class TodoItem
	{
		public string Title { get; set; }
		public string Description { get; set; }
		TodoList List { get; set; }

		public TodoItem ()
		{
		}
	}
}

