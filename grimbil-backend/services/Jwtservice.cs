using grimbil_backend.Models;
using grimbil_ef.dbContext;
using grimbil_ef.Models;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
            var jwthandler = new JwtSecurityTokenHandler();
            
                return jwthandler.ReadJwtToken(jwt.Replace("Bearer", "").Trim());
            
        }
        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("userEmail", user.Useremail),
                new Claim("userType", Convert.ToString(user.Usertype)),
                new Claim("userId",Convert.ToString(user.Userid)),
                new Claim("password", user.Userpassword)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:Key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);//SecurityAlgorithms.HmacSha512Signature
            
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                issuer : _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                signingCredentials: creds); ;

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            var jwt2 = new JwtSecurityTokenHandler();
                jwt2.ValidateToken(jwt, new TokenValidationParameters
            {

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:Key").Value)),
                ValidateIssuer = false,

                ValidateLifetime = true,
                ValidateAudience = false
            }, out SecurityToken validatedToken);
            return jwt;
        }
        
       

    }
}
