using grimbil_backend.Models;
using grimbil_backend.services;
using grimbil_ef.dbContext;
using grimbil_ef.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Reflection.Metadata.Ecma335;

namespace grimbil_backend.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class AuthController: ControllerBase
    {
        private readonly GrimbildbContext _context;
        private readonly IJwtService _jwtService;
        
        public AuthController(GrimbildbContext context, IJwtService jwtService) {
            _context = context;
            _jwtService = jwtService;
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody]LoginDto login)
        {
            if (!_context.Users.Select(x => x.Useremail).Contains(login.UserEmail))
            {
                return BadRequest("Useremail does not exist");
            }
           var user = _context.Users.Where(x => x.Useremail == login.UserEmail).FirstOrDefault();
            if (user.Usertype == 2)
            {
                return BadRequest("User has been blocked by moderator");
            }
            if (user.Userpassword!= Hashingservice.GetHashString( login.Password))
            {
                return BadRequest("Incorrect password");
            }
            var token = _jwtService.CreateToken(user);
            //token = _jwtservice.GenerateJwtToken(user.Useremail);
            Response.Headers.Authorization = token;

            Response.Cookies.Append("token",user.Useremail);
            Response.Cookies.Append("jwt", token);
            return Ok("User logged in successfully"+ Response.Headers.Authorization);
        }
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("RefreshToken")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        
        public IActionResult RefreshToken()
        {
            string token="";
            if (!string.IsNullOrEmpty(Request.Headers.Authorization))
            {
                token = Request.Headers.Authorization;
            }
            else if (!string.IsNullOrEmpty(Request.Cookies["jwt"]))
            {
                token = Request.Cookies["jwt"];
            }
            else { BadRequest("token doesn't exist"); }
            var jwt =_jwtService.DecodeToken(token);
            var useremail = jwt.Claims.Where(x => x.Type == "userEmail").FirstOrDefault().Value;
            var user = _context.Users.Where(x => x.Useremail == useremail).FirstOrDefault();
            var userpassword = jwt.Claims.Where(x => x.Type == "password").FirstOrDefault().Value;
            if (user.Userpassword == userpassword)
            {
                var jwttoken = _jwtService.CreateToken(user);
                Response.Headers.Authorization = token;
                Response.Cookies.Append("jwt", token);

                return Ok("user token refreshed");
            }
            return BadRequest("json web token credentials are not users credentials");

                
        }
    }
}
