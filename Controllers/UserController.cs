using DAMApi.Models.Entities;
using DAMApi.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DAMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepository;
        public UserController(ILogger<UserController> logger,
            IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userId is not null)
            {
                var user = await _userRepository.GetUsersByIdAsync(userId);
                if (user is not null)
                {
                    if (user.UserName == username)
                    {
                        var userProfile = new 
                        {
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            FullName = user.FullName,
                            UserName = user.UserName,
                        };
                        return Ok(ApiResponse<object>.SuccessResponse(
                            userProfile, 
                            "User profile fetched successfully"
                        ));
                    }
                    return NotFound(ApiResponse<object>.FailureResponse(
                        "User profile does not match the authenticated user",
                        StatusCodes.Status404NotFound
                    ));
                }
            }
            return BadRequest(ApiResponse<object>.FailureResponse("User profile could not be fetched"));
        }
    }
}
