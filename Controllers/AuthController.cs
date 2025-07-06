using DAMApi.DTOs;
using DAMApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using DAMApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using DAMApi.Services.Implementation;

namespace DAMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        private readonly IJWTService _jWTService;
        private readonly GoogleApiService _googleApiService;
        public AuthController(ILogger<AuthController> logger,
                              IAuthService authService,
                              IJWTService jWTService,
                              GoogleApiService googleApiService)
        {
            _logger = logger;
            _authService = authService;
            _jWTService = jWTService;
            _googleApiService = googleApiService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);

                if (result is null)
                {
                    _logger.LogWarning("Registration failed. User already exists or invalid data.");
                    return BadRequest("User already exists or invalid data.");
                }
                await _googleApiService.CreateFolderTree(result.UserName!);
                return Ok(new { Message = "User registered successfully  and folder created!", UserId = result.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] UserLoginDto request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);

                if (result is null)
                {
                    _logger.LogWarning("login failed. invalid data.");
                    return BadRequest("invalid credential.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loggin-in the user.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost("refresh-tokens")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var result = await _jWTService.RefreshTokensAsync(request);
                if (result is null 
                    || result.AccessToken is null
                    || result.RefreshToken is null)
                {
                    _logger.LogWarning("Refresh token failed. Invalid data.");
                    return Unauthorized("Invalid refresh token!");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while refreshing the token.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize]
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            // Simulate a logout process
            return Ok("Logout successful");
        }


        [Authorize(Roles ="Admin")]
        [HttpGet("profile")]
        public IActionResult GetUserProfile()
        {
            string username = "santos";
            // Simulate fetching user profile
            return Ok(username);

        }
    }
}
