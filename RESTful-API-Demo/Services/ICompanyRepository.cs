using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RESTful_API_Demo.Assists;
using RESTful_API_Demo.Entities;
using RESTful_API_Demo.Parameters;

namespace RESTful_API_Demo.Services
{
    public interface ICompanyRepository
    {
        Task<PagedList<Company>> GetCompaniesAsync(CompanyParameter parameter);
        Task<Company> GetCompanyAsync(Guid companyId);
        Task<IEnumerable<Company>> GetCompaniesAsync(IEnumerable<Guid> companyIds);
        void AddCompany(Company company);
        void UpdateCompany(Company company);
        void DeleteCompany(Company company);
        Task<bool> CompanyExistAsync(Guid companyId);

        Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameter parameter);
        Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, IEnumerable<Guid> employeeIds);
        Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId);
        void AddEmployee(Guid companyId, Employee employee);
        void UpdateEmployee(Employee employee);
        void DeleteEmployee(Employee employee);
        Task<bool> SaveAsync();
    }
}
