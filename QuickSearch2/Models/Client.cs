using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickSearch2.Models
{
	public class Client
	{
		public string MemberStatus { get; set; }
		public string PolicyType { get; set; }
		public string MemberNo { get; set; }
		public DateTime Policy_Inception_date { get; set; }
		public DateTime CancellationDate { get; set; }
		public string Title { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string IDNumber { get; set; }
		public string PassportNumber { get; set; }
		public string TelHome { get; set; }
		public string TelWork { get; set; }
		public string TelOther { get; set; }
		public string MainEmailAddress { get; set; }
		public string AltEmailAddress { get; set; }
		public string ResidentialAddressComplexNo { get; set; }
		public string ResidentialAddressComplexName { get; set; }
		public string ResidentialAddressStreetNo { get; set; }
		public string ResidentialAddressStreetName { get; set; }
		public string ResidentialAddressSuburb { get; set; }
		public string ResidentialAddressPOcode { get; set; }
		public string ResidentialAddressProvince { get; set; }
		public bool HomeAssistPanicSOS { get; set; }
	}
}