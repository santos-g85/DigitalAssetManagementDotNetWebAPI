using DAMApi.DTOs;
using DAMApi.Models.DTOs;
using DAMApi.Models.Entities;
using DAMApi.Repository.Interfaces;
using DAMApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using FluentValidation;

namespace DAMApi.Services.Implementation
{
    public class AuthService(ILogger<AuthService> logger,
                       IUserRepository userRepository,
                       IJWTService jWTService,
                       IValidator<UserRegisterDto> registervalidator,
                       IValidator<UserLoginDto> loginvalidator,
                       GoogleApiService googleApiService,
                       IFolderRepositroy folderRepositroy) : IAuthService
    {
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IJWTService _jWTService = jWTService;
        private readonly IValidator<UserRegisterDto> _registerValidator = registervalidator;
        private readonly IValidator<UserLoginDto> _loginValidator = loginvalidator;
        private readonly GoogleApiService _googleApiService = googleApiService;
        private readonly IFolderRepositroy _folderRepositroy= folderRepositroy;



        public async Task<TokenResponseDto?> LoginAsync(UserLoginDto request)
        {
            var passwordhasher = new PasswordHasher<UserModel>();
            var validationresult =  await _loginValidator.ValidateAsync(request);
            if(validationresult.IsValid)
            {
                var result = await _userRepository.GetUsersByEmailIdAsync(request.Email);
                if (result is not null)
                {
                    if (passwordhasher.VerifyHashedPassword(null, result.PasswordHash,
                    request.Password) == PasswordVerificationResult.Failed)
                    {
                        return null;
                    }
                    return await _jWTService.CreateTokenResponseAsync(result); ;
                }
                return null;
            }
            return null;
            
        }

        public async Task<UserModel?> RegisterAsync(UserRegisterDto request)
        {
            var passwordHasher = new PasswordHasher<UserModel>();
            var validationResult = await _registerValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var problemDetails = new HttpValidationProblemDetails(validationResult.ToDictionary())
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation Failed",
                    Detail = "One or more validation errors occured",
                    Instance = "/auth/register"
                };
                _logger.LogWarning("Validation failed: {Errors}", problemDetails);
                return null;
            }
            var existingUser = await _userRepository.GetUsersByEmailIdAsync(request.Email);
            if (existingUser is not null)
            {
                _logger.LogWarning("User already exists!");
                return null;
            }
            var newUser = new UserModel
            {
                UserName = request.Email.Replace("@gmail.com", " ").ToUpper(),
                FullName = string.Join(" ",
                           request.FullName
                           .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                           .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower())
                            ),
                Email = request.Email,
                PasswordHash = passwordHasher.HashPassword(null, request.Password),
                PhoneNumber = request.PhoneNumber.ToString()
            };

            var result = await _userRepository.AddUserAsync(newUser);

            if (result is null)
            {
                _logger.LogWarning("User registration failed!");
                return null;
            }
            FolderModel? folderresults = await FolderTreeCreation(result);
            _logger.LogInformation($"User with email {result.Email} registered successfully.");
            return result;
        }

        private async Task<FolderModel?> FolderTreeCreation(UserModel? result)
        {
            var createfolder = await _googleApiService.CreateFolderTree(result.UserName!);

            var userFolderRecord = new FolderModel
            {
                FolderName = createfolder.ToString(),
                FolderPath = createfolder.ToString()
            };
            var folderresults = await _folderRepositroy.CreateFolderRecord(userFolderRecord);
            return folderresults;
        }
    }
}

