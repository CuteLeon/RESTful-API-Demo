using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RESTful_API_Demo.ActionConstraints;
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
            shapedCompanies = shapedCompanies
                .Select(shapedCompany =>
                {
                    var dictionary = shapedCompany as IDictionary<string, object>;
                    dictionary.Add(
                        "links",
                        this.CreateLinksForCompany(
                            (Guid)dictionary[nameof(CompanyDTO.Id)],
                            null));
                    return shapedCompany;
                }).ToList();
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

                case UriTypes.Current:
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

        [Produces(
            "application/json",
            "application/vnd.company.hateoas+json")]
        [HttpGet("{companyId}", Name = nameof(GetCompany))]
        public async Task<IActionResult> GetCompany(
            Guid companyId,
            string fields,
            [FromHeader(Name = "Accept")]string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType))
            {
                return this.BadRequest();
            }

            var company = await this.companyRepository.GetCompanyAsync(companyId);
            if (company == null)
            {
                return this.NotFound();
            }

            var companyDto = this.mapper.Map<CompanyDTO>(company);
            var shapedCompany = companyDto.ShapeData(fields);


            if (parsedMediaType.MediaType == "application/vnd.company.hateoas+json")
            {
                var links = this.CreateLinksForCompany(companyId, fields);
                shapedCompany.TryAdd("links", links);

                return this.Ok(shapedCompany);
            }
            else
            {
                return this.Ok(shapedCompany);
            }
        }

        [HttpPost(Name = nameof(CreateCompany))]
        [RequestHeaderMatchsMediaType("Content-Type", "application/json", "application/vnd.company.hateoas+json")]
        [Consumes("application/json", "application/vnd.company.hateoas+json")]
        public async Task<ActionResult<CompanyDTO>> CreateCompany(
            [FromBody]CompanyCreateDTO companyCreateDTO)
        {
            var company = this.mapper.Map<Company>(companyCreateDTO);
            this.companyRepository.AddCompany(company);
            await this.companyRepository.SaveAsync();
            var companyDTO = this.mapper.Map<CompanyDTO>(company);
            var links = this.CreateLinksForCompany(company.Id, null);
            var shapedCompany = companyDTO.ShapeData(null);
            shapedCompany.TryAdd("links", links);

            // CreatedAtRoute 可以在响应的Location头返回一个路由(使用RouteName定义)用于定位新创建的资源
            return this.CreatedAtRoute(
                nameof(GetCompany),
                // 匿名类型的成员名称需要与路由模板的参数匹配
                new { companyId = company.Id },
                shapedCompany);
        }

        [HttpOptions]
        public async Task<ActionResult> GetCompaniesOptions()
        {
            this.Response.Headers.Add("Allow", "GET,POST,OPTIONS");
            return this.Ok();
        }

        [HttpDelete("{companyId}", Name = nameof(DeleteCompany))]
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

        private IEnumerable<LinkDTO> CreateLinksForCompany(Guid companyId, string fields)
        {
            var links = new List<LinkDTO>();

            if (string.IsNullOrEmpty(fields))
            {
                links.Add(new LinkDTO(this.Url.Link(nameof(GetCompany), new { companyId }), "self", "GET"));
            }
            else
            {
                links.Add(new LinkDTO(this.Url.Link(nameof(GetCompany), new { companyId, fields }), "self", "GET"));
            }

            links.Add(new LinkDTO(this.Url.Link(nameof(DeleteCompany), new { companyId }), "delete_company", "DELETE"));
            links.Add(new LinkDTO(this.Url.Link(nameof(EmployeesController.CreateEmployeeForCompany), new { companyId }), "create_employee_for_company", "POST"));
            links.Add(new LinkDTO(this.Url.Link(nameof(EmployeesController.GetEmployeeForCompany), new { companyId }), "get_employee_for_company", "GET"));
            return links;
        }
    }
}