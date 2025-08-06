using Microsoft.AspNetCore.Mvc;
using SkillSnap.Shared.Models;


namespace SkillSnap.Api
{
[Route("api/[controller]")]
[ApiController]
    public class SeedController : ControllerBase
    {
        private readonly SkillSnapContext _context;

        public SeedController(SkillSnapContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult SeedData()
        {
            if (_context.PortfolioUsers.Any())
            {
                return BadRequest("Sample data already exists.");
            }
            var user = new PortfolioUser
            {
                Name = "Jordan Developer",
                Bio = "Full-stack developer passionate about learning new tech.",
                ProfilePictureUrl = "https://example.com/images/jordan.png",
                Email = "jd@example.com",

            };
            _context.PortfolioUsers.Add(user);
            _context.SaveChanges();
            user.Projects.Add(new Project
            {
                Title = "Portfolio Website",
                Description = "A personal portfolio website to showcase my skills and projects.",
                PortfolioUserId = user.Id,
                PortfolioUser = user
            });
            user.Skills.Add(new Skill
            {
                Name = "C#",
                Level = "Advanced",
                PortfolioUserId = user.Id,
                PortfolioUser = user
            });
            user.Skills.Add(new Skill
            {
                Name = "Blazor",
                Level = "Intermediate",
                PortfolioUserId = user.Id,
                PortfolioUser = user
            });
            _context.SaveChanges();
            return Ok("Sample data inserted.");
        }
    }
}
