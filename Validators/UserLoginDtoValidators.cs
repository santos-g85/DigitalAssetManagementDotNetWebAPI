using DAMApi.DTOs;
using DAMApi.Models.DTOs;
using FluentValidation;
using System.Data;

namespace DAMApi.Validators
{
    public class UserLoginDtoValidators : AbstractValidator<UserLoginDto>
    {
        public UserLoginDtoValidators() 
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required!")
                .Matches(expression: @"^[a-zA-Z0-9._%+-]+@[a-zA-Z.-]+\.[a-zA-Z]{2,}$").WithMessage("Email must be in a valid format.")
                .MaximumLength(50).WithMessage("Email can not excede 50 characters!");

            RuleFor(x => x.Password)
               .NotEmpty().WithMessage("Password is required.")
               .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$").WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")
               .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
               .MaximumLength(16).WithMessage("Password must not exceed 16 characters.");
        }
    }
}
