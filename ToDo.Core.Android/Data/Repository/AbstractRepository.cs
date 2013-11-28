using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;

namespace ToDo.Core.iOS
{
	public abstract class AbstractRepository<t> : IRestBackedRepository
	{
		public abstract IRestClient Api { get; set;}
		public AbstractRepository()
		{
		}

		public abstract Task<IEnumerable<t>> GetAllForCurrentUser();
		public abstract Task<bool> Add(params t[] entity);
		public abstract Task<bool> Remove(params t[] entity);
		public abstract Task<bool> Sync ();
	}
}

