using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MyFace.Models.Request;

using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Response;
using MyFace.Models.Database;
using MyFace.Repositories;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;

namespace MyFace.Helpers
{
    public class PasswordAuthorization
    {
        public bool PassAuthorization(CreatePostRequest newpost)
        {
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

            User user;
            try
            {
                user = _users.GetByUsername(username);
            }
            catch (InvalidOperationException e)
            {
                return Unauthorized("The username/password combination does not match");
            }

            // hash user's password and check it
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(user.Salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            if (hashed != user.HashedPassword)
            {
                return Unauthorized("The username/password combination does not match");
            }

            if (user.Id != newPost.UserId)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "You are not allowed to create a post for a different user"
                );
            }
        }
    }
}