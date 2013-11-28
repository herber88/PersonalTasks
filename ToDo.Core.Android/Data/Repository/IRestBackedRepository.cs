using System;
using RestSharp;

namespace ToDo.Core.iOS
{
	public interface IRestBackedRepository
	{
		IRestClient Api { get;set;}
	}
}

