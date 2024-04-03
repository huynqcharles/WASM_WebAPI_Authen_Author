using API.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepo _accountRepo;
        public AccountsController(IAccountRepo accountRepo)
        {
            _accountRepo = accountRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO, string role)
        {
            var response = await _accountRepo.RegisterAsync(registerDTO, role);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var response = await _accountRepo.LoginAsync(loginDTO);
            return Ok(response);
        }
    }
}
