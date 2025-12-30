using System;
using System.Collections.Generic;

namespace Covenant.Common.Args
{
	public class WorkerLicenseExpiredEventArgs : EventArgs
	{
		public string AgencyEmail { get; set; }
		public List<WorkerInformation> Workers { get; set; } = new List<WorkerInformation>();

		public class WorkerInformation
		{
			public string WorkerFullName { get; set; }
			public string WorkerEmail { get; set; }
			public string MobileNumber { get; set; }
			public long NumberId { get; set; }
			public string LicenseDescription { get; set; }
			public string LicenseNumber { get; set; }
			public DateTime? Expires { get; set; }
		}
	}
}