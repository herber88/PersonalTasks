using System;
using MonoTouch.UIKit;
using System.Threading.Tasks;
using System.Collections.Generic;
using RestSharp;

namespace ToDo
{
	public class TrelloListRepository
	{
		TrelloApi trelloApi;

		public TrelloListRepository (Action<UIViewController> presentAuthenticationViewController)
		{
			trelloApi = new TrelloApi (presentAuthenticationViewController);
		}

		public Task<List<TrelloList>> GetAllListsForBoard(TrelloBoard board)
		{
			TaskCompletionSource<List<TrelloList>> tcs = new TaskCompletionSource<List<TrelloList>>();
			trelloApi.ExecuteAsync<List<TrelloList>> (new RestRequest("members/my/boards/" + board.Id + "/lists"), (response) => {
				tcs.SetResult(response.Data);
			});
			return tcs.Task;
		}

		public Task<TrelloList> GetById(string id)
		{
			TaskCompletionSource<TrelloList> tcs = new TaskCompletionSource<TrelloList>();
			trelloApi.ExecuteAsync<TrelloList> (new RestRequest("lists/" + id), (response) => {
				tcs.SetResult(response.Data);
			});
			return tcs.Task;
		}
	}
}

