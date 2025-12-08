using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Project_X.Models.CustomAttributes
{
    public class EndTimeAfterStart: ValidationAttribute
    {
        private readonly string _startDateProperty;
        public EndTimeAfterStart(string startDateProperty)
        {
            _startDateProperty = startDateProperty;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var endDate = (DateTime?)value;
            var startDateProperty = validationContext.ObjectType.GetProperty(_startDateProperty);
            if (startDateProperty == null)
            {
                return new ValidationResult($"Unknown property: {_startDateProperty}");
            }
            var startDate = (DateTime?)startDateProperty.GetValue(validationContext.ObjectInstance);
            if (endDate.HasValue && startDate.HasValue && endDate <= startDate)
            {
                return new ValidationResult(ErrorMessage ?? "End time must be after start time.");
            }
            return ValidationResult.Success!;
        }
    }
}
