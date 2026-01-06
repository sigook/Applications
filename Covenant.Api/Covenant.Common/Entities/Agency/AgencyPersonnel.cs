namespace Covenant.Common.Entities.Agency
{
    public class AgencyPersonnel
    {
        private AgencyPersonnel() { }
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }
        public Agency Agency { get; private set; }
        public Guid AgencyId { get; private set; }
        public User User { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsPrimary { get; private set; }

        public void SetToNotPrimary() => IsPrimary = false;
        public void SetToPrimary() => IsPrimary = true;

        public static AgencyPersonnel CreatePrimary(Guid agencyId, Guid userId, string name = null) => Create(agencyId, userId, true, name);

        public static AgencyPersonnel Create(Guid agencyId, Guid userId, bool isPrimary = false, string name = null) =>
            new AgencyPersonnel { AgencyId = agencyId, UserId = userId, Name = name, IsPrimary = isPrimary };

        public static AgencyPersonnel CreatePrimary(Guid agencyId, User user, string name = null) => Create(agencyId, user, true, name);

        public static AgencyPersonnel Create(Guid agencyId, User user, bool isPrimary, string name = null) =>
            new AgencyPersonnel
            {
                AgencyId = agencyId,
                User = user ?? throw new ArgumentNullException(nameof(user)),
                UserId = user.Id,
                Name = name ?? user.Email?.Split("@")[0],
                IsPrimary = isPrimary
            };
    }
}