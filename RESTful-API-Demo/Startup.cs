using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RESTful_API_Demo.Data;
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
            services.AddControllers(setup =>
            {
                // 自动对无法接受的 Accept Header 返回 406 代码
                setup.ReturnHttpNotAcceptable = true;
                // 默认使用 Json，添加支持 XML 格式，自动根据 Accept Header 返回对应格式
                setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            });

            services.AddScoped<ICompanyRepository, CompanyRepository>();
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
