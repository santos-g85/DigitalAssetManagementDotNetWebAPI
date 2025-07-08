using DAMApi.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace DAMApi.Models.Entities
{
    public class UserModel : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public UserRoles UserRole { get; set; } = UserRoles.User;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
