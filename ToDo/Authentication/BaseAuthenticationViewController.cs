using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace ToDo
{
	public abstract class BaseAuthenticationViewController : UIViewController
	{
		protected UIWebView webView;
		protected UINavigationBar navigationBar;

		Uri _startUri;
		protected Action _cancelled;
		protected Action<string> _authenticationTokenReceived;
		bool _addCancelButton;

		const int navigationBarHeight = 44 + 20;

		protected abstract void RequestFinished (NSUrlRequest request);
		protected abstract void RequestStarted (NSUrlRequest request);

		public BaseAuthenticationViewController (Uri startUri, Action<string> authenticationTokenReceived, Action cancelled, bool addCancelButton)
		{
			_startUri = startUri;
			_cancelled = cancelled;
			_authenticationTokenReceived = authenticationTokenReceived;
			_addCancelButton = addCancelButton;
		}

		public override void ViewWillAppear (bool animated)
		{
			webView.LoadRequest (new NSUrlRequest (new NSUrl (_startUri.AbsoluteUri)));
			base.ViewWillAppear (animated);
		}

		public class WebViewDelegate : UIWebViewDelegate{
			Action<NSUrlRequest> _loadStarted, _loadFinished;
			public WebViewDelegate(Action<NSUrlRequest> loadStarted, Action<NSUrlRequest> loadFinished){
				_loadStarted = loadStarted;
				_loadFinished = loadFinished;
			}

			public override bool ShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
			{
				_loadStarted (request);
				return true;
			}

			public override void LoadingFinished (UIWebView webView)
			{
				_loadFinished (webView.Request);
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			webView = new UIWebView (new RectangleF(0, (_addCancelButton) ? navigationBarHeight : 0, View.Frame.Width, (_addCancelButton) ? View.Frame.Height - navigationBarHeight : View.Frame.Height));
			webView.Delegate = new WebViewDelegate(RequestStarted, RequestFinished);

			if (!_addCancelButton) {
				this.View.AddSubview (webView);
			} else {
				var cancelButton = new UIBarButtonItem (UIBarButtonSystemItem.Cancel);
				cancelButton.Clicked += (object sender, EventArgs e) => {
					_cancelled();
					this.DismissViewController(true, null);
				};

				var navigationItem = new UINavigationItem {
					LeftBarButtonItem = cancelButton
				};

				navigationBar = new UINavigationBar (new RectangleF (0, 0, View.Frame.Width, navigationBarHeight));
				navigationBar.PushNavigationItem (navigationItem, false);

				this.View.AddSubviews (navigationBar, webView);
			}
		}
	}
}

