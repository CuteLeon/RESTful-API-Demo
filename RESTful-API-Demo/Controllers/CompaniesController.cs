using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RESTful_API_Demo.Assists;
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

        [HttpGet(Name = nameof(GetCompanies))]
        [HttpHead]
        public async Task<IActionResult> GetCompanies(
            [FromQuery]CompanyParameter parameter)
        {
            var companiesPagedList = await this.companyRepository.GetCompaniesAsync(parameter);
            // 构造分页信息
            var previousPageLink = companiesPagedList.HasPrevious ? this.CreateCompnaiesUri(parameter, UriTypes.Previous) : default;
            var nextPageLink = companiesPagedList.HasNext ? this.CreateCompnaiesUri(parameter, UriTypes.Next) : default;
            var paginationMetadata = new
            {
                currentPage = companiesPagedList.CurrentPage,
                totalPages = companiesPagedList.TotalPages,
                pageSize = companiesPagedList.PageSize,
                totalCount = companiesPagedList.TotalCount,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink,
            };
            // 将分页信息记录到响应头
            this.Response.Headers.Add(
                "X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));
            var companyDtos = this.mapper.Map<IEnumerable<CompanyDTO>>(companiesPagedList);
            var shapedCompanies = companyDtos.ShapeData(parameter.Fields);
            return this.Ok(shapedCompanies);
        }

        /// <summary>
        /// 创建前一页和后一页的链接
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="uriType"></param>
        /// <returns></returns>
        private string CreateCompnaiesUri(CompanyParameter parameter, UriTypes uriType)
        {
            switch (uriType)
            {
                case UriTypes.Previous:
                    return this.Url.Link(
                        nameof(GetCompanies),
                        new
                        {
                            fields = parameter.Fields,
                            orderBy = parameter.OrderBy,
                            pageNumber = parameter.PageNumber - 1,
                            pageSize = parameter.PageSize,
                            companyName = parameter.CompanyName,
                            searchTerm = parameter.SearchTerm,
                        });

                case UriTypes.Next:
                    return this.Url.Link(
                        nameof(GetCompanies),
                        new
                        {
                            fields = parameter.Fields,
                            orderBy = parameter.OrderBy,
                            pageNumber = parameter.PageNumber + 1,
                            pageSize = parameter.PageSize,
                            companyName = parameter.CompanyName,
                            searchTerm = parameter.SearchTerm,
                        });

                default:
                    return this.Url.Link(
                        nameof(GetCompanies),
                        new
                        {
                            fields = parameter.Fields,
                            orderBy = parameter.OrderBy,
                            pageNumber = parameter.PageNumber,
                            pageSize = parameter.PageSize,
                            companyName = parameter.CompanyName,
                            searchTerm = parameter.SearchTerm,
                        });
            }
        }

        [HttpGet("{companyId}", Name = nameof(GetCompany))]
        public async Task<IActionResult> GetCompany(Guid companyId, string fields)
        {
            var company = await this.companyRepository.GetCompanyAsync(companyId);
            if (company == null)
            {
                return this.NotFound();
            }

            var companyDto = this.mapper.Map<CompanyDTO>(company);
            var shapedCompany = companyDto.ShapeData(fields);
            return this.Ok(shapedCompany);
        }

        [HttpPost]
        public async Task<ActionResult<CompanyDTO>> CreateCompany(
            [FromBody]CompanyCreateDTO companyCreateDTO)
        {
            var company = this.mapper.Map<Company>(companyCreateDTO);
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
            this.Response.Headers.Add("Allow", "GET,POST,OPTIONS");
            return this.Ok();
        }

        [HttpDelete("{companyId}")]
        public async Task<ActionResult> DeleteCompany(Guid companyId)
        {
            var company = await this.companyRepository.GetCompanyAsync(companyId);
            if (company == null)
            {
                return this.NotFound();
            }

            this.companyRepository.DeleteCompany(company);
            await this.companyRepository.SaveAsync();
            return this.NoContent();
        }
    }
}