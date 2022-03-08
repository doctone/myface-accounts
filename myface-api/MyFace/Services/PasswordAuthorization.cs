using MyFace.Models.Request;
using MyFace.Models.Database;
using MyFace.Repositories;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;

namespace MyFace.Services
{
    public interface IAuth
    {
        public bool UserNamePasswordMatch(CreatePostRequest newPost);

        public bool IsCorrectUser(CreatePostRequest newPost, string username);
    }
    public class PasswordAuthorization
    {
        private readonly IUsersRepo _users;

        public PasswordAuthorization(IUsersRepo users)
        {
            _users = users;
        }
        public bool UserNamePasswordMatch(string username, string password)
        {
            User user;
            try
            {
                user = _users.GetByUsername(username);
            }
            catch (InvalidOperationException e)
            {
                return false;
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
                return false;
            }
            return true;
        }

        public bool IsCorrectUser(int id, string username)
        {
            User user = _users.GetByUsername(username);
            return user.Id == id;

        }
    }
}