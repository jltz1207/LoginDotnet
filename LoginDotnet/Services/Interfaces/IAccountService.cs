using LoginDotnet.Models.Dtos;
using LoginDotnet.Models.Entities;

namespace LoginDotnet.Services.Interfaces
{
    public interface IAccountService
    {
        Task<List<User>> GetUsers();
        Task<Object> Register(RegisterDto userDto, string ipAddress);
        Task<Object> Login(LoginDto loginDto, string ipAddress); // added ipAddress parameter

    }
}
