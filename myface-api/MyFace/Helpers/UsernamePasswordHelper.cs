using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Primitives;

namespace MyFace.Helpers
{
    public class UsernamePasswordHelper
    {
        public static string[] GetUsernamePassword(StringValues authHeader)
        {
            var authHeaderString = authHeader[0];

            // authHeader looks like "Basic {base 64 encoded string}"

            var authHeaderSplit = authHeaderString.Split(' ');
            var authType = authHeaderSplit[0];
            var encodedUsernamePassword = authHeaderSplit[1];

            var usernamePassword = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(encodedUsernamePassword)
            );

            return usernamePassword.Split(':');

        }
    }
}