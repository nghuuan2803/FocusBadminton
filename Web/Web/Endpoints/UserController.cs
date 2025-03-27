using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Infrastructure.Data;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<Account> _userManager;
    private readonly AppDbContext _dbContext;

    public UserController(UserManager<Account> userManager, AppDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    [HttpGet("info")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo()
    {
        try
        {
            // Lấy userId từ token (claim NameIdentifier)
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Không tìm thấy thông tin người dùng trong token.");
            }

            // Lấy thông tin từ database
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            var member = await _dbContext.Members
                .Include(m => m.CurrentTeam) // Include Team để lấy CurrentTeam
                .FirstOrDefaultAsync(m => m.AccountId == userId);

            var userInfo = new
            {
                name = member?.FullName ?? user.UserName,
                email = user.Email,
                phoneNumber = user.PhoneNumber,
                avatar = user.Avatar ?? "",
                currentTeamId = member?.CurrentTeamId,
                currentTeamName = member?.CurrentTeam?.Name,
                gender = member?.Gender,
                dob = member?.DoB?.ToString("yyyy-MM-dd"),
                address = member?.Address,
                contributed = member?.Contributed ?? 0.0
            };

            return Ok(userInfo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Lỗi server: {ex.Message}");
        }
    }

    [HttpPut("info")]
    [Authorize]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfoRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Không tìm thấy thông tin người dùng trong token.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            var member = await _dbContext.Members
                .FirstOrDefaultAsync(m => m.AccountId == userId);
            if (member == null)
            {
                return NotFound("Không tìm thấy thông tin thành viên.");
            }

            // Cập nhật thông tin
            if (!string.IsNullOrEmpty(request.FullName)) member.FullName = request.FullName;
            if (!string.IsNullOrEmpty(request.Address)) member.Address = request.Address;
            if (!string.IsNullOrEmpty(request.Gender)) member.Gender = request.Gender;
            if (request.DoB.HasValue) member.DoB = request.DoB.Value;
            if (!string.IsNullOrEmpty(request.Avatar)) user.Avatar = request.Avatar;

            _dbContext.Members.Update(member);
            await _userManager.UpdateAsync(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Cập nhật thông tin thành công." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Lỗi server: {ex.Message}");
        }
    }

    public class UpdateUserInfoRequest
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public DateTime? DoB { get; set; }
        public string? Avatar { get; set; }
    }
}