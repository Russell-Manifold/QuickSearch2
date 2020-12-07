using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickSearch2.Models
{
	public class AToken
	{
		[AutoIncrement, PrimaryKey]
		public int ID { get; set; }
		public string FullToken { get; set; }
	}
}