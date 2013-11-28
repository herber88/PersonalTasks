using System;
using MonoTouch.UIKit;
using System.Threading.Tasks;
using System.Collections.Generic;
using RestSharp;
using SQLite;
using MonoTouch.Foundation;
using System.Threading;
using System.Linq;

namespace ToDo
{
	public class TrelloCardRepository
	{
		TrelloApi trelloApi;
		//SQLiteAsyncConnection conn = new SQLiteAsyncConnection (Constants.Database.TrelloData);

		public List<TrelloCard> Cards = new List<TrelloCard> ();

		const string defaultUserKey = "trelloCardRepositorySyncTime";
		DateTime LastSyncTime {
			get
			{
				return DateTime.FromOADate(NSUserDefaults.StandardUserDefaults.DoubleForKey (defaultUserKey));
			}
			set
			{
				NSUserDefaults.StandardUserDefaults.SetDouble (value.ToOADate(), defaultUserKey);
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			}
		}

		public TrelloCardRepository (Action<UIViewController> presentAuthenticationViewController)
		{
			trelloApi = new TrelloApi (presentAuthenticationViewController);
			//EnsureTableExists ();
			new Timer ((a) => Refresh (), null, 0, 60000);
		}

		public async void Refresh()
		{
			Cards = await Fetch();
		}

		/*Task<CreateTablesResult> EnsureTableExists(){
			var task = conn.CreateTableAsync<TrelloCard> ();
			return task;
			//return table;
		}

		async Task<int> AddToDatabase(IEnumerable<TrelloCard> cards){
			await EnsureTableExists ();

			return await conn.InsertAllAsync (cards);
		}

		async Task<List<TrelloCard>> FetchLocal(){
			await EnsureTableExists ();

			var task = conn.Table<TrelloCard> ().ToListAsync ();
			task.ContinueWith((arg) =>  
			{
					Console.WriteLine(arg.Exception);
			});

			Cards = await task;

			return Cards;
		}*/

		Task<List<TrelloCard>> Fetch ()
		{
			TaskCompletionSource<List<TrelloCard>> tcs = new TaskCompletionSource<List<TrelloCard>> ();
			trelloApi.EnsureAuthenticated().ContinueWith((a) => 
				trelloApi.ExecuteAsync<List<TrelloCard>> (new RestRequest ("members/my/cards"), response =>  {
					LastSyncTime = DateTime.Now;
					tcs.SetResult (response.Data);
					//AddToDatabase(response.Data);
				}));
			return tcs.Task;
		}

		public async Task<List<TrelloCard>> AllTasks(bool forceRefresh = false)
		{
			if (Cards.Count > 0)
				return Cards;
			else {
				/*var localResults = await FetchLocal ();
				if(localResults.Count > 0)
					return localResults;
				else*/

				var trelloCards = await Fetch ();

				return trelloCards;//.Select(t => new Todo { Title = t.Name});
			}
 		}
	}
}