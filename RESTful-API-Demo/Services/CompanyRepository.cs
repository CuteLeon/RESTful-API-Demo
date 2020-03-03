using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RESTful_API_Demo.Data;
using RESTful_API_Demo.Entities;

namespace RESTful_API_Demo.Services
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly RouteDBContext context;

        public CompanyRepository(RouteDBContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Company>> GetCompaniesAsync()
            => await this.context.Companies.ToListAsync();

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

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId)
        {
            if (Guid.Empty == companyId)
                throw new ArgumentNullException(nameof(companyId));

            return await this.context.Employees
                .Where(x => x.CompanyId == companyId)
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
