using E_CommerceApp.Models;
using System.ComponentModel.DataAnnotations;

namespace E_CommerceApp.CustomValidators
{
    public class UserNameUniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));
            if (dbContext == null)
                throw new InvalidOperationException("DbContext not available");

            if (value == null) return ValidationResult.Success;

            string? userName = value as string;
            if (string.IsNullOrWhiteSpace(userName)) return ValidationResult.Success;

            string normalizedUserName = userName.ToUpperInvariant();

            string? currentUserId = null;
            if (validationContext.Items.ContainsKey("UserId"))
            {
                currentUserId = validationContext.Items["UserId"]?.ToString();
            }

            var existingUser = dbContext.Users.FirstOrDefault(u => u.NormalizedUserName == normalizedUserName);

            if (existingUser != null && existingUser.Id != currentUserId)
            {
                return new ValidationResult("Username must be unique");
            }

            return ValidationResult.Success;
        }
    }

}
