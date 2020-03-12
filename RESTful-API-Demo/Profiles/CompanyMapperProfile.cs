using AutoMapper;
using RESTful_API_Demo.DTOS;
using RESTful_API_Demo.Entities;

namespace RESTful_API_Demo.Profiles
{
    // Company 与 CompanyDTO 类型在 AutoMapper 的映射配置类
    public class CompanyMapperProfile : Profile
    {
        public CompanyMapperProfile()
        {
            // CreateMap 创建默认映射，也可以使用 ForMember 实现自定义的成员映射逻辑
            this.CreateMap<Company, CompanyDTO>()
                .ForMember(
                dest => dest.Name,
                option => option.MapFrom(src => src.Name));

            this.CreateMap<CompanyCreateDTO, Company>();
        }
    }
}
