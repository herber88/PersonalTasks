using System;
using RestSharp;

namespace ToDo.Core.iOS
{
	public class TrelloRepositoryFactory
	{
		IAuthenticator _authenticator;
		public TrelloRepositoryFactory(IAuthenticator authenticator)
		{
			_authenticator = authenticator;
		}

		public t GetInstance<t>() where t : IRestBackedRepository, new()
		{
			var repo = new t();
			repo.Api = new RestClient("https://api.trello.com/1");
			repo.Api.Authenticator = _authenticator;
			return repo;
		}
	}
}