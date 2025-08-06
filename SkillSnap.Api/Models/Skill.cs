using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSnap.Api
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
        public required PortfolioUser PortfolioUser { get; set; }
    }
}
