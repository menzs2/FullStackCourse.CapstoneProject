using System.ComponentModel.DataAnnotations;

namespace SkillSnap.Shared.Models
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

        public List<Project> Projects { get; set; } = new List<Project>();

        public List<Skill> Skills { get; set; } = new List<Skill>();
    }
}
