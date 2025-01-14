﻿using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using MyFace.Services;
using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;
using MyFace.Helpers;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("/posts")]
    public class PostsController : ControllerBase
    {    
        private readonly IPostsRepo _posts;

        private readonly IUsersRepo _users;

        private readonly PasswordAuthorization _auth;
        public PostsController(IPostsRepo posts, IUsersRepo users, PasswordAuthorization auth)
        {
            _posts = posts;
            _users = users;
            _auth = auth;
        }
        
        [HttpGet("")]
        public ActionResult<PostListResponse> Search([FromQuery] PostSearchRequest searchRequest)
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

            var posts = _posts.Search(searchRequest);
            var postCount = _posts.Count(searchRequest);
            return PostListResponse.Create(searchRequest, posts, postCount);
        }

        [HttpGet("{id}")]
        public ActionResult<PostResponse> GetById([FromRoute] int id)
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

            var usernamePasswordArray = UsernamePasswordHelper.GetUsernamePassword(authHeader);

            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];
            var UserId = _users.GetByUsername(username).Id;

            if (!_auth.UserNamePasswordMatch(username, password))
            {
                return Unauthorized("Username and password do not match");
            }


            //call the function
            var post = _posts.Create(newPost, UserId);

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
            var currentPost = _posts.GetById(id);

            if (!_auth.IsCorrectUser(currentPost.UserId, username))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "You are not allowed to update a post for a different user"
                );
            }

            var post = _posts.Update(id, update);
            return new PostResponse(post);
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
            var currentPost = _posts.GetById(id);

            if (!_auth.IsCorrectUser(currentPost.UserId, username))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "You are not allowed to delete a post for a different user"
                );
            }
            
            _posts.Delete(id);
            return Ok();
        }
    }
}