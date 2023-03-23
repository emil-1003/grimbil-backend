using grimbil_backend.Models;
using grimbil_backend.services;
using grimbil_ef.dbContext;
using grimbil_ef.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [AllowAnonymous]
        [HttpGet("AllPosts")]
        public IActionResult GetAllPosts()
        {           
            return Ok(JsonConvert.SerializeObject(_context.Posts));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("AllUserposts")]
        public IActionResult GetAlluserpostsPosts()
        {
            User user = GetUser(Request.Headers.Authorization);
            return Ok(JsonConvert.SerializeObject(_context.Posts.Where(x=> x.Userid == user.Userid)));
        }
        [AllowAnonymous]
        [HttpGet("GetPost")]
        public IActionResult GetPost(int postid)
        {
            double averageRating = 0;
            int amRatings = 0;
            foreach (var rating in _context.Ratings.Where(x => x.Postid == postid))
            {
                amRatings++;
                averageRating += rating.Rating1;
            }
            averageRating /= amRatings;
            var post = _context.Posts.Include(x => x.Pictures)
                .Include(z => z.Comments)
                .Include(y => y.Ratings)
                .Include(u => u.User)
                .Where(post => post.Postid == postid).Select(p => new
                {
                    p.Postid,
                    p.Title,
                    rating = averageRating,
                    p.Comments,
                    p.Description,
                    p.Ratings,
                    User = new
                    {
                        p.User.Userid,
                        p.User.Useremail
                    },
                    p.Pictures
                })
                .FirstOrDefault();

            return Ok(post);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("CreatePost")]
        public IActionResult CreatePost([FromBody]PostDto post)
        {
           
            if (string.IsNullOrEmpty(Request.Headers.Authorization))
            {
                BadRequest("Token doesn't exist");
            }
            var user = GetUser(Request.Headers.Authorization);
            _context.Posts.Add(new Post { Postid =0, Userid = user.Userid,Title =post.Title,Description=post.Content});
            
            _context.SaveChanges();
             var postid = _context.Posts.Where(x=> x.Userid == user.Userid && x.Title == post.Title&& x.Description == post.Content).FirstOrDefault().Postid;
            foreach (string picture in post.Pictures)
            {
                _context.Pictures.Add(new Picture { Postid = postid, Pictureid = 0, Picture1 = picture });
            }
            _context.SaveChanges();
            return Ok(); 
            
          
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("CreateComment")]
        public IActionResult CreateComment([FromBody]CommentDto comment)
        {
            if (string.IsNullOrEmpty(Request.Headers.Authorization))
            {
                BadRequest("Token doesn't exist");
            }
            var user = GetUser(Request.Headers.Authorization);
            Post post = _context.Posts.Where(x => x.Postid == comment.PostId).FirstOrDefault();
            _context.Comments.Add(new Comment {Postid = comment.PostId, Commentid = 0, Userid = user.Userid, Comment1 = comment.Content });
            
            _context.SaveChanges();
            return Ok();
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("CreateRating")]
        public IActionResult CreateRate([FromBody]RateDto rating)
        {
            if (string.IsNullOrEmpty(Request.Headers.Authorization))
            {
                BadRequest("Token doesn't exist");
            }
            var user = GetUser(Request.Headers.Authorization);
            Post post = _context.Posts.Where(x => x.Postid == rating.PostId).FirstOrDefault();
            _context.Ratings.Add(new Rating { Postid = rating.PostId, Rating1 = rating.Rating, Ratingid = 0 });
            _context.SaveChanges();
            return Ok();
        }
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("DeletePost")]
        public IActionResult DeletePost(int postid) {
            var user = GetUser(Request.Headers.Authorization);
            if (!_context.Posts.Any(x=> x.Postid == postid))
            {
                return BadRequest("Post doesn't exist");
            }
            if (_context.Posts.SingleOrDefault(x=> x.Postid== postid).Userid == user.Userid||user.Usertype == 1)
            {
                _context.Posts.Remove(_context.Posts.Single(x => x.Postid == postid));
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest("user doesn't own post");
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("DeleteComment")]
        public IActionResult DeleteComment(int postid)
        {
            var user = GetUser(Request.Headers.Authorization);
            if (!_context.Comments.Any(x => x.Postid == postid))
            {
                return BadRequest("Post doesn't exist");
            }
            if (_context.Comments.SingleOrDefault(x => x.Postid == postid).Userid == user.Userid || user.Usertype == 1)
            {
                _context.Comments.Remove(_context.Comments.Single(x => x.Postid == postid));
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest("user doesn't own post");
        }
        private User GetUser(string Auth)
        {
            var jwt = _jwtService.DecodeToken(Auth);
            var userEmail = jwt.Claims.Where(x => x.Type == "userEmail").FirstOrDefault();
            return _context.Users.Where(x => x.Useremail == userEmail.Value).FirstOrDefault();
        }
    }
}
