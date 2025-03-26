using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Auth;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;

namespace Web.Endpoints
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginStrategyFactory _loginStrategyFactory;
        private readonly UserManager<Account> _userManager;
        private readonly AppDbContext _dbContext;

        public AuthController(ILoginStrategyFactory loginStrategyFactory, UserManager<Account> userManager, AppDbContext dbContext)
        {
            _loginStrategyFactory = loginStrategyFactory;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 📌 Chọn strategy dựa vào request.LoginType ("google", "password", "facebook", ...)
            var strategy = _loginStrategyFactory.GetStrategy(request.LoginType);
            var result = await strategy.LoginAsync(request.Credential);

            if (!result.Succeeded) 
            {
                return BadRequest(new { error = result.Errors });
            }
            return Ok(result.Data);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Kiểm tra validation từ DataAnnotations
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { Errors = errors });
            }

            // Kiểm tra email đã tồn tại
            var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return BadRequest(new { Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            // Kiểm tra số điện thoại đã tồn tại
            var existingUserByPhone = await _dbContext.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (existingUserByPhone)
            {
                ModelState.AddModelError("PhoneNumber", "Số điện thoại đã được sử dụng.");
                return BadRequest(new { Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            // Tạo đối tượng Account
            var user = new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true, // Có thể thêm logic xác nhận email sau
                CreatedAt = DateTimeOffset.Now,
                CreatedBy = "System",
                PersonalPoints = 0,
                RewardPoints = 0
            };

            // Tạo người dùng trong Identity
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }

            // Tạo bản ghi Member
            var member = new Member
            {
                AccountId = user.Id,
                FullName = request.Fullname,
                CreatedAt = DateTimeOffset.Now,
                CreatedBy = "System",
                Email = request.Email
            };

            try
            {
                await _dbContext.Members.AddAsync(member);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu lưu Member thất bại
                return StatusCode(500, new { Error = "Không thể lưu thông tin thành viên. Vui lòng thử lại.", Details = ex.Message });
            }

            // Gán vai trò Customer
            var roleResult = await _userManager.AddToRoleAsync(user, "member");
            if (!roleResult.Succeeded)
            {
                // Xử lý lỗi gán vai trò (hiếm xảy ra, nhưng cần kiểm tra)
                var roleErrors = roleResult.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = roleErrors });
            }

            return Ok(new { Message = "Đăng ký thành công" });
        }
    }
}
