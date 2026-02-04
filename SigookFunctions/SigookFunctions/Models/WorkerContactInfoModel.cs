namespace SigookFunctions.Models
{
	public class WorkerContactInfoModel
	{
		public string WorkerId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }

		public string FullName => $"{FirstName} {LastName}";
		
		public bool IsModelValid()
		{
			if (string.IsNullOrEmpty(Email)) return false;
			if (!Email.Contains("@")) return false;
			if (string.IsNullOrEmpty(FirstName)) return false;
			if (string.IsNullOrEmpty(WorkerId)) return false;
			return true;
		}

		public override string ToString() => $"Email:{Email},FirstName={FirstName},LastName={LastName},WorkerId={WorkerId}";
	}
}