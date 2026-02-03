using FoodDeliveryServer.Dtos;
using Microsoft.AspNetCore.Mvc;
using FoodDeliveryServer.Services;

namespace FoodDeliveryServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(UserDto request)
        {
            var register = await _authService.Register(request);
            return Ok(register);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request)
        {
            var login = await _authService.Login(request);
            return Ok(login);
        }
    }
}
