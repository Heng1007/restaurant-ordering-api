using FoodDeliveryServer.Dtos;


namespace FoodDeliveryServer.Services
{
    public interface IAuthService
    {
        Task<string> Register(UserDto request);
        Task<string> Login(UserLoginDto request);
    }
}
