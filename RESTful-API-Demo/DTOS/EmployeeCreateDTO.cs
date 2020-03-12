using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RESTful_API_Demo.Entities;
using RESTful_API_Demo.ValidationAttributes;

namespace RESTful_API_Demo.DTOS
{
    [EmployeeCustomValidation]
    public class EmployeeCreateDTO : IValidatableObject
    {
        [Display(Name = "名称")]
        [Required(ErrorMessage = "{0}不可为空！")]
        [MaxLength(50, ErrorMessage = "{0}的长度不可超过{1}！")]
        public string FirstName { get; set; }

        [Display(Name = "姓氏")]
        [Required(ErrorMessage = "{0}不可为空！")]
        [MaxLength(50, ErrorMessage = "{0}的长度不可超过{1}！")]
        public string LastName { get; set; }

        [Display(Name = "员工号")]
        [Required(ErrorMessage = "{0}不可为空！")]
        [StringLength(maximumLength: 10, MinimumLength = 10, ErrorMessage = "{0}的长度是{1}！")]
        public string EmployeeNo { get; set; }

        [Display(Name = "性别")]
        public Gender Gender { get; set; }

        [Display(Name = "出生日期")]
        public DateTime DateOfBirth { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FirstName == LastName)
            {
                yield return new ValidationResult("姓氏和名称不可以相同！", new[] { nameof(FirstName), nameof(LastName) });
            }
        }
    }
}
