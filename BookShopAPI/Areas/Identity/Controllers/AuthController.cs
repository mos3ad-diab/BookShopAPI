using BookShopAPI.DTOs.Request;
using BookShopAPI.DTOs.Response;
using BookShopAPI.Models;
using BookShopAPI.Repositories;
using BookShopAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

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
        [HttpPost("Register")]
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
                "Book shop Api Confirmation",
                $"<h1> please click <a href = {link}> here </a> to confirm your account </h1>"
                );
            await _userManager.AddToRoleAsync(user, CD.CUSTOMER_ROLE);
            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Account Created Successfuly, please confirm your email"
            });
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "faild to confirm your email",
                    Errors = result.Errors.Select(e => e.Description)

                });
            }
            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Account Created Successfuly, please confirm your email"
            });
        }

        [HttpPost("ResendEmailConfirmation")]
        public async Task<IActionResult> ResendEmailConfirmation(EmailConfirmationRequest emailConfirmationRequest)
        {
            var user = await _userManager.FindByEmailAsync(emailConfirmationRequest.UserNameOrEmail) ??
               await _userManager.FindByNameAsync(emailConfirmationRequest.UserNameOrEmail);


            if (user is null)
            {
                ModelState.AddModelError("", "User not found");
                
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "User not found",

                });

            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "This email is already confirmed.");
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "This email is already confirmed.",

                });
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action("ConfirmEmail", "Account", new { area = "Identity", userId = user.Id, token = token }, Request.Scheme);

            await _emailSender.SendEmailAsync(
                user.Email,
                "Falcon Cinema Confirmation",
                $"<h1> please click <a href = {link}> here </a> to confirm your account </h1>"
                );

            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Account confirmed Successfully"
            });

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login_Request loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.UserNameOrEmail) ??
                await _userManager.FindByNameAsync(loginRequest.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError("", "Invalid user name or password");
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid user name or password"
                });
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, loginRequest.RememberMe, true);
            string message;
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {                   
                    message = "to many attemps please try again later";
                }
                else if (result.IsNotAllowed)
                {
                    message = "please confirme your email";
                }
                else
                {
                    message = "Invalid user name or password";
                }
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = message
                });

            }

            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Login Successfully"
            });
        }
        
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Logout Successfully"
            });
        }

        [HttpPost("ForgetPasswordAsync")]
        public async Task<IActionResult> ForgetPasswordAsync(ForgetPassworRequest forgetPassworRequest)
        {
            var user = await _userManager.FindByEmailAsync(forgetPassworRequest.UserNameOrEmail) ??
                await _userManager.FindByNameAsync(forgetPassworRequest.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "Invalid user name or Email");
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid user name or Email",
                    

                });
            }


            var otp = new Random().Next(1000, 9999).ToString();
            var applicationUserOTP = new ApplicationUserOTP(otp, user.Id);
            await _applicationUserOTPrepository.InsertAsync(applicationUserOTP);
            await _applicationUserOTPrepository.CommitAsync();
            await _emailSender.SendEmailAsync(
                user.Email,
                "BookShop Password Reset",
                $"Use this OTP {otp} to continue the prosess"
                );

            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "OTP sent successfully"
            });
        }

        [HttpPost("ConfirmOTP")]
        public async Task<IActionResult> ConfirmOTP(ConfirmOtpRequest confirmOtpRequest)
        {
            var user = await _userManager.FindByIdAsync(confirmOtpRequest.UserId);

            if (user is null)
            {
                ModelState.AddModelError("", "Invalid user");
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid user",
                });
            }
            var otps = await _applicationUserOTPrepository.GetAllAsync(o =>
            o.ApplicationUserId == user.Id &&
            o.IsValid == true &&
            o.ValidTo >= DateTime.UtcNow
            );
            var applicationUserOtp = otps.OrderByDescending(e => e.CreatedAt).FirstOrDefault();
            if (applicationUserOtp == null || applicationUserOtp.OTP != confirmOtpRequest.OTP)
            {
                ModelState.AddModelError("", "Invalid / expired otp");
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid / expired otp"

                });
            }
            applicationUserOtp.IsValid = false;
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _applicationUserOTPrepository.CommitAsync();
            return Ok(new ApiResponse<string>()
            {
                IsSuccess = true,
                Message = "OTP confimred successfully",
                Data = token
                
                
            });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword_Request resetPasswordRequest)
        {
            
            var user = await _userManager.FindByIdAsync(resetPasswordRequest.UserId);

            if (user is null)
            {
                ModelState.AddModelError("", "Invalid user");
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid user"

                });

            }
            await _userManager.ResetPasswordAsync(user, resetPasswordRequest.Token, resetPasswordRequest.Password);

            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Password reset successfully"
            });
        }
        
        
        
        




    }
}
