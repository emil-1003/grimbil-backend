using grimbil_backend.services;
using grimbil_ef.dbContext;
using grimbil_ef.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace grimbil_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController :ControllerBase
    {
        private readonly GrimbildbContext _context;
        private readonly IJwtService _jwtService;
        public PostsController(GrimbildbContext context, IJwtService jwt) {
            _context = context;
            _jwtService = jwt;
        }

        [HttpGet("allposts")]
        public IActionResult GetAllPosts()
        {           
            return Ok(JsonConvert.SerializeObject(_context.Posts));
        }
        [HttpPost("createpost")]
        public IActionResult CreatePost([FromBody]Post post)
        {
            return Ok();
        }
    }
}
