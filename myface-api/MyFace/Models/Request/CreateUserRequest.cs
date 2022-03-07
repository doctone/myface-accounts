using System.ComponentModel.DataAnnotations;
using MyFace.Helpers;

namespace MyFace.Models.Request
{
    public class CreateUserRequest
    {
        [Required]
        [StringLength(70)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(70)]
        public string LastName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [StringLength(70)]
        public string Username { get; set; }

        [Required]
        [StringLength(70, MinimumLength =12, ErrorMessage = "Password should be min 12 symbols")]
        [IsValidPassword(ErrorMessage = "Password should contain at least 1 digit, 1 lower case letter and 1 upper case letter.")]
        public string Password { get; set; }
        
        public string ProfileImageUrl { get; set; }
        
        public string CoverImageUrl { get; set; }
    }
}