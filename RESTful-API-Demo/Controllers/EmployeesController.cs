using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RESTful_API_Demo.DTOS;
using RESTful_API_Demo.Services;

namespace RESTful_API_Demo.Controllers
{
    [ApiController]
    [Route("api/companies/{companyId}/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly ICompanyRepository companyRepository;
        private readonly IMapper mapper;

        public EmployeesController(
            ICompanyRepository companyRepository,
            IMapper mapper)
        {
            this.companyRepository = companyRepository ??
                throw new ArgumentNullException(nameof(companyRepository));
            this.mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetEmployees(
            [FromRoute]Guid companyId,
            [FromQuery(Name = "gender")]string genderDisplay,
            [FromQuery(Name = "q")]string keyword)
        {
            if (!await this.companyRepository.CompanyExistAsync(companyId))
            {
                return this.NotFound();
            }

            var employees = await this.companyRepository.GetEmployeesAsync(companyId, genderDisplay, keyword);
            var employeeDTOs = this.mapper.Map<IEnumerable<EmployeeDTO>>(employees);
            return this.Ok(employeeDTOs);
        }

        [Route("{employeeId}")]
        public async Task<ActionResult<EmployeeDTO>> GetEmployeeForCompany(Guid companyId, Guid employeeId)
        {
            var employee = await this.companyRepository.GetEmployeeAsync(companyId, employeeId);
            if (employee == null)
            {
                return this.NotFound();
            }

            var employeeDTO = mapper.Map<EmployeeDTO>(employee);
            return this.Ok(employeeDTO);
        }
    }
}