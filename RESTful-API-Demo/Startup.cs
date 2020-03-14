using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using RESTful_API_Demo.Data;
using RESTful_API_Demo.PropertyMappingServices;
using RESTful_API_Demo.Services;

namespace RESTful_API_Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(setup =>
                {
                    // 自动对无法接受的 Accept Header 返回 406 代码
                    setup.ReturnHttpNotAcceptable = true;
                })
                // 用于解决默认的 Json 库无法解析 JsonPatchDocument<> 类型的问题
                .AddNewtonsoftJson(setup =>
                {
                    setup.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                })
                // 启用 XML 格式支持
                .AddXmlDataContractSerializerFormatters()
                // 自定义模型验证错误信息
                .ConfigureApiBehaviorOptions(setup =>
                {
                    setup.InvalidModelStateResponseFactory = context =>
                    {
                        var detail = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "https://github.com/CuteLeon",
                            Title = "发生异常",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "详细信息",
                            Instance = context.HttpContext.Request.Path,
                        };
                        detail.Extensions.Add("TraceId", context.HttpContext.TraceIdentifier);

                        return new UnprocessableEntityObjectResult(detail)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                });

            services.Configure<MvcOptions>(config =>
            {
                // 为 NewtonSoft.Json 输出格式化器增加自定义的 MediaType 支持
                var newtonsoftJsonOutputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
                if (newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.company.hateoas+json");
                }
            });

            // 使 AutoMapper 自动发现应用域内的类型映射配置类 (Profile)
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddPropertyMappingService();

            services.AddDbContext<RoutineDBContext>(option =>
            {
                option.UseSqlite("Data Source=RestfulAPI.db");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // 全局异常处理表达式
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("未被处理的异常！");
                    });
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
