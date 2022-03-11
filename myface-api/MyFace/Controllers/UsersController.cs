using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using MyFace.Services;
using Microsoft.Extensions.Primitives;
using MyFace.Helpers;
using Microsoft.AspNetCore.Http;
using MyFace.Models.Database;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepo _users;

        private readonly PasswordAuthorization _auth;

        public UsersController(IUsersRepo users, PasswordAuthorization auth)
        {
            _users = users;
            _auth = auth;
        }
        
        [HttpGet("")]
        public ActionResult<UserListResponse> Search([FromQuery] UserSearchRequest searchRequest)
        {
            var authHeader = Request.Headers["Authorization"];

            if (authHeader == StringValues.Empty)
            {
                return Unauthorized();
            }

            var usernamePasswordArray = UsernamePasswordHelper.GetUsernamePassword(authHeader);

            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];

            if (!_auth.UserNamePasswordMatch(username, password))
            {
                return Unauthorized("Username and password do not match");
            }

            var users = _users.Search(searchRequest);
            var userCount = _users.Count(searchRequest);
            return UserListResponse.Create(searchRequest, users, userCount);
        }

        [HttpGet("{id}")]
        public ActionResult<UserResponse> GetById([FromRoute] int id)
        {
            var authHeader = Request.Headers["Authorization"];

            if (authHeader == StringValues.Empty)
            {
                return Unauthorized();
            }

            var usernamePasswordArray = UsernamePasswordHelper.GetUsernamePassword(authHeader);

            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];

            if (!_auth.UserNamePasswordMatch(username, password))
            {
                return Unauthorized("Username and password do not match");
            }

            var user = _users.GetById(id);
            return new UserResponse(user);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateUserRequest newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = _users.Create(newUser);

            var url = Url.Action("GetById", new { id = user.Id });
            var responseViewModel = new UserResponse(user);
            return Created(url, responseViewModel);
        }

        [HttpPatch("{id}/update")]
        public ActionResult<UserResponse> Update([FromRoute] int id, [FromBody] UpdateUserRequest update)
        {
            var authHeader = Request.Headers["Authorization"];

            if (authHeader == StringValues.Empty)
            {
                return Unauthorized();
            }

            var usernamePasswordArray = UsernamePasswordHelper.GetUsernamePassword(authHeader);

            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];

            if (!_auth.UserNamePasswordMatch(username, password))
            {
                return Unauthorized("Username and password do not match");
            }
        

            var currentUser = _users.GetByUsername(username);
            
            if (!_auth.IsCorrectUser(id, username) && currentUser.Role != AuthRole.admin)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "Only admins can update users other than your own"
                );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _users.Update(id, update);
            return new UserResponse(user);
        }

        [HttpPatch("{id}/update-role")]
        
        public ActionResult<UserResponse> UpdateRole([FromRoute] int id, [FromBody] UpdateRoleRequest update)
        {
            var authHeader = Request.Headers["Authorization"];

            if (authHeader == StringValues.Empty)
            {
                return Unauthorized();
            }

            var usernamePasswordArray = UsernamePasswordHelper.GetUsernamePassword(authHeader);

            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];

            if (!_auth.UserNamePasswordMatch(username, password))
            {
                return Unauthorized("Username and password do not match");
            }
            var currentUser = _users.GetByUsername(username);
            
            if (currentUser.Role != AuthRole.admin)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "Only admins can update users' roles"
                );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _users.UpdateRole(id, update);
            return new UserResponse(user);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
             var authHeader = Request.Headers["Authorization"];

            if (authHeader == StringValues.Empty)
            {
                return Unauthorized();
            }

            var usernamePasswordArray = UsernamePasswordHelper.GetUsernamePassword(authHeader);

            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];

            if (!_auth.UserNamePasswordMatch(username, password))
            {
                return Unauthorized("Username and password do not match");
            }

            var currentUser = _users.GetByUsername(username);

            if (!_auth.IsCorrectUser(id, username) && currentUser.Role != AuthRole.admin)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "Only admins can delete users other than your own"
                );
            }
            _users.Delete(id);
            return Ok();
        }
    }
}