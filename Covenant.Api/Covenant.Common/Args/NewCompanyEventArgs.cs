using System;

namespace Covenant.Common.Args
{
	public class NewCompanyEventArgs : EventArgs
	{
		public NewCompanyEventArgs(Guid agencyId,  string companyFullName, string companyEmail,bool active)
		{
			AgencyId = agencyId;
			Active = active;
			CompanyFullName = companyFullName;
			CompanyEmail = companyEmail;
		}

		public Guid AgencyId { get; }
		public bool Active { get; }
		public string CompanyFullName { get; }
		public string CompanyEmail { get; }
	}
}