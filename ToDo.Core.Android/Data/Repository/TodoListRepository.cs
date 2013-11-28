using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using System.Linq;

namespace ToDo.Core.iOS
{
	public class TodoListRepository : AbstractRepository<TodoList>
	{
		public override IRestClient Api { get; set; }

		public List<TodoList> TodoLists = new List<TodoList>();

		public override Task<bool> Sync ()
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool> ();

			Api.ExecuteAsync<List<TrelloBoard>>(new RestRequest ("members/my/boards"), (result) => {
				var data = result.Data;
				TodoLists = data.Select(tb => new TodoList
					{
						Title = tb.Name
					}).ToList();
				tcs.SetResult(true);
			});

			return tcs.Task;
		}

		public override async Task<IEnumerable<TodoList>> GetAllForCurrentUser ()
		{
			await Sync ();
			return TodoLists;
		}

		public override Task<bool> Add (TodoList entity)
		{
			throw new NotImplementedException ();
		}

		internal TodoListRepository ()
		{

		}

		private class TrelloBoard
		{
			public string Id { get; set; }

			public string Name { get; set;}

			public TrelloBoard ()
			{


			}
		}
	}
}

