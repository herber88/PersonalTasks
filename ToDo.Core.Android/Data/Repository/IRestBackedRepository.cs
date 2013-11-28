using System;
using RestSharp;

namespace ToDo.Core.iOS
{
	public interface IRepository
	{
		IRestClient Api { get;set;}
	}
}

