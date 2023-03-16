using grimbil_backend.services;
using grimbil_ef.dbContext;
using grimbil_ef.Models;
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
        private readonly IJwtService _jwtservice;
        
        public AuthController(GrimbildbContext context, IJwtService jwtService) {
            _context = context;
            _jwtservice = jwtService;
        }
        [HttpGet("Login")]
        public IActionResult Login(string useremail, string password)
        {
            if (!_context.Users.Select(x => x.Useremail).Contains(useremail))
            {
                return BadRequest("Useremail does not exist");
            }
           var user = _context.Users.Where(x => x.Useremail == useremail).FirstOrDefault();
            if (user.Usertype == 2)
            {
                return BadRequest("User has been blocked by moderator");
            }
            if (user.Userpassword!= password)
            {
                return BadRequest("Incorrect password");
            }
            var token = _jwtservice.CreateToken(user);
            Response.Headers.Authorization = token;

            Response.Cookies.Append("jwt", token);
            return Ok("User logged in successfully"+ Response.Headers.Authorization);
        }
        [HttpGet("RefreshToken")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult RefreshToken()
        {
            string token="";
            if(!string.IsNullOrEmpty( Request.Headers.Authorization))
            {
                token=Request.Headers.Authorization;
            }else if (!string.IsNullOrEmpty(Request.Cookies["jwt"]))
            {
                token = Request.Cookies["jwt"];
            }
            else { BadRequest("token doesn't exist"); }
            var jwt =_jwtservice.DecodeToken(token);
            var useremail = jwt.Claims.Where(x => x.Type == "userEmail").FirstOrDefault().Value;
            var user = _context.Users.Where(x => x.Useremail == useremail).FirstOrDefault();
            var userpassword = jwt.Claims.Where(x => x.Type == "password").FirstOrDefault().Value;
            if (user.Userpassword == userpassword)
            {
                var jwttoken = _jwtservice.CreateToken(user);
                Response.Headers.Authorization = token;
                Response.Cookies.Append("jwt", token);

                return Ok("user token refreshed");
            }
            return BadRequest("json web token credentials are not users credentials");

                
        }
    }
}
