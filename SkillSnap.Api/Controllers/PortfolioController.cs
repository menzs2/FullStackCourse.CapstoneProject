using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Shared.Models;

namespace SkillSnap.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly SkillSnapContext _context;

        public PortfolioController(SkillSnapContext context, IMemoryCache cache)
        {
            _cache = cache;
            _context = context;
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetPortfolios()
        {
            if (_cache.TryGetValue("Portfolios", out List<PortfolioUser>? portfolios))
            {
                return Ok(portfolios);
            }

            portfolios = _context.PortfolioUsers
                .Include(p => p.Projects)
                .Include(p => p.Skills)
                .ToList();

            if (portfolios == null || !portfolios.Any())
            {
                return NotFound("No portfolios found.");
            }

            _cache.Set("Portfolios", portfolios);
            return Ok(portfolios);
        }
        
        [HttpGet("{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetPortfolio(int userId)
        {
            if (!_cache.TryGetValue($"Portfolio_{userId}", out PortfolioUser? portfolioUser))
            {
                portfolioUser = _context.PortfolioUsers
                    .Include(p => p.Projects)
                    .Include(p => p.Skills)
                    .FirstOrDefault(p => p.Id == userId);
                if (portfolioUser == null)
                {
                    return NotFound();
                }
                _cache.Set($"Portfolio_{userId}", portfolioUser);
            }
            return Ok(portfolioUser);
        }
    }
}
