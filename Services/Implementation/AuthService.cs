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

        public async Task<ServiceResult<TokenResponseDto?>> LoginAsync(UserLoginDto request)
        {
            var passwordhasher = new PasswordHasher<UserModel>();
            var validationresult =  await _loginValidator.ValidateAsync(request);
            if(!validationresult.IsValid)
            {
                _logger.LogWarning("login validation failed");
                var errors = string.Join("; ", validationresult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<TokenResponseDto?>.Failure($"login validation failure: {errors}", StatusCodes.Status400BadRequest);
            }
            var dbuser = await _userRepository.GetUsersByEmailIdAsync(request.Email);
            if (dbuser is not null)
            {
                if (passwordhasher.VerifyHashedPassword(null, dbuser.PasswordHash,
                request.Password) == PasswordVerificationResult.Failed)
                {
                    return ServiceResult<TokenResponseDto?>.Failure("password didn't match", StatusCodes.Status406NotAcceptable);
                }
                var tokenresult = await _jWTService.CreateTokenResponseAsync(dbuser);
                return ServiceResult<TokenResponseDto?>.Success( tokenresult,"token generated successfully", StatusCodes.Status200OK);
            }
            return ServiceResult<TokenResponseDto?>.Failure("user doesnot exitst!",StatusCodes.Status404NotFound);
        }

        public async Task<ServiceResult<UserModel?>> RegisterAsync(UserRegisterDto request)
        {
            var passwordHasher = new PasswordHasher<UserModel>();
            var validationResult = await _registerValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            { 
               _logger.LogWarning(" login Validation failed");
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<UserModel?>.Failure($"Validation failed: {errors}", 400);
            }
            var existingUser = await _userRepository.GetUsersByEmailIdAsync(request.Email);
            if (existingUser is not null)
            {
                _logger.LogWarning("User already exists!");
                return ServiceResult<UserModel?>.Failure("user already exists!",StatusCodes.Status400BadRequest);
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
                return ServiceResult<UserModel?>.Failure("User registration failed!",StatusCodes.Status500InternalServerError);
            }
            FolderModel? folderresults = await FolderTreeCreation(result);
            _logger.LogInformation($"User with email {result.Email} registered successfully.");
            return ServiceResult<UserModel?>.Success(result, $"user with email {result.Email} created!", StatusCodes.Status200OK);
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

