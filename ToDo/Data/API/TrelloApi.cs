using System;
using MonoTouch.Foundation;
using System.Threading.Tasks;
using MonoTouch.UIKit;
using System.Web;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
//using Newtonsoft.Json;
using RestSharp;

namespace ToDo
{
	public class TrelloApi : RestClient
	{


		public const string AppKey = "7e2934188f1515e28053c375751c0076";

		const string defaultUserKey = "trelloUserKey";
		String AuthenticationToken {
			get
			{
				return NSUserDefaults.StandardUserDefaults.StringForKey (defaultUserKey);
			}
			set
			{
				NSUserDefaults.StandardUserDefaults.SetString (value, defaultUserKey);
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			}
		}

		Action<UIViewController> _presentAuthenticationViewController;

		public bool IsAuthenticated{
			get { return !string.IsNullOrEmpty (AuthenticationToken); }
			set { if(!value) AuthenticationToken = ""; }
		}

		/*public async HttpContent GetRequest(Uri request){
			bool auth = await this.EnsureAuthenticated ();

			if (auth) {
				HttpClient client = new HttpClient ();
				var asyncResponse = await client.GetAsync ();

				if (asyncResponse.StatusCode != System.Net.HttpStatusCode.OK) {
					return asyncResponse.Content;
				} else if (asyncResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
					this.IsAuthenticated = false;
					return null;
				} else {
					return asyncResponse;
				}
			} else {
				return null;
			}
		}*/

		/*public async Task<t> GetRequest<t>(string resourceUri) where t : new()
		{
			bool auth = await this.EnsureAuthenticated ();

			if (auth) {
				TaskCompletionSource<t> tcs = new TaskCompletionSource<t> ();
				this.ExecuteAsGet<t>(new RestRequest(resourceUri), (result) => {
					tcs.SetResult(result);
				});
				return tcs.Task;
			} else {
				return new t();
			}
		}*/

		public Task<bool> EnsureAuthenticated(){
			if (!string.IsNullOrEmpty (AuthenticationToken)){
				this.Authenticator = new SimpleAuthenticator ("key", AppKey, "token", AuthenticationToken);
				var task = new Task<bool>(() => true);
				task.Start();
				return task;
			}
			else {
				TaskCompletionSource<bool> promise = new TaskCompletionSource<bool>();
				_presentAuthenticationViewController(new TrelloAuthenticationViewController(
					(token) => {
						AuthenticationToken = token;
						this.Authenticator = new SimpleAuthenticator ("key", AppKey, "token", AuthenticationToken);
						promise.SetResult(true);
					}, 
					() => promise.SetResult(false))
				);
				promise.Task.Start();
				return promise.Task;
			}
		}

		public TrelloApi (Action<UIViewController> presentAuthenticationViewController) : base("https://api.trello.com/1")
		{
			_presentAuthenticationViewController = presentAuthenticationViewController;
		}
	}
}

