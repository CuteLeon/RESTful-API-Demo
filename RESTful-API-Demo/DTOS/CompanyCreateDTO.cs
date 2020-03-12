using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RESTful_API_Demo.DTOS
{
    public class CompanyCreateDTO
    {
        [Display(Name = "公司名称")]
        [Required(ErrorMessage = "{0}不可为空！")]
        [MaxLength(100, ErrorMessage = "{0}的最大长度不可以超过{1}！")]
        public string Name { get; set; }

        [Display(Name = "公司简介")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "{0}的长度范围应介于{2}~{1}！")]
        public string Introduction { get; set; }

        public ICollection<EmployeeCreateDTO> Employees { get; set; } = new List<EmployeeCreateDTO>();
    }
}
