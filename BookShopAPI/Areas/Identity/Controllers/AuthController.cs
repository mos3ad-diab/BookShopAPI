using BookShopAPI.Models;
using BookShopAPI.Repositories;
using BookShopAPI.Utilities;
using BookShopAPI.DTOs.Request;
using BookShopAPI.DTOs.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookShopAPI.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPrepository;
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRepository<ApplicationUserOTP> applicationUserOTPrepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _applicationUserOTPrepository = applicationUserOTPrepository;
        }

        public async Task<IActionResult> Register(Register_Request registerRequest)
        {
            
            var user = new ApplicationUser()
            {
                Name = registerRequest.Name,
                UserName = registerRequest.UserName,
                Address = registerRequest.Address,
                Email = registerRequest.Email
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "faild to create account",
                    Errors = result.Errors.Select(e => e.Description)

                });
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action("ConfirmEmail", "Account", new { area = "Identity", userId = user.Id, token = token }, Request.Scheme);

            await _emailSender.SendEmailAsync(
                registerRequest.Email,
                "Falcon Cinema Confirmation",
                $"<h1> please click <a href = {link}> here </a> to confirm your account </h1>"
                );
            await _userManager.AddToRoleAsync(user, CD.CUSTOMER_ROLE);
            return CreatedAtAction("Login",new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Account Created Successfuly"
            });
        }
    }
}
