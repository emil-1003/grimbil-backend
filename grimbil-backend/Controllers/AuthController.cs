using grimbil_backend.Models;
using grimbil_backend.services;
using grimbil_ef.dbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            Response.Headers.Authorization = token;

            Response.Cookies.Append("token",user.Useremail);
            Response.Cookies.Append("jwt", token);
            return Ok(Response.Headers.Authorization);
        }
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("RefreshToken")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        
        public IActionResult RefreshToken()
        {
            var jwt =_jwtService.DecodeToken(Request.Headers.Authorization);
            var useremail = jwt.Claims.Where(x => x.Type == "userEmail").FirstOrDefault().Value;
            var user = _context.Users.Where(x => x.Useremail == useremail).FirstOrDefault();
            var userpassword = jwt.Claims.Where(x => x.Type == "password").FirstOrDefault().Value;
            if (user.Userpassword == userpassword)
            {
                var jwttoken = _jwtService.CreateToken(user);
                Response.Headers.Authorization = jwttoken;
                return Ok("user token refreshed");
            }
            return BadRequest("json web token credentials are not users credentials");

                
        }
    }
}
