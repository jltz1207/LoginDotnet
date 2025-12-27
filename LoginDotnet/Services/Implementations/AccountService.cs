using LoginDotnet.Infra.Security;
using LoginDotnet.Models.Dtos;
using LoginDotnet.Models.Entities;
using LoginDotnet.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoginDotnet.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private UserManager<User> _userManager;
        private IConfiguration _config;
        public AccountService(UserManager<User> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<List<User>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return users;
        }

        public async Task<Object> Register(RegisterDto userDto)
        {
            var user = await _userManager.FindByEmailAsync(userDto.Email);
            if (user != null)
            {
                throw new Exception("User with this email already exists");
            }
            var newUser = new User
            {
                FullName = $"{userDto.FirstName} {userDto.LastName}",
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                UserName = userDto.Email,
                DateOfBirth = userDto.DateOfBirth,
                PhoneNumber = userDto.PhoneNumber
            };
            var result = await _userManager.CreateAsync(newUser, userDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"User creation failed: {errors}");
            }
            var token = await TokenService.CreateToken(newUser, _userManager, _config);
            return new
            {
                Token = token,
                User = newUser
            }
            ;
        }

        public async Task<Object> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                throw new Exception("Invalid credentials");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordValid)
            {
                throw new Exception("Invalid credentials");
            }

            var token = await TokenService.CreateToken(user, _userManager, _config);
            return new
            {
                Token = token,
                User = user
            };
        }


    }
}
