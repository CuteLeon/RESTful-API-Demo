using System.Collections.Generic;

namespace RESTful_API_Demo.DTOS
{
    public class CompanyCreateDTO
    {
        public string Name { get; set; }
        public string Introduction { get; set; }
        public ICollection<EmployeeCreateDTO> Employees { get; set; } = new List<EmployeeCreateDTO>();
    }
}
