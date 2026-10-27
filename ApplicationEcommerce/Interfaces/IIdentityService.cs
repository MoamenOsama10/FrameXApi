using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationEcommerce.Interfaces
{
    public interface IIdentityService
    {
        Task<(bool Succeeded, string? UserId, IEnumerable<string> Errors)> RegisterAsync(
            string email, string password, string fullName);

        Task<(bool Succeeded, string? UserId, IList<string> Roles, IEnumerable<string> Errors)> LoginAsync(
     string email, string password);

        Task<bool> LogoutAsync(string userId);

        Task<(bool Succeeded, IEnumerable<string> Errors)> ForgotPasswordAsync(string email);

        Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(
            string email, string token, string newPassword);

        Task<UserProfileResult?> GetUserProfileAsync(string userId);

        Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateProfileAsync(
            string userId, string fullName, string? phoneNumber, DateTime? dateOfBirth, string? gender);
    }

    public record UserProfileResult(
        string Id,
        string FullName,
        string Email,
        string? PhoneNumber,
        string? Gender,
        DateTime? DateOfBirth,
        string MembershipType,
        DateTime MemberSince);
}
