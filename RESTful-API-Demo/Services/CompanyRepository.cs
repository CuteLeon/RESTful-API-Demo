using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RESTful_API_Demo.Data;
using RESTful_API_Demo.Entities;
using RESTful_API_Demo.Parameters;

namespace RESTful_API_Demo.Services
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly RoutineDBContext context;

        public CompanyRepository(RoutineDBContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Company>> GetCompaniesAsync(CompanyParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var result = this.context.Companies.AsQueryable();

            // 过滤
            if (!string.IsNullOrEmpty(parameter.CompanyName))
            {
                result = result.Where(x => x.Name.Contains(parameter.CompanyName));
            }

            // 搜索
            if (!string.IsNullOrEmpty(parameter.SearchTerm))
            {
                result = result.Where(x =>
                    x.Name.Contains(parameter.SearchTerm) ||
                    x.Introduction.Contains(parameter.SearchTerm));
            }

            // 分页
            result = result
                .Skip((parameter.PageNumber - 1) * parameter.PageSize)
                .Take(parameter.PageSize);

            return await result.ToListAsync();
        }

        public async Task<Company> GetCompanyAsync(Guid companyId)
        {
            if (Guid.Empty == companyId)
                throw new ArgumentNullException(nameof(companyId));

            return await this.context.Companies
                .FirstOrDefaultAsync(x => x.Id == companyId);
        }

        public async Task<IEnumerable<Company>> GetCompaniesAsync(IEnumerable<Guid> companyIds)
        {
            if (companyIds == null)
                throw new ArgumentNullException(nameof(companyIds));

            return await this.context.Companies
                .Where(x => companyIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public void AddCompany(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            if (company.Id == Guid.Empty)
            {
                company.Id = Guid.NewGuid();
            }

            _ = company.Employees
                .Where(x => x.Id == Guid.Empty)
                .Select(x => x.Id = Guid.NewGuid())
                .ToArray();

            this.context.Companies.Add(company);
        }

        public void UpdateCompany(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            this.context.Companies.Update(company);
        }

        public void DeleteCompany(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            this.context.Companies.Remove(company);
        }

        public async Task<bool> CompanyExistAsync(Guid companyId)
        {
            if (Guid.Empty == companyId)
                throw new ArgumentNullException(nameof(companyId));

            return await this.context.Companies.AnyAsync(x => x.Id == companyId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, string genderDisplay, string keyword)
        {
            if (Guid.Empty == companyId)
                throw new ArgumentNullException(nameof(companyId));

            var result = this.context.Employees
                .Where(x => x.CompanyId == companyId);

            if (!string.IsNullOrEmpty(genderDisplay) &&
                Enum.TryParse<Gender>(genderDisplay.Trim(), out var gender))
            {
                result = result.Where(x => x.Gender == gender);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                result = result.Where(x =>
                    x.EmployeeNo.Contains(keyword) ||
                    x.FirstName.Contains(keyword) ||
                    x.LastName.Contains(keyword));
            }

            result = result.OrderBy(x => x.EmployeeNo);
            return await result.ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, IEnumerable<Guid> employeeIds)
        {
            if (employeeIds == null)
                throw new ArgumentNullException(nameof(employeeIds));

            return await this.context.Employees
                .Where(x =>
                    x.CompanyId == companyId &&
                    employeeIds.Contains(x.Id))
                .OrderBy(x => x.EmployeeNo)
                .ToListAsync();
        }

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId)
        {
            if (Guid.Empty == companyId)
                throw new ArgumentNullException(nameof(companyId));

            if (Guid.Empty == employeeId)
                throw new ArgumentNullException(nameof(employeeId));

            return await this.context.Employees
                .Where(x => x.CompanyId == companyId && x.Id == employeeId)
                .FirstOrDefaultAsync();
        }

        public void AddEmployee(Guid companyId, Employee employee)
        {
            if (Guid.Empty == companyId)
                throw new ArgumentNullException(nameof(companyId));

            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (employee.Id == Guid.Empty)
            {
                employee.Id = Guid.NewGuid();
            }

            employee.CompanyId = companyId;
            this.context.Employees.Add(employee);
        }

        public void UpdateEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            this.context.Entry(employee).State = EntityState.Modified;
        }

        public void DeleteEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            this.context.Employees.Remove(employee);
        }

        public async Task<bool> SaveAsync()
            => await this.context.SaveChangesAsync() >= 0;
    }
}
