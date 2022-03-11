using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;
using MyFace.Helpers;
using MyFace.Services;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("feed")]
    public class FeedController : ControllerBase
    {
        private readonly IPostsRepo _posts;
        private readonly IUsersRepo _users;
        private readonly PasswordAuthorization _auth;

        public FeedController(IPostsRepo posts, IUsersRepo users, PasswordAuthorization auth)
        {
            _posts = posts;
            _users = users;
            _auth = auth;
        }

        [HttpGet("")]
        public ActionResult<FeedModel> GetFeed([FromQuery] FeedSearchRequest searchRequest)
        {
            var authHeader = Request.Headers["Authorization"];

            if (authHeader == StringValues.Empty)
            {
                return Unauthorized();
            }

            var usernamePasswordArray = UsernamePasswordHelper.GetUsernamePassword(authHeader);

            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];
            var UserId = _users.GetByUsername(username).Id;

            if (!_auth.UserNamePasswordMatch(username, password))
            {
                return Unauthorized("Username and password do not match");
            }

            var posts = _posts.SearchFeed(searchRequest);
            var postCount = _posts.Count(searchRequest);

            return FeedModel.Create(searchRequest, posts, postCount);
        }
    }
}
