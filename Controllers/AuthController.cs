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
        public AuthController(ILogger<AuthController> logger,
                              IAuthService authService,
                              IJWTService jWTService)
        {
            _logger = logger;
            _authService = authService;
            _jWTService = jWTService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Registration failed: {Message}", result.Message);
                    return StatusCode(
                        result.StatusCode,
                        ApiResponse<object>.FailureResponse(
                            result.Message ?? "Registration failed.",
                            result.StatusCode
                        )
                    );
                }
                var usedata = new
                {
                    Email = result.Data?.Email,
                    Username = result.Data?.UserName,
                };
                return StatusCode(
                    result.StatusCode,
                    ApiResponse<object>.SuccessResponse(
                        usedata,
                        result.Message ?? "User registered successfully and folder created!"
                    )
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.FailureResponse("Internal server error.", StatusCodes.Status500InternalServerError)
                );
            }
        }


        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] UserLoginDto request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                if (!result.IsSuccess )
                {
                    _logger.LogWarning("login failed. invalid data.");
                    return BadRequest(ApiResponse<object>.FailureResponse("Invalid Credetial", StatusCodes.Status400BadRequest));
                }
                var tokens = new TokenResponseDto { 
                    AccessToken = result.Data.AccessToken,
                    RefreshToken = result.Data.RefreshToken
                };
                return Ok(ApiResponse<object>.SuccessResponse(tokens, "User logged in successfully!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loggin-in the user.");
                return StatusCode(500, ApiResponse<object>.FailureResponse("Internal server error.", StatusCodes.Status500InternalServerError));
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
                    return Unauthorized(ApiResponse<TokenResponseDto>.FailureResponse("invalid credentails", StatusCodes.Status400BadRequest));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while refreshing the token.");
                return StatusCode(500,ApiResponse<TokenResponseDto>.FailureResponse($"{ex.Message}",StatusCodes.Status500InternalServerError));
            }
        }
    }
}
