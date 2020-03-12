using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RESTful_API_Demo.DTOS;
using RESTful_API_Demo.Entities;
using RESTful_API_Demo.ModelBinders;
using RESTful_API_Demo.Services;

namespace RESTful_API_Demo.Controllers
{
    [ApiController]
    [Route("api/companies/{companyId}/employeeCollections")]
    public class EmployeeCollectionsController : ControllerBase
    {
        private readonly ICompanyRepository companyRepository;
        private readonly IMapper mapper;

        public EmployeeCollectionsController(
            ICompanyRepository companyRepository,
            IMapper mapper)
        {
            this.companyRepository = companyRepository ??
                throw new ArgumentNullException(nameof(companyRepository));
            this.mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> CreateEmployeesForCompany(
            [FromRoute]Guid companyId,
            [FromBody]IEnumerable<EmployeeCreateDTO> employeeCreateDTOs)
        {
            if (!await companyRepository.CompanyExistAsync(companyId))
            {
                return this.NotFound();
            }

            var employees = this.mapper.Map<IEnumerable<Employee>>(employeeCreateDTOs);
            foreach (var employee in employees)
            {
                this.companyRepository.AddEmployee(companyId, employee);
            }
            await this.companyRepository.SaveAsync();

            var employeeDTOs = this.mapper.Map<IEnumerable<EmployeeDTO>>(employees);
            var employeeIds = string.Join(",", employees.Select(x => x.Id));
            return this.CreatedAtRoute(
                nameof(GetEmployeesForCompany),
                new { companyId = companyId, employeeIds = employeeIds },
                employeeDTOs);
        }

        [HttpGet("{employeeIds}", Name = nameof(GetEmployeesForCompany))]
        public async Task<ActionResult<EmployeeDTO>> GetEmployeesForCompany(
            [FromRoute]
            Guid companyId,
            [FromRoute]
            [ModelBinder(BinderType =typeof(ArrayModelBinder))]
            IEnumerable<Guid> employeeIds)
        {
            if (employeeIds == null)
            {
                return this.BadRequest();
            }

            var employees = await this.companyRepository.GetEmployeesAsync(employeeIds);
            if (employees.Count() != employeeIds.Count())
            {
                return NotFound();
            }

            var employeeDTOs = this.mapper.Map<IEnumerable<EmployeeDTO>>(employees);
            return this.Ok(employeeDTOs);
        }
    }
}