using System.ComponentModel.DataAnnotations;

namespace MovieBookingBackend.Validations
{
    public class EnumValidation : ValidationAttribute
    {
        private readonly Type _enumType;

        public EnumValidation(Type enumType)
        {
            _enumType = enumType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || Enum.IsDefined(_enumType, value))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult($"Invalid value for {validationContext.MemberName}. Allowed values are: {string.Join(", ", Enum.GetNames(_enumType))}");
        }
    }
}
