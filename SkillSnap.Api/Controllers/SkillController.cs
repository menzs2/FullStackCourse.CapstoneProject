using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSnap.Shared.Models;

namespace SkillSnap.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillController : ControllerBase
    {
        private readonly SkillSnapContext _context;

        public SkillController(SkillSnapContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetSkills()
        {
            var skills = _context.Skills.ToList();
            return skills != null && skills.Any() ? Ok(skills) : NotFound("No skills found.");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public IActionResult CreateSkill([FromBody] Skill skill)
        {
            if (skill == null)
            {
                return BadRequest("Skill cannot be null.");
            }

            _context.Skills.Add(skill);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetSkills), new { id = skill.Id }, skill);
        }
    }
}
