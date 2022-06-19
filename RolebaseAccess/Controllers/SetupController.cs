using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RolebaseAccess.Data;

namespace RolebaseAccess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupController : ControllerBase
    {
        private readonly ApiDbContext  _apiDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<SetupController> _logger;

        public SetupController(ApiDbContext apiDbContext,UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager,ILogger<SetupController> logger)
        {
            _apiDbContext = apiDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllRoles")]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            //check if the role exists
            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if(!roleExist)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

                if(roleResult.Succeeded)
                {
                    _logger.LogInformation($"The role {roleName} has been added successfully");
                    return Ok(new {
                        result = $"The role {roleName} has been added successfully"
                    });
                }
                else
                {
                    _logger.LogInformation($"The role {roleName} has not been added");
                    return Ok(new {
                        result = $"The role {roleName} has not been added"
                    });  
                }
            }

            return BadRequest(new {error = "Role already exist"});
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        [Route("AddUserToRole")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            //check if user exist
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                _logger.LogInformation($"The user with the email {email} does not exist");
                return BadRequest(new {
                    error = "User does not exist"
                });
            }

            //check if the role exists
            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if(!roleExist)
            {
                _logger.LogInformation($"The role {roleName} does not exist");
                return BadRequest(new {
                    error = "Role does not exist"
                });
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok(new {
                   result = "Role assignment success" 
                });
            }
            else
            {
                _logger.LogInformation($"Role assignment failed");
                return BadRequest(new {
                    error = "Role assignment failed"
                });
            }
        }

        [HttpGet]
        [Route("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                _logger.LogInformation($"The user with the email {email} does not exist");
                return BadRequest(new {
                    error = "User does not exist"
                });
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        [HttpPost]
        [Route("RemoveUserFromRole")]
        public async Task<IActionResult> RemoveUserFromRole(string email, string roleName)
        {
            //check if user exist
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                _logger.LogInformation($"The user with the email {email} does not exist");
                return BadRequest(new {
                    error = "User does not exist"
                });
            }

            //check if the role exists
            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if(!roleExist)
            {
                _logger.LogInformation($"The role {roleName} does not exist");
                return BadRequest(new {
                    error = "Role does not exist"
                });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if(result.Succeeded)
            {
                return Ok(new {
                    result = "User has been removed from role"
                });
            }
            else
            {
                return BadRequest(new {
                    error = "Failed to remove user from role"
                });
            }

        }
    }
}