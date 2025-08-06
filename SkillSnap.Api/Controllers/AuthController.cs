using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SkillSnap.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfigurationSection _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration.GetSection("Jwt");
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LoginAsync([FromBody] Microsoft.AspNetCore.Identity.Data.LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Invalid login request.");
            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Issue JWT token
                    var user = await _userManager.FindByEmailAsync(request.Email);
                    if (user == null)
                    {
                        return BadRequest("User not found.");
                    }
                    var JwtToken = await GenerateJwtTokenAsync(user);

                    // Set the JWT token in a secure cookie
                    // Ensure the cookie is HttpOnly, Secure, and SameSite
                    Response.Cookies.Append(
                        "JwtToken",
                        JwtToken,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddHours(1)
                        });
                    return Ok(new { Token = JwtToken, Message = "Login successful." });
                }

                return BadRequest("Invalid username or password.");
            }
            return BadRequest(ModelState);
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestWithRole request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Invalid registration request.");
            }
            ApplicationUser user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                // if no role is specified, default to "User"
                if (string.IsNullOrEmpty(request.Role))
                {
                    request.Role = "User";
                }
                if (await _roleManager.RoleExistsAsync(request.Role))
                {
                    await _userManager.AddToRoleAsync(user, request.Role);
                }
            }
            
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return CreatedAtAction(nameof(Register), new { Email = request.Email }, new { Message = "User registered successfully." });
        }

        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Role, (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "User") // Default to User role if none assigned
        };

            var issuer = _configuration["Issuer"];
            var audience = _configuration["Audience"];
            var secret = _configuration["Secret"];

            if (string.IsNullOrEmpty(issuer))
                throw new InvalidOperationException("JWT Issuer configuration is missing.");
            if (string.IsNullOrEmpty(audience))
                throw new InvalidOperationException("JWT Audience configuration is missing.");
            if (string.IsNullOrEmpty(secret))
                throw new InvalidOperationException("JWT Secret configuration is missing.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)); // Use a secure key from config
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class RegisterRequestWithRole
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string? Role { get; set; } // Optional role for registration
    }

}