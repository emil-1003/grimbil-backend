using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using grimbil_ef.dbContext;
using System.Diagnostics.Eventing.Reader;
using grimbil_ef.Models;
using grimbil_backend.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Reflection.Metadata.Ecma335;

namespace grimbil_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly GrimbildbContext _context;
        private readonly IJwtService _jwtService;
        public UserController(GrimbildbContext context, IJwtService jwtService) {
            _context = context;
            _jwtService = jwtService;
        }
        [AllowAnonymous]
        [HttpPost("Create")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult Create([FromBody] User user)
        {
            if (user == null || user.Useremail == null || user.Userpassword == null)
            {
                return BadRequest("user cannot be null");
            }
            if (_context.Users.Select(x => x.Useremail).Contains(user.Useremail))
            {
                return BadRequest("user already exists");
            }
            user.Userpassword = Hashingservice.GetHashString(user.Userpassword);
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(string.Format("user {0} is created", user.Useremail));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers(){
            User user = GetUser(Request.Headers.Authorization);
            if (user.Usertype != 1)
            {
                return Unauthorized("You are not a Moderator");
            }
            return Ok(_context.Users.Select(x=> new {x.Userid, x.Useremail, x.Usertype}));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("BlockUser")]
        public IActionResult BlockUser(int userid)
        {
            User user = GetUser(Request.Headers.Authorization);
            if (user.Usertype != 1)
            {
                return Unauthorized("You are not a Moderator");
            }
            if(_context.Users.Any(x=> x.Userid == userid))
            {
                _context.Users.Single(x => x.Userid == userid).Usertype = 2;
                _context.SaveChanges();
                return Ok("User Has been blocked");
            }
            return BadRequest("user doesn't exist");
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("UnblockUser")]
        public IActionResult UnblockUser(int userid)
        {
            User user = GetUser(Request.Headers.Authorization);
            if (user.Usertype != 1)
            {
                return Unauthorized("You are not a Moderator");
            }
            if (_context.Users.Any(x => x.Userid == userid))
            {
                _context.Users.Single(x => x.Userid == userid).Usertype = 0;
                _context.SaveChanges();
                return Ok("User Has been unblocked");
            }
            return BadRequest("user doesn't exist");
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("DeleteUser")]
        public IActionResult DeleteUser(int userId)
        {
            User user = GetUser(Request.Headers.Authorization);
            if (user.Usertype != 1|| user.Userid== userId)
            {
                return Unauthorized("You are not allowed");
            }
            if (_context.Users.Any(x=> x.Userid == userId))
            {
                _context.Users.Remove(_context.Users.Single(x => x.Userid==userId));
                _context.SaveChanges();
                return Ok("User deleted");
            }
            return BadRequest("User doesn't exist");
        }

        private User GetUser(string Auth)
        {
            var jwt = _jwtService.DecodeToken(Auth);
            var userEmail = jwt.Claims.Where(x => x.Type == "userEmail").FirstOrDefault();
            return _context.Users.Where(x => x.Useremail == userEmail.Value).FirstOrDefault();
        }
        
    }
}
