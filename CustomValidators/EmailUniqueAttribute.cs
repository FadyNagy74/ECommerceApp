using E_CommerceApp.Models;
using System.ComponentModel.DataAnnotations;

namespace E_CommerceApp.CustomValidators
{
    public class EmailUniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));
            if (dbContext == null)
                throw new InvalidOperationException("DbContext not available");

            if (value == null) return ValidationResult.Success;

            string? email = value as string;
            if (string.IsNullOrWhiteSpace(email)) return ValidationResult.Success;

            string normalizedEmail = email.ToUpperInvariant();

            string? currentUserId = null;
            if (validationContext.Items.ContainsKey("UserId"))
            {
                currentUserId = validationContext.Items["UserId"]?.ToString();
            }

            var existingUser = dbContext.Users.FirstOrDefault(u => u.NormalizedEmail == normalizedEmail);

            if (existingUser != null && existingUser.Id != currentUserId)
            {
                return new ValidationResult("Email must be unique");
            }

            return ValidationResult.Success;
        }
    }

}
