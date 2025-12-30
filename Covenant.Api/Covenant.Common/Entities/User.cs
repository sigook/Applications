namespace Covenant.Common.Entities
{
    public class User
    {
        private User() 
        { 
        }

        public User(CvnEmail email, Guid id = default)
        {
            Email = email;
            Id = id == default ? Guid.NewGuid() : id;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Email { get; private set; }
        public bool Enabled { get; set; } = true;
        public DateTime LastModified { get; set; }

        public void UpdateEmail(CvnEmail email)
        {
            Email = email;
            LastModified = DateTime.Now;
        }

        public void InactiveUser()
        {
            Enabled = false;
            LastModified = DateTime.Now;
        }
    }
}