using System;
using System.Threading.Tasks;
using RestSharp;

namespace ToDo.Core.iOS
{
	public interface IAuthenticatorFactory
	{
		Task<IAuthenticator> GetAuthenticatior ();
	}
}

