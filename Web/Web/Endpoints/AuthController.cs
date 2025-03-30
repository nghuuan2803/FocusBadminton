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
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { Errors = errors });
            }

            var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return BadRequest(new { Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var existingMemberByPhone = await _dbContext.Members.AnyAsync(m => m.PhoneNumber == request.PhoneNumber);
            if (existingMemberByPhone)
            {
                ModelState.AddModelError("PhoneNumber", "Số điện thoại đã được sử dụng.");
                return BadRequest(new { Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var user = new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true,
                CreatedAt = DateTimeOffset.Now,
                CreatedBy = "System",
                PersonalPoints = 0,
                RewardPoints = 0
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }

            var member = new Member
            {
                AccountId = user.Id,
                FullName = request.Fullname,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                CreatedAt = DateTimeOffset.Now,
                CreatedBy = "System"
            };

            try
            {
                await _dbContext.Members.AddAsync(member);
                var rowsAffected = await _dbContext.SaveChangesAsync();
                Console.WriteLine($"Register - Rows affected: {rowsAffected}, AccountId: {member.AccountId}, PhoneNumber: {member.PhoneNumber}, FullName: {member.FullName}");
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(user);
                return StatusCode(500, new { Error = "Không thể lưu thông tin thành viên.", Details = ex.Message });
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "member");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                var roleErrors = roleResult.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = roleErrors });
            }

            return Ok(new { Message = "Đăng ký thành công" });
        }
    }
}
