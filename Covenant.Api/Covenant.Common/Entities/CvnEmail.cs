using Covenant.Common.Functionals;
using System.Net.Mail;

namespace Covenant.Common.Entities
{
    public class CvnEmail : IEquatable<CvnEmail>
    {
        private CvnEmail(string email) => Email = email;

        public string Email { get; }

        public static Result<CvnEmail> Create(string email)
        {
            var error = $"'{email}' is not a valid email address.";
            if (string.IsNullOrWhiteSpace(email))
            {
                return Result.Fail<CvnEmail>(ResultError.Create(nameof(email), error));
            }
            try
            {
                var mailAddress = new MailAddress(email);
                return Result.Ok(new CvnEmail(mailAddress.Address));
            }
            catch (Exception)
            {
                return Result.Fail<CvnEmail>(error);
            }
        }

        public static implicit operator string(CvnEmail email) => email.Email;

        public static implicit operator CvnEmail(string email) => new(email);

        #region Equals
        public bool Equals(CvnEmail other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Email == other.Email;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CvnEmail)obj);
        }

        public override int GetHashCode() => Email != null ? Email.GetHashCode() : 0;
        public static bool operator ==(CvnEmail left, CvnEmail right) => Equals(left, right);
        public static bool operator !=(CvnEmail left, CvnEmail right) => !Equals(left, right);
        #endregion
    }
}