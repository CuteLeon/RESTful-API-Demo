using System.ComponentModel.DataAnnotations;
using RESTful_API_Demo.DTOS;

namespace RESTful_API_Demo.ValidationAttributes
{
    public class EmployeeCustomValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            /* 当特性修饰在类上时，validationContext.ObjectInstance 和 value 都是类的对象；
             * 当特性修饰在属性上时，validationContext.ObjectInstance 是类的对象，而 value 将会是属性的值；
             */
            if (!(value is EmployeeAddOrUpdateDTO employee))
            {
                return base.IsValid(value, validationContext);
            }

            if (employee.EmployeeNo == employee.FirstName)
            {
                return new ValidationResult(ErrorMessage, new[] { nameof(employee) });
            }

            return ValidationResult.Success;
        }
    }
}
