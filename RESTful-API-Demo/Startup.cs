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
                    // �Զ����޷����ܵ� Accept Header ���� 406 ����
                    setup.ReturnHttpNotAcceptable = true;
                })
                // ���ڽ��Ĭ�ϵ� Json ���޷����� JsonPatchDocument<> ���͵�����
                .AddNewtonsoftJson(setup =>
                {
                    setup.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                })
                // ���� XML ��ʽ֧��
                .AddXmlDataContractSerializerFormatters()
                // �Զ���ģ����֤������Ϣ
                .ConfigureApiBehaviorOptions(setup =>
                {
                    setup.InvalidModelStateResponseFactory = context =>
                    {
                        var detail = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "https://github.com/CuteLeon",
                            Title = "�����쳣",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "��ϸ��Ϣ",
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
                // Ϊ NewtonSoft.Json �����ʽ���������Զ���� MediaType ֧��
                var newtonsoftJsonOutputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
                if (newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.company.hateoas+json");
                }
            });

            // ʹ AutoMapper �Զ�����Ӧ�����ڵ�����ӳ�������� (Profile)
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
                // ȫ���쳣������ʽ
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("δ��������쳣��");
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
