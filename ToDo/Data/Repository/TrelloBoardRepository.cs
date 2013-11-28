using System;
using MonoTouch.UIKit;
using System.Threading.Tasks;
using System.Collections.Generic;
using RestSharp;

namespace ToDo
{
	public class TrelloBoardRepository
	{
		TrelloApi trelloApi;

		public TrelloBoardRepository (Action<UIViewController> presentAuthenticationViewController)
		{
			trelloApi = new TrelloApi (presentAuthenticationViewController);
		}

		public Task<List<TrelloBoard>> GetAllBoards()
		{
			TaskCompletionSource<List<TrelloBoard>> tcs = new TaskCompletionSource<List<TrelloBoard>> ();
			trelloApi.EnsureAuthenticated().ContinueWith((a) => {
				trelloApi.ExecuteAsync<List<TrelloBoard>>(new RestRequest ("members/my/boards", Method.GET), (result) => {
					tcs.SetResult(result.Data);
				});
			});

			return tcs.Task;
		}
	}
}

