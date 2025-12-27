using LoginDotnet.Models.Dtos;
using LoginDotnet.Models.Entities;

namespace LoginDotnet.Services.Interfaces
{
    public interface IAccountService
    {
        Task<List<User>> GetUsers();
        Task<Object> Register(RegisterDto userDto);
        Task<Object> Login(LoginDto loginDto); // added login signature

    }
}
