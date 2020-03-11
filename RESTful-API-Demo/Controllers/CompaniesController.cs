using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTful_API_Demo.DTOS;
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

        public CompaniesController(
            ICompanyRepository companyRepository)
        {
            this.companyRepository = companyRepository ??
                throw new ArgumentNullException(nameof(companyRepository));
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await this.companyRepository.GetCompaniesAsync();
            var companyDtos = companies
                .Select(company => new CompanyDTO()
                {
                    Id = company.Id,
                    Name = company.Name,
                })
                .ToList();
            return this.Ok(companyDtos);
        }

        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetCompanies(Guid companyId)
        {
            var company = await this.companyRepository.GetCompanyAsync(companyId);
            if (company == null)
            {
                return this.NotFound();
            }
            return this.Ok(company);
        }
    }
}