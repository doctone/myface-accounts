using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using MyFace.Services;
using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("/posts")]
    public class PostsController : ControllerBase
    {    
        private readonly IPostsRepo _posts;

        private readonly IUsersRepo _users;
        public PostsController(IPostsRepo posts, IUsersRepo users)
        {
            _posts = posts;
            _users = users;
        }
        
        [HttpGet("")]
        public ActionResult<PostListResponse> Search([FromQuery] PostSearchRequest searchRequest)
        {
            var posts = _posts.Search(searchRequest);
            var postCount = _posts.Count(searchRequest);
            return PostListResponse.Create(searchRequest, posts, postCount);
        }

        [HttpGet("{id}")]
        public ActionResult<PostResponse> GetById([FromRoute] int id)
        {
            var post = _posts.GetById(id);
            return new PostResponse(post);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreatePostRequest newPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var authHeader = Request.Headers["Authorization"];

            if (authHeader == StringValues.Empty)
            {
                return Unauthorized();
            }

            var authHeaderString = authHeader[0];

            // authHeader looks like "Basic {base 64 encoded string}"

            var authHeaderSplit = authHeaderString.Split(' ');
            var authType = authHeaderSplit[0];
            var encodedUsernamePassword = authHeaderSplit[1];

            var usernamePassword = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(encodedUsernamePassword)
            );

            var usernamePasswordArray = usernamePassword.Split(':');

            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];

            PasswordAuthorization auth = new PasswordAuthorization();

            if (!auth.UserNamePasswordMatch(username, password, _users))
            {
                return Unauthorized("Username and password do not match");
            }
            if (!auth.IsCorrectUser(newPost, username, _users))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "You are not allowed to create a post for a different user"
                );
            }

            //call the function
            var post = _posts.Create(newPost);

            var url = Url.Action("GetById", new { id = post.Id });
            var postResponse = new PostResponse(post);
            return Created(url, postResponse);
        }

        [HttpPatch("{id}/update")]
        public ActionResult<PostResponse> Update([FromRoute] int id, [FromBody] UpdatePostRequest update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = _posts.Update(id, update);
            return new PostResponse(post);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _posts.Delete(id);
            return Ok();
        }
    }
}