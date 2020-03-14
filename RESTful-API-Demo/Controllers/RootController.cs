using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RESTful_API_Demo.DTOS;

namespace RESTful_API_Demo.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = nameof(GetRoot))]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDTO>()
            {
                new LinkDTO(this.Url.Link(nameof(GetRoot),new{ }),"self", "GET"),
                new LinkDTO(this.Url.Link(nameof(CompaniesController.GetCompanies),new{ }),"get_companies", "GET"),
                new LinkDTO(this.Url.Link(nameof(CompaniesController.CreateCompany),new{ }),"create_company", "POST"),
            };

            return this.Ok(links);
        }
    }
}