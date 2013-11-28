using System;
using System.Threading.Tasks;
using System.Web;
using MonoTouch.UIKit;
using RestSharp;

namespace ToDo
{
	public abstract class BaseApi : RestClient
	{
		public override IRestResponse Execute (IRestRequest request)
		{
			return base.Execute (request);
		}
	}
}

