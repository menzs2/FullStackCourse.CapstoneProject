using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Shared.Models;

namespace SkillSnap.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly SkillSnapContext _context;
        private readonly IMemoryCache _inMemoryStoreCache;
        private const string cacheKey = "projectsCache";

        public ProjectController(SkillSnapContext context, IMemoryCache inMemoryStoreCache)
        {
            _context = context;
            _inMemoryStoreCache = inMemoryStoreCache;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public IActionResult GetProjects()
        {

            if (_inMemoryStoreCache.TryGetValue(cacheKey, out List<Project> cachedProjects))
            {
                return Ok(cachedProjects);
            }

            var projects = _context.Projects.ToList();
            if (projects != null && projects.Any())
            {
                _inMemoryStoreCache.Set(cacheKey, projects);
                return Ok(projects);
            }

            return NotFound("No projects found.");
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
            // Clear the cache after adding a new project
            _inMemoryStoreCache.Remove(cacheKey);
            return CreatedAtAction(nameof(GetProjects), new { id = project.Id }, project);
        }
    }
}
