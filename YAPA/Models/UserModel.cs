using Microsoft.AspNetCore.Identity;
using YAPA.Models.Entities;

namespace YAPA.Models
{
    public class UserModel : IdentityUser<int>
    {
        public List<RefreshTokenModel> RefreshTokens { get; set; } = new();
    }
}
