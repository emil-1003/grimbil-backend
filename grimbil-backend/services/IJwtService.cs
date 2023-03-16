using grimbil_ef.Models;
using System.IdentityModel.Tokens.Jwt;

namespace grimbil_backend.services
{
    public interface IJwtService
    {
        public string CreateToken(User user);
        public JwtSecurityToken DecodeToken(string jwt);

    }
}
