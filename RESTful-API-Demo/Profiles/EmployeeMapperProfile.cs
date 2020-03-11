using System;
using AutoMapper;
using RESTful_API_Demo.DTOS;
using RESTful_API_Demo.Entities;

namespace RESTful_API_Demo.Profiles
{
    public class EmployeeMapperProfile : Profile
    {
        public EmployeeMapperProfile()
        {
            this.CreateMap<Employee, EmployeeDTO>()
                .ForMember(
                    dest => dest.Name,
                    option => option.MapFrom(src => $"{src.FirstName}{src.LastName}"))
                .ForMember(
                    dest => dest.Age,
                    option => option.MapFrom(src => DateTime.Now.Year - src.DateOfBirth.Year));
        }
    }
}
