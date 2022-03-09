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
    [Route("/interactions")]
    public class InteractionsController : ControllerBase
    {
        private readonly IInteractionsRepo _interactions;

        private readonly PasswordAuthorization _auth;

        public InteractionsController(IInteractionsRepo interactions, PasswordAuthorization auth)
        {
            _interactions = interactions;
            _auth = auth;
        }
    
        [HttpGet("")]
        public ActionResult<ListResponse<InteractionResponse>> Search([FromQuery] SearchRequest search)
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

            var interactions = _interactions.Search(search);
            var interactionCount = _interactions.Count(search);
            return InteractionListResponse.Create(search, interactions, interactionCount);
        }

        [HttpGet("{id}")]
        public ActionResult<InteractionResponse> GetById([FromRoute] int id)
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
            
            var interaction = _interactions.GetById(id);
            return new InteractionResponse(interaction);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateInteractionRequest newUser)
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

            var usernamePasswordArray = UsernamePasswordHelper.GetUsernamePassword(authHeader);

            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];

            if (!_auth.UserNamePasswordMatch(username, password))
            {
                return Unauthorized("Username and password do not match");
            }

            if (!_auth.IsCorrectUser(newUser.UserId, username))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "You are not allowed to interact with posts on behalf of a different user"
                );
            }
        
            var interaction = _interactions.Create(newUser);

            var url = Url.Action("GetById", new { id = interaction.Id });
            var responseViewModel = new InteractionResponse(interaction);
            return Created(url, responseViewModel);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            Interaction i = _interactions.GetById(id);
            
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

            if (!_auth.IsCorrectUser(i.UserId, username))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "You are not allowed to interact with posts on behalf of a different user"
                );
            }

            _interactions.Delete(id);
            return Ok();
        }
    }
}
