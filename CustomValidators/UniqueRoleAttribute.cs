using E_CommerceApp.Models;
using System.ComponentModel.DataAnnotations;

namespace E_CommerceApp.CustomValidators
{
    public class UniqueRoleAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));

            if (value == null) return ValidationResult.Success;

            string? RoleName = value.ToString();

            bool exists = dbContext.Roles.Any(r => r.Name == RoleName);
            if (exists)
                return new ValidationResult("Role name must be unique");

            return ValidationResult.Success;
        }
    }
}
