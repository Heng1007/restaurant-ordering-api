using FoodDeliveryServer.Data;
using FoodDeliveryServer.Dtos;
using FoodDeliveryServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodDeliveryServer.Utils;

namespace FoodDeliveryServer.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        public AuthService(AppDbContext appDbContext, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = appDbContext;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> Login(UserLoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                _logger.LogWarning($" Login Failed. Username {request.Username}' 不存在");
                return "User not found.";
            }

            // 2. 验密码：拿用户输入的明文，跟数据库里的乱码比对
            // BCrypt 会自动处理 Salt，你不用管
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning($"❌ 登录失败: 用户 '{request.Username}' 密码错误");
                return "Wrong password.";
            }

            // 3. (核心) 生成 JWT Token 通行证 🎫
            // 这里代码比较多，我稍后解释每一行
            _logger.LogInformation($"✅ 用户 '{request.Username}' 登录成功");
            string token = CreateToken(user);
            return token;
        }

        public async Task<string> Register(UserDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return "Username already exists.";
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                Role = Constants.Roles.User
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreateToken(user);
        }

        private string CreateToken(User user)
        {
            // A. 定义“荷载” (Claims) - 也就是手环上写的信息
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username), // 手环上写着用户名
                new Claim(ClaimTypes.Role, user.Role),     // 手环上写着角色 (Admin/User)
                new Claim("id", user.Id.ToString())        // 手环上写着用户ID
            };

            // B. 拿钥匙 (从 appsettings 读取刚才配置的 Key)
            // 注意：实际项目中应该用 IConfiguration 注入读取，这里为了简单演示先硬编码，或者你告诉我你想不想学注入读取？
            // 为了不报错，我们先暂时写死 (但这是不好的习惯，等你跑通了我们再改)
            var secretKey = _configuration["MyJwtKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("JWT Secret Key is not configured.");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // C. 签名凭证 (用 HmacSha256 算法签名)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // D. 制造 Token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1), // 有效期 1 天
                signingCredentials: creds
            );

            // E. 把 Token 对象转成字符串
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
