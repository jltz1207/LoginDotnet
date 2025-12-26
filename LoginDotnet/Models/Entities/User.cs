using Microsoft.AspNetCore.Identity;

namespace LoginDotnet.Models.Entities
{
    public class User: IdentityUser
    {
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
