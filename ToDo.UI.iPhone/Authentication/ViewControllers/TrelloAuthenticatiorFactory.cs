using System;
using System.Threading.Tasks;
using MonoTouch.Foundation;
using RestSharp;
using MonoTouch.UIKit;
using ToDo.Core.iOS;

namespace ToDo.UI.iPhone
{
	public class TrelloAuthenticatiorFactory : IAuthenticatorFactory
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

		IAuthenticator Authenticator{
			get{
				return new SimpleAuthenticator ("key", AppKey, "token", AuthenticationToken);
			}
		}

		Action<UIViewController> _presentAuthenticationViewController;

		public TrelloAuthenticatiorFactory (Action<UIViewController> presentAuthenticationViewController)
		{
			_presentAuthenticationViewController = presentAuthenticationViewController;
		}

		public Task<IAuthenticator> GetAuthenticatior(){
			if (!string.IsNullOrEmpty (AuthenticationToken)){
				var task = new Task<IAuthenticator>(() => Authenticator);
				task.Start ();
				return task;
			}
			else {
				TaskCompletionSource<IAuthenticator> promise = new TaskCompletionSource<IAuthenticator>();
				_presentAuthenticationViewController(new TrelloAuthenticationViewController(
					(token) => {
						AuthenticationToken = token;
						promise.SetResult(Authenticator);
					}, 
					() => promise.SetResult(null))
				);
				return promise.Task;
			}
		}
	}
}

