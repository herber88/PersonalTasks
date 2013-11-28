using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using System.Linq;

namespace ToDo.Core.iOS
{
	public class TodoItemRepository : AbstractRepository<TodoItem>
	{
		public override IRestClient Api { get; set; }

		List<TodoItem> TodoItems = new List<TodoItem>();

		public override Task<bool> Sync ()
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool> ();

			Api.ExecuteAsync<List<TrelloCard>>(new RestRequest ("members/my/cards"), (result) => {
				var data = result.Data;
				TodoItems = data.Select(tb => new TodoItem
					{
						Title =  tb.Name
					}).ToList();
				tcs.SetResult(true);
			});

			return tcs.Task;
		}

		public override async Task<IEnumerable<TodoItem>> GetAllForCurrentUser ()
		{
			await Sync();
			return TodoItems;
		}

		public override Task<bool> Add (TodoItem entity)
		{
			throw new NotImplementedException ();
		}

		internal TodoItemRepository ()
		{

		}

		private class TrelloCard
		{
			public string Id { get; set; }

			public int IdShort { get; set; }

			public string Name { get; set;}

			public bool IsClosed { get; set;}

			public string BoardId { get; set;}

			public string ListId { get; set;}

			public string[] MembersAssignedIds { get; set;}

			public string[] MembersVotedIds { get; set;}

			public DateTime LastActivity { get; set; }

			public string Url { get; set; }

			public string ShortUrl { get; set; }

			public DateTime? Due { get; set; }

			public bool IsSubscribed { get; set; }

			public TrelloCard ()
			{

			}
		}
	}
}

