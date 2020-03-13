using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RESTful_API_Demo.DTOS;
using RESTful_API_Demo.Entities;
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

        [Route("{employeeId}"), ActionName(nameof(GetEmployeeForCompany))]
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

        [HttpPost]
        public async Task<ActionResult<EmployeeDTO>> CreateEmployeeForCompany(
            [FromRoute]Guid companyId,
            [FromBody]EmployeeCreateDTO employeeCreateDTO)
        {
            if (!await companyRepository.CompanyExistAsync(companyId))
            {
                return this.NotFound();
            }

            var employee = this.mapper.Map<Employee>(employeeCreateDTO);
            this.companyRepository.AddEmployee(companyId, employee);
            await this.companyRepository.SaveAsync();
            var employeeDTO = this.mapper.Map<EmployeeDTO>(employee);
            return this.CreatedAtAction(
                nameof(GetEmployeeForCompany),
                new { companyId = companyId, employeeId = employee.Id },
                employeeDTO);
        }

        [HttpPut("{employeeId}")]
        public async Task<ActionResult> UpdateEmployeeForCompany(Guid companyId, Guid employeeId, EmployeeUpdateDTO employeeUpdateDTO)
        {
            if (!await this.companyRepository.CompanyExistAsync(companyId))
            {
                return this.NotFound();
            }

            var employee = await this.companyRepository.GetEmployeeAsync(companyId, employeeId);
            if (employee == null)
            {
                employee = this.mapper.Map<Employee>(employeeUpdateDTO);
                employee.Id = employeeId;
                this.companyRepository.AddEmployee(companyId, employee);
                await this.companyRepository.SaveAsync();

                var employeeDTO = this.mapper.Map<EmployeeDTO>(employee);
                return this.CreatedAtAction(
                    nameof(GetEmployeeForCompany),
                    new { companyId = companyId, employeeId = employee.Id },
                    employeeDTO);
            }
            else
            {
                this.mapper.Map(employeeUpdateDTO, employee);
                this.companyRepository.UpdateEmployee(employee);
                await this.companyRepository.SaveAsync();

                return this.NoContent();
            }
        }

        // 请求的 Content-Type 值为 "application/json-patch+json"
        [HttpPatch("{employeeId}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(
            [FromRoute]Guid companyId,
            [FromRoute]Guid employeeId,
            [FromBody]JsonPatchDocument<EmployeeUpdateDTO> jsonPatchDocument)
        {
            if (!await this.companyRepository.CompanyExistAsync(companyId))
            {
                return this.NotFound();
            }

            var employee = await this.companyRepository.GetEmployeeAsync(companyId, employeeId);
            if (employee == null)
            {
                return this.NotFound();
            }

            // 将 JasnPatchDocument 的操作应用到 DTO 对象
            var employeeUpdateDTO = this.mapper.Map<EmployeeUpdateDTO>(employee);
            jsonPatchDocument.ApplyTo(employeeUpdateDTO);
            this.mapper.Map(employeeUpdateDTO, employee);

            this.companyRepository.UpdateEmployee(employee);
            await this.companyRepository.SaveAsync();
            return NoContent();
        }
    }
}