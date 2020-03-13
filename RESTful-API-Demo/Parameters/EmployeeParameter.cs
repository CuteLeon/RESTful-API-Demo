using Microsoft.AspNetCore.Mvc;

namespace RESTful_API_Demo.Parameters
{
    public class EmployeeParameter
    {
        public string Gender { get; set; }

        [FromQuery(Name = "keyword")]
        public string SearchTerm { get; set; }
    }
}
