using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Member : BaseAuditableEntity<int>
    {
        [MaxLength(30)]
        public string? FullName { get; set; }

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
        public double Contributed { get; set; }

        [MaxLength(50)]
        public string? Email { get; set; }
        public int? CurrentTeamId { get; set; }
        public Team? CurrentTeam { get; set; }
        public DateTimeOffset? JoinedTeamAt { get; set; }
        public int? OldTeam { get; set; }

        [MaxLength(5)]
        public string? Gender { get; set; }
        public DateTime? DoB { get; set; }

        [MaxLength(150)]
        public string? Address { get; set; }

        public string? AccountId { get; set; }
        public Account? Account { get; set; }
    }
}
