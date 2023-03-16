using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using grimbil_ef.dbContext;
using System.Diagnostics.Eventing.Reader;
using grimbil_ef.Models;
using grimbil_backend.services;

namespace grimbil_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly GrimbildbContext _context;
        public UserController(GrimbildbContext context) {
            _context = context;
        }
        [HttpPost("create")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult Create([FromBody] User user)
        {
            if (user== null || user.Useremail == null || user.Userpassword == null)
            {
                return BadRequest("user cannot be null");
            }
            if(_context.Users.Select(x=> x.Useremail).Contains(user.Useremail))
            {
                return BadRequest("user already exists");
            }
            user.Userpassword=Hashingservice.GetHashString(user.Userpassword);
            _context.Users.Add(user);
            _context.SaveChanges();
            
            return Ok(string.Format("user {0} is created", user.Useremail));
        }
    }
}
