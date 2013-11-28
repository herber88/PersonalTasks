using System;
using System.Runtime.Serialization;
using SQLite;

namespace ToDo
{
	[DataContract]
	public class TrelloCard
	{
		[DataMember(Name="id")]
		public string Id { get; set; }

		[DataMember(Name="idShort")]
		public int IdShort { get; set; }

		[DataMember(Name="name")]
		public string Name { get; set;}

		[DataMember(Name="closed")]
		public bool IsClosed { get; set;}

		[DataMember(Name="idBoard")]
		public string BoardId { get; set;}

		[DataMember(Name="idList")]
		public string ListId { get; set;}

		[DataMember(Name="idMembers")]
		public string[] MembersAssignedIds { get; set;}

		[DataMember(Name="idMembersVoted")]
		public string[] MembersVotedIds { get; set;}

		[DataMember(Name="dateLastActivity")]
		public DateTime LastActivity { get; set; }

		[DataMember(Name="url")]
		public string Url { get; set; }

		[DataMember(Name="shortUrl")]
		public string ShortUrl { get; set; }

		[DataMember(Name="due")]
		public DateTime? Due { get; set; }

		[DataMember(Name="subscribed")]
		public bool IsSubscribed { get; set; }

		public TrelloCard ()
		{

		}
	}
}

