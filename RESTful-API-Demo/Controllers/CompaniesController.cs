using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RESTful_API_Demo.DTOS;
using RESTful_API_Demo.Entities;
using RESTful_API_Demo.Parameters;
using RESTful_API_Demo.Services;

namespace RESTful_API_Demo.Controllers
{
    /// <summary>
    /// 公司控制器
    /// </summary>
    /// <remarks>
    /// WebAPI 仅继承自 ControllerBase 即可
    /// </remarks>
    /// <document>
    /// ApiController 会启用以下行为：
    ///     1、要求使用属性路由（Attribute Routing）
    ///     2、自动HTTP 400响应
    ///     3、Multipart/form-data 请求推断
    ///     4、错误状态代码的问题详细信息
    /// </document>
    [ApiController]
    // [Route("api/[controller]")]
    [Route("api/companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository companyRepository;
        private readonly IMapper mapper;

        public CompaniesController(
            ICompanyRepository companyRepository,
            IMapper mapper)
        {
            this.companyRepository = companyRepository ??
                throw new ArgumentNullException(nameof(companyRepository));
            this.mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [HttpHead]
        public async Task<ActionResult<IEnumerable<CompanyDTO>>> GetCompanies(
            [FromQuery]CompanyParameter parameter)
        {
            var companies = await this.companyRepository.GetCompaniesAsync(parameter);
            var companyDtos = this.mapper.Map<IEnumerable<CompanyDTO>>(companies);
            return this.Ok(companyDtos);
        }

        [HttpGet("{companyId}", Name = nameof(GetCompany))]
        public async Task<IActionResult> GetCompany(Guid companyId)
        {
            var company = await this.companyRepository.GetCompanyAsync(companyId);
            if (company == null)
            {
                return this.NotFound();
            }

            var companyDto = this.mapper.Map<CompanyDTO>(company);
            return this.Ok(companyDto);
        }

        [HttpPost]
        public async Task<ActionResult<CompanyDTO>> CreateCompany(
            [FromBody]CompanyCreateDTO companyCreateDTO)
        {
            var company = mapper.Map<Company>(companyCreateDTO);
            this.companyRepository.AddCompany(company);
            await this.companyRepository.SaveAsync();
            var companyDTO = this.mapper.Map<CompanyDTO>(company);

            // CreatedAtRoute 可以在响应的Location头返回一个路由(使用RouteName定义)用于定位新创建的资源
            return this.CreatedAtRoute(
                nameof(GetCompany),
                // 匿名类型的成员名称需要与路由模板的参数匹配
                new { companyId = company.Id },
                companyDTO);
        }

        [HttpOptions]
        public async Task<ActionResult> GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET,POST,OPTIONS");
            return this.Ok();
        }
    }
}