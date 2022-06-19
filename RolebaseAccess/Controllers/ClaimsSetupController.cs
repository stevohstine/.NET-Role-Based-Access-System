using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RolebaseAccess.Configuration;
using RolebaseAccess.Data;

namespace RolebaseAccess.Controllers
{
    public class ClaimsSetupController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        private readonly TokenValidationParameters _tokenValidationParams;
        private readonly ApiDbContext _apiDbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ClaimsSetupController> _logger;

        public ClaimsSetupController(UserManager<IdentityUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor, TokenValidationParameters tokenValidationParams, ApiDbContext apiDbContext, RoleManager<IdentityRole> roleManager,ILogger<ClaimsSetupController> logger)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
            _tokenValidationParams = tokenValidationParams;
            _apiDbContext = apiDbContext;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllClaims")]
        public async Task<IActionResult> GetAllClaims(string email)
        {
             // Check if the user exist
            var user = await _userManager.FindByEmailAsync(email);

            if(user == null) // User does not exist
            {
                _logger.LogInformation($"The user with the {email} does not exist");
                return BadRequest(new {
                    error = "User does not exist"
                });
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            return Ok(userClaims);
        }

        [HttpPost]
        [Route("AddClaimsToUser")]
        public async Task<IActionResult> AddClaimsToUser(string email, string claimName, string claimValue)
        {
            // Check if the user exist
            var user = await _userManager.FindByEmailAsync(email);

            if(user == null) // User does not exist
            {
                _logger.LogInformation($"The user with the {email} does not exist");
                return BadRequest(new {
                    error = "User does not exist"
                });
            }

            var userClaim = new Claim(claimName, claimValue);

            var result = await _userManager.AddClaimAsync(user, userClaim);

            if(result.Succeeded)
            {
                return Ok(new {
                    resutl = $"User {user.Email} has a claim {claimName} added to them"
                });
            }

            return BadRequest(new {
                error = $"Unable to add claim {claimName} to the user {user.Email}"
            });

        }
    }
}