using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryServer.Dtos
{
    public class UserDto
    {
        // 必填：用户名
        [Required(ErrorMessage = "Username must be filled!")]
        public string Username { get; set; } = string.Empty;

        // 必填：原始密码 (比如 "123456")
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = string.Empty;
    }
}
