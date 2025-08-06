using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSnap.Api
{
    public class PortfolioUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }

        [InverseProperty("PortfolioUser")]
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        [InverseProperty("PortfolioUser")]
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}
