using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSnap.Shared.Models
{
    public class Skill
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Level { get; set; }
        [ForeignKey("PortfolioUser")]
        [InverseProperty("Skills")]
        public int PortfolioUserId { get; set; }
        public PortfolioUser? PortfolioUser { get; set; }
    }
}
