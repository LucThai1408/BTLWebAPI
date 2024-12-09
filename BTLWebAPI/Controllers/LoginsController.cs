using BTLWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BTLWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginsController : ControllerBase
    {
        private readonly BTLContext _context;
        IConfiguration _configuration;

        public LoginsController(BTLContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Login(UserModel user)
        {
            var passmd5 = Cipher.GenerateMD5(user.Password);
            var acc = _context.Accounts.FirstOrDefault(x => x.UserName == user.Username && x.Password == passmd5);
            if (acc != null)
            {
                if (acc.Role.Equals("admin"))
                {
                    //lấy key trong file cấu hình
                    var key = _configuration["Jwt:Key"];
                    //mã hóa ky
                    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    //ký vào key đã mã hóa
                    var signingCredential = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
                    //tạo claims chứa thông tin người dùng (nếu cần)
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role , acc.Role),
                    new Claim(ClaimTypes.Name,user.Username),
                    new Claim("Fullname",acc.FullName),
                    new Claim("PhoneNumber",acc.Phone),
                    new Claim(ClaimTypes.StreetAddress , acc.Address)
                };

                    //tạo token với các thông số khớp với cấu hình trong startup để validate
                    var token = new JwtSecurityToken
                    (
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        expires: DateTime.Now.AddHours(1),
                        signingCredentials: signingCredential,
                        claims: claims
                    );
                    //sinh ra chuỗi token với các thông số ở trên
                    var hanam88token = new JwtSecurityTokenHandler().WriteToken(token);
                    //trả về kết quả cho client username và chuỗi token
                    return new JsonResult(new { token = hanam88token });
                }
                else
                {
                    return new JsonResult(new { message = "Username or password incorrect" });
                }
            }
            //trả về lỗi
            return new JsonResult(new { message = "Username or password incorrect" });
        }
    }
}
