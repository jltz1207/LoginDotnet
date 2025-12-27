using LoginDotnet.Models.Dtos;
using LoginDotnet.Models.Entities;
using LoginDotnet.Models.ViewModels;
using LoginDotnet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LoginDotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IAccountService accountService, ILogger<AccountController> logger  )
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _accountService.GetUsers();
                return Ok(users);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred during Get Users");
                return BadRequest(new ErrorViewModel
                {
                    Code = "INTERNAL_ERROR",
                    Message = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto userdto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var obj = await _accountService.Register(userdto);
                return Ok(obj);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration");
                return BadRequest(new ErrorViewModel
                {
                    Code = "INTERNAL_ERROR",
                    Message = ex.Message
                });
            }

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var obj = await _accountService.Login(loginDto);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user login");   
                return BadRequest(new ErrorViewModel
                {
                    Code = "INVALID_CREDENTIALS",
                    Message = ex.Message
                });
            }
        }


    }
}
