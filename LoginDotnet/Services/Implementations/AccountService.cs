using LoginDotnet.Data;
using LoginDotnet.Infra.Security;
using LoginDotnet.Models.Dtos;
using LoginDotnet.Models.Entities;
using LoginDotnet.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LoginDotnet.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private UserManager<User> _userManager;
        private IConfiguration _config;
        private ApplicationContext _context;
        public AccountService(UserManager<User> userManager, IConfiguration config, ApplicationContext context)
        {
            _userManager = userManager;
            _config = config;
            _context = context;
        }

        public async Task<List<User>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return users;
        }

        public async Task<object> Register(RegisterDto userDto, string ipAddress)
        {
            // create log early but postpone Save until outcome is known
            var userLog = new UserActivityLog
            {
                Email = userDto.Email,
                ActivityType = ActivityType.Register,
                Timestamp = CommonService.GenerateHKTime(),
                IpAddress = ipAddress 
            };


            var existing = await _userManager.FindByEmailAsync(userDto.Email);
            if (existing != null)
            {
                userLog.result = false;
                userLog.Message = "User with this email already exists";
                _context.UserActivityLogs.Add(userLog);
                await _context.SaveChangesAsync();
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

            var createResult = await _userManager.CreateAsync(newUser, userDto.Password);
            if (!createResult.Succeeded)
            {
                userLog.result = false;
                userLog.Message = "User creation failed: " + string.Join("; ", createResult.Errors.Select(e => e.Description));
                _context.UserActivityLogs.Add(userLog);
                await _context.SaveChangesAsync();

                throw new Exception($"User creation failed: {userLog.Message}");

            }

            // generate token
            var token = await TokenService.CreateToken(newUser, _userManager, _config);

            userLog.result = true;
            userLog.Message = "User registered successfully";
            _context.UserActivityLogs.Add(userLog);

            // single save and commit transaction
            await _context.SaveChangesAsync();

            return new
            {
                Token = token,
                User = newUser
            };
        }
        public async Task<object> Login(LoginDto loginDto, string ipAddress)
        {
            // create log early
            var userLog = new UserActivityLog
            {
                Email = loginDto.Email,
                ActivityType = ActivityType.Login,
                Timestamp = CommonService.GenerateHKTime(),
                IpAddress = ipAddress
            };

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                userLog.result = false;
                userLog.Message = "Invalid credentials";
                _context.UserActivityLogs.Add(userLog);
                await _context.SaveChangesAsync();
                throw new Exception("Invalid credentials");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordValid)
            {
                userLog.result = false;
                userLog.Message = "Invalid credentials";
                _context.UserActivityLogs.Add(userLog);
                await _context.SaveChangesAsync();
                throw new Exception("Invalid credentials");
            }

            var token = await TokenService.CreateToken(user, _userManager, _config);

            userLog.result = true;
            userLog.Message = "User logged in successfully";
            _context.UserActivityLogs.Add(userLog);
            await _context.SaveChangesAsync();

            return new
            {
                Token = token,
                User = user
            };
        }


    }
}
