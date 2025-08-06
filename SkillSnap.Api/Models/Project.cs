using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSnap.Api
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        public required string Title { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [ForeignKey("PortfolioUser")]
        [InverseProperty("Project")]
        public int PortfolioUserId { get; set; }

        public required PortfolioUser PortfolioUser { get; set; }
    }
}
