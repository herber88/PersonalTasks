using System;
using System.Runtime.Serialization;
using SQLite;

namespace ToDo
{
	[DataContract]
	public class TrelloBoard
	{
		[DataMember(Name="id")]
		[PrimaryKey]
		public string Id { get; set; }

		[DataMember(Name="name")]
		public string Name { get; set;}



		public TrelloBoard ()
		{


		}
	}
}

