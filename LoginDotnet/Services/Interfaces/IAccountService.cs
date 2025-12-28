using LoginDotnet.Models.Dtos;
using LoginDotnet.Models.Entities;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace LoginDotnet.Services.Interfaces
{
    public interface IAccountService
    {
        Task<List<User>> GetUsers();
        Task<Object> Register(RegisterDto userDto);
        Task<Object> Login(LoginDto loginDto);
        Task<string> UploadProfilePic(IFormFile userProfile);
        Task<string> GetProfileImage();
    }
}
