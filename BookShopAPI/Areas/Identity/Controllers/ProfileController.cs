using BookShopAPI.DTOs.Request;
using BookShopAPI.DTOs.Response;
using BookShopAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookShopAPI.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) { return NotFound(); }
            var profile = new ProfileResponse()
            {
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };
            return Ok(new ApiResponse<ProfileResponse>()
            {
                IsSuccess = true,
                Message = "User found successfully",
                Data = profile

            });
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update(ProfileResponse profileResponse)
        {
            var user = await _userManager.GetUserAsync(User);

            user.Name = profileResponse.Name;
            user.PhoneNumber = profileResponse.PhoneNumber;
            user.Address = profileResponse.Address;

            await _userManager.UpdateAsync(user);

            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Profile updated successfully"
            });
        }
        

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(" ,", result.Errors.Select(e => e.Description));
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "failed to change your password",
                    Errors = result.Errors.Select(e => e.Description)

                });
            }
            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Password changed successfully!"
            });
        }
    }
}
