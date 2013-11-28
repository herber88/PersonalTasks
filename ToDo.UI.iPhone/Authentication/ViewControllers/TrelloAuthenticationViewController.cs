using System;
using MonoTouch.Foundation;
using System.Text.RegularExpressions;

namespace ToDo.UI.iPhone
{
	public class TrelloAuthenticationViewController : BaseAuthenticationViewController
	{
		const string AuthenticatedUrl = "https://trello.com/1/token/approve";
		const string DeniedUrl = "https://trello.com";
		const string StartUrl = "https://trello.com/1/authorize?key=" + TrelloAuthenticatiorFactory.AppKey + "&name=&expiration=never&response_type=token&scope=read,write";

		public TrelloAuthenticationViewController(Action<string> receivedAuthenticationToken, Action cancelled) : base(new Uri(StartUrl), receivedAuthenticationToken, cancelled, false)
		{}

		protected override void RequestFinished (NSUrlRequest request)
		{
			if(request.Url.AbsoluteString.TrimEnd('/').Equals(AuthenticatedUrl)){
				var responseBody = webView.EvaluateJavascript("document.body.innerHTML");
				var text = new NSString(responseBody, NSStringEncoding.ASCIIStringEncoding);
				var regex = new Regex("<pre>.+</pre>", RegexOptions.Singleline);
				var match = regex.Match(text);
				if(match.Success){
					var result = match.Captures[0].Value;
					result = result.Replace("<pre>", "").Replace("</pre>", "").Replace("\n", "").Replace(" ", "");
					_authenticationTokenReceived(result);
					this.DismissViewController(true, null);
				}
			}
		}

		protected override void RequestStarted (NSUrlRequest request)
		{
			if(request.Url.AbsoluteString.TrimEnd('/').Equals(DeniedUrl))
			{
				_cancelled ();
				this.DismissViewController (true, null);
			}
		}
	}
}

