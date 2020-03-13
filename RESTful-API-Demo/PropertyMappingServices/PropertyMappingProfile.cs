using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using RESTful_API_Demo.DTOS;
using RESTful_API_Demo.Entities;

namespace RESTful_API_Demo.PropertyMappingServices
{
    public static class PropertyMappingProfile
    {
        public static IServiceCollection AddPropertyMappingService(this IServiceCollection services)
        {
            var propertyMappingService = new PropertyMappingService();
            propertyMappingService
                .ConfigCompanyPropertyMappings()
                .ConfigEmployeePropertyMappings();

            services.AddSingleton<IPropertyMappingService>(propertyMappingService);
            return services;
        }

        public static IPropertyMappingService ConfigEmployeePropertyMappings(
            this IPropertyMappingService propertyMappingService)
        {
            var propertyMapping = new PropertyMapping<EmployeeDTO, Employee>(new Dictionary<string, PropertyMappingValue>
            {
                { nameof(EmployeeDTO.Id).ToLower(), new PropertyMappingValue(new []{ nameof(Employee.Id)})},
                { nameof(EmployeeDTO.CompanyId).ToLower(), new PropertyMappingValue(new []{ nameof(Employee.CompanyId)})},
                { nameof(EmployeeDTO.EmployeeNo).ToLower(), new PropertyMappingValue(new []{ nameof(Employee.EmployeeNo)})},
                { nameof(EmployeeDTO.Name).ToLower(), new PropertyMappingValue(new []{ nameof(Employee.FirstName),nameof(Employee.LastName)})},
                { nameof(EmployeeDTO.GenderDisplay).ToLower(), new PropertyMappingValue(new []{ nameof(Employee.Gender)})},
                { nameof(EmployeeDTO.Age).ToLower(), new PropertyMappingValue(new []{ nameof(Employee.DateOfBirth)}, true)},
            });

            propertyMappingService.AddPropertyMapping(propertyMapping);
            return propertyMappingService;
        }

        public static IPropertyMappingService ConfigCompanyPropertyMappings(
            this IPropertyMappingService propertyMappingService)
        {

            return propertyMappingService;
        }
    }
}
