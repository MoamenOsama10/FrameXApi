using ApplicationEcommerce.Interfaces;
using InfrastructureEcommerce.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InfrastructureEcommerce.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public IdentityService(
       UserManager<ApplicationUser> userManager,
       SignInManager<ApplicationUser> signInManager,
       IEmailService emailService,
       IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<(bool Succeeded, string? UserId, IEnumerable<string> Errors)> RegisterAsync(
            string email, string password, string fullName)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return (false, null, result.Errors.Select(e => e.Description));

            return (true, user.Id, Enumerable.Empty<string>());
        }

        public async Task<(bool Succeeded, string? UserId, IList<string> Roles, IEnumerable<string> Errors)> LoginAsync(
       string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return (false, null, new List<string>(), new[] { "Invalid email or password." });

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

            if (!result.Succeeded)
                return (false, null, new List<string>(), new[] { "Invalid email or password." });

            var roles = await _userManager.GetRolesAsync(user);

            return (true, user.Id, roles, Enumerable.Empty<string>());
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);

            return true;
        }

       
        public async Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(
            string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return (false, new[] { "Invalid request." });

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return result.Succeeded
                ? (true, Enumerable.Empty<string>())
                : (false, result.Errors.Select(e => e.Description));
        }

        public async Task<UserProfileResult?> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return null;

            return new UserProfileResult(
                user.Id,
                user.FullName,
                user.Email!,
                user.PhoneNumber,
                user.Gender,
                user.DateOfBirth,
                user.MembershipType,
                user.MemberSince);
        }  

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateProfileAsync(
            string userId, string fullName, string? phoneNumber, DateTime? dateOfBirth, string? gender)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return (false, new[] { "User not found." });

            user.FullName = fullName;
            user.PhoneNumber = phoneNumber;
            user.DateOfBirth = dateOfBirth;
            user.Gender = gender;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded
                ? (true, Enumerable.Empty<string>())
                : (false, result.Errors.Select(e => e.Description));
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return (true, Enumerable.Empty<string>()); // متعمد: ما نأكدش لو الإيميل موجود أو لأ

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);

            var clientResetUrl = _configuration["ClientApp:ResetPasswordUrl"]
                ?? "https://localhost:3000/reset-password";

            var resetLink = $"{clientResetUrl}?email={HttpUtility.UrlEncode(email)}&token={encodedToken}";

            var emailBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 480px; margin: auto;'>
                <h2 style='color: #111;'>FrameX - Reset Your Password</h2>
                <p>We received a request to reset your password. Click the button below to choose a new one.</p>
                <a href='{resetLink}'
                   style='display:inline-block; padding: 12px 24px; background:#111; color:#fff;
                          text-decoration:none; border-radius:6px; margin-top:12px;'>
                    Reset Password
                </a>
                <p style='margin-top:24px; color:#666; font-size:13px;'>
                    If you didn't request this, you can safely ignore this email. This link expires in 24 hours.
                </p>
            </div>";

            await _emailService.SendEmailAsync(email, "Reset your FrameX password", emailBody);

            return (true, Enumerable.Empty<string>());
        }

        // ... ResetPasswordAsync, GetUserProfileAsync, UpdateProfileAsync زي ما هم بالظبط من قبل
    }
}
