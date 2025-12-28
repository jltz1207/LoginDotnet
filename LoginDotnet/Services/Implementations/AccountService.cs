using LoginDotnet.Data;
using LoginDotnet.Infra.Security;
using LoginDotnet.Models.Dtos;
using LoginDotnet.Models.Entities;
using LoginDotnet.Services.CommonServices;
using LoginDotnet.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace LoginDotnet.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly ApplicationContext _context;
        private readonly FileStorageService _fileStorageService;
        private readonly IHttpContextAccessor _httpContextAcessor;

        public AccountService(UserManager<User> userManager, IConfiguration config, ApplicationContext context, FileStorageService fileStorageService, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _config = config;
            _context = context;
            _fileStorageService = fileStorageService;
            _httpContextAcessor = httpContextAccessor;
        }

        public async Task<List<User>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return users;
        }

        public async Task<object> Register(RegisterDto userDto)
        {
            var context = _httpContextAcessor.HttpContext;
            if(context == null)
            {
                throw new Exception("The connection is with problem.");
            }

            var userLog = new UserActivityLog
            {
                Email = userDto.Email,
                ActivityType = ActivityType.Register,
                Timestamp = CommonService.GenerateHKTime(),
                IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
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
            userLog.UserId = newUser.Id;
            _context.UserActivityLogs.Add(userLog);

            // single save and commit transaction
            await _context.SaveChangesAsync();

            return new
            {
                Token = token,
                User = newUser
            };
        }
        public async Task<object> Login(LoginDto loginDto)
        {
            var context = _httpContextAcessor.HttpContext;
            if(context == null)
            {
                throw new Exception("The connection is with problem.");
            }
            
            var userLog = new UserActivityLog
            {
                Email = loginDto.Email,
                ActivityType = ActivityType.Login,
                Timestamp = CommonService.GenerateHKTime(),
                IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
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
            userLog.UserId = user.Id;
            _context.UserActivityLogs.Add(userLog);
            await _context.SaveChangesAsync();

            return new
            {
                Token = token,
                User = user
            };
        }

        public async Task<string> UploadProfilePic(IFormFile userProfile)
        {
            var context = _httpContextAcessor.HttpContext;
            if (context == null)
            {
                throw new Exception("The connection is with problem.");
            }

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userEmail = context.User.FindFirst(ClaimTypes.Email).Value;
            if (userId == null || userEmail == null)
            {
                throw new Exception("User is not authenticated");
            }
            var userLog = new UserActivityLog
            {
                UserId = userId,
                Email = userEmail,
                ActivityType = ActivityType.ProfileUpdate,
                Timestamp = CommonService.GenerateHKTime(),
                IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
            };
            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null)
            {
                userLog.result = false;
                userLog.Message = "User not found";
                _context.UserActivityLogs.Add(userLog);
                await _context.SaveChangesAsync();
                throw new Exception("User not found");
            }

            var subDirectories = new List<string> { "ProfileImage", userId };
            string relativePath = await _fileStorageService.SaveFileAsync(userProfile, subDirectories, true);

            // do database action
            currentUser.ProfilePath = relativePath;
            await _userManager.UpdateAsync(currentUser);
            userLog.result = true;
            userLog.Message = "User Uploaded successfully";
            _context.UserActivityLogs.Add(userLog);
            await _context.SaveChangesAsync();

            var webPath = _fileStorageService.GenerateFileLink(relativePath);

            return webPath;
        }
        public async Task<string> GetProfileImage()
        {
            var context = _httpContextAcessor.HttpContext;
            if (context == null)
            {
                throw new Exception("The connection is with problem.");
            }
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userId == null)
            {
                throw new Exception("User is not authenticated");
            }
            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser == null)
            {
                throw new Exception("User not found");
            }
            if(currentUser.ProfilePath == null)
            {
                throw new Exception("Profile image not found");
            }
            var webPath = _fileStorageService.GenerateFileLink(currentUser.ProfilePath);
            return webPath;
        }
    }
}
