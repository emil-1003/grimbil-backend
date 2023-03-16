using grimbil_backend.Models;
using grimbil_ef.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace grimbil_backend.services
{
    public class Jwtservice : IJwtService
    {
        private readonly IConfiguration _configuration;
        public Jwtservice(IConfiguration configuration) {
            _configuration = configuration;
        }
        public JwtSecurityToken DecodeToken(string jwt)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(jwt);
            
        }
        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("userEmail", user.Useremail),
                new Claim("userType", Convert.ToString(user.Usertype)),
                new Claim("password", user.Userpassword)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:Key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);//SecurityAlgorithms.HmacSha512Signature

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            
            return jwt;
        }
       
    }
}
