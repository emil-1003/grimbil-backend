using grimbil_backend.Models;
using grimbil_backend.services;
using grimbil_ef.dbContext;
using grimbil_ef.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg;
using System.ComponentModel.Design;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

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
        [HttpGet("allposts")]
        public IActionResult GetAllPosts()
        {           
            return Ok(JsonConvert.SerializeObject(_context.Posts));
        }
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("createpost")]
        public IActionResult CreatePost([FromBody]PostDto post)
        {
           
            if (string.IsNullOrEmpty(Request.Headers.Authorization))
            {
                BadRequest("Token doesn't exist");
            }
            var user = GetUser(Request.Headers.Authorization);
            _context.Posts.Add(new Post { Postid =0, Userid = user.Userid,Title =post.title,Description=post.content, User= user});
            _context.SaveChanges();
                return Ok(); 
            
          
        }
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("CreateComment")]
        public IActionResult CreateComment([FromBody]CommentDto comment)
        {
            if (string.IsNullOrEmpty(Request.Headers.Authorization))
            {
                BadRequest("Token doesn't exist");
            }
            var user = GetUser(Request.Headers.Authorization);
            Post post = _context.Posts.Where(x => x.Postid == comment.PostId).FirstOrDefault();
            _context.Comments.Add(new Comment { Post = post,Postid = comment.PostId, Commentid = 0, Userid = user.Userid, Comment1 = comment.Content });
            
            _context.SaveChanges();
            return Ok();
        }
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("CreateRating")]
        public IActionResult CreateRate([FromBody]RateDto rating)
        {
            if (string.IsNullOrEmpty(Request.Headers.Authorization))
            {
                BadRequest("Token doesn't exist");
            }
            var user = GetUser(Request.Headers.Authorization);
            Post post = _context.Posts.Where(x => x.Postid == rating.PostId).FirstOrDefault();
            _context.Ratings.Add(new Rating { Postid = rating.PostId, Rating1 = rating.Rating, Ratingid = 0, Post = post, User = user });
            _context.SaveChanges();
            return Ok();
        }
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("CreateImages")]
        public IActionResult UploadImages([FromBody]ImageDto images)
        {
            var post = _context.Posts.Where(x => x.Postid == images.PostId).FirstOrDefault();
            foreach (string image in images.Base64Images) {
                _context.Pictures.Add(new Picture { Picture1 = Convert.FromBase64String(image),Pictureid=0, Postid=images.PostId, Post = post});

            }
            _context.SaveChanges();
            return Ok();
        }
        [HttpGet("GetPostImages")]
        public IActionResult GetImages(int postid)
        {
           Post post =  _context.Posts.Include("Pictures").Where(x=> x.Postid == postid).FirstOrDefault();
            
            
            return  Ok(post.Pictures.FirstOrDefault());
        }
        private User GetUser(string Auth)
        {
            var jwt = _jwtService.DecodeToken(Auth);
            var userEmail = jwt.Claims.Where(x => x.Type == "userEmail").FirstOrDefault();
            return _context.Users.Where(x => x.Useremail == userEmail.Value).FirstOrDefault();
        }
    }
}
