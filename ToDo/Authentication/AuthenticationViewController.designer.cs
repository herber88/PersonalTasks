// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace ToDo
{
	[Register ("AuthenticationViewController")]
	partial class AuthenticationViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIWebView uiWebView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIWebView webView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (uiWebView != null) {
				uiWebView.Dispose ();
				uiWebView = null;
			}

			if (webView != null) {
				webView.Dispose ();
				webView = null;
			}
		}
	}
}
