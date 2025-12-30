namespace Covenant.Common.Models.Agency
{
    public class AgencyContactInformationModel
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string MobileNumber { get; set; }
        public int? OfficeNumberExt { get; set; }
        public string OfficeNumber { get; set; }
        public string Email { get; set; }

        protected bool Equals(AgencyContactInformationModel other)
        {
            return string.Equals(Title, other.Title, StringComparison.CurrentCulture) && string.Equals(FirstName, other.FirstName, StringComparison.CurrentCulture) && string.Equals(MiddleName, other.MiddleName, StringComparison.CurrentCulture) && string.Equals(LastName, other.LastName, StringComparison.CurrentCulture) && string.Equals(Position, other.Position, StringComparison.CurrentCulture) && string.Equals(OfficeNumber, other.OfficeNumber, StringComparison.CurrentCulture) && string.Equals(MobileNumber, other.MobileNumber, StringComparison.CurrentCulture) && string.Equals(Email, other.Email, StringComparison.CurrentCulture);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is AgencyContactInformationModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Title != null ? StringComparer.CurrentCulture.GetHashCode(Title) : 0);
                hashCode = (hashCode * 397) ^ (FirstName != null ? StringComparer.CurrentCulture.GetHashCode(FirstName) : 0);
                hashCode = (hashCode * 397) ^ (MiddleName != null ? StringComparer.CurrentCulture.GetHashCode(MiddleName) : 0);
                hashCode = (hashCode * 397) ^ (LastName != null ? StringComparer.CurrentCulture.GetHashCode(LastName) : 0);
                hashCode = (hashCode * 397) ^ (Position != null ? StringComparer.CurrentCulture.GetHashCode(Position) : 0);
                hashCode = (hashCode * 397) ^ (OfficeNumber != null ? StringComparer.CurrentCulture.GetHashCode(OfficeNumber) : 0);
                hashCode = (hashCode * 397) ^ (MobileNumber != null ? StringComparer.CurrentCulture.GetHashCode(MobileNumber) : 0);
                hashCode = (hashCode * 397) ^ (Email != null ? StringComparer.CurrentCulture.GetHashCode(Email) : 0);
                return hashCode;
            }
        }

    }
}