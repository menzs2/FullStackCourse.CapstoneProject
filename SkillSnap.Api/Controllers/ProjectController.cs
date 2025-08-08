using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSnap.Shared.Models;

namespace SkillSnap.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly SkillSnapContext _context;

        public ProjectController(SkillSnapContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProjects()
        {
            var projects = _context.Projects.ToList();
            return projects != null && projects.Any() ? Ok(projects) : NotFound("No projects found.");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public IActionResult CreateProject([FromBody] Project project)
        {
            if (project == null)
            {
                return BadRequest("Project cannot be null.");
            }

            _context.Projects.Add(project);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetProjects), new { id = project.Id }, project);
        }
    }
}
