using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RESTful_API_Demo.Data;

namespace RESTful_API_Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<RoutineDBContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.Migrate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(e, "Êý¾Ý¿âÇ¨ÒÆÊ§°Ü£¡");
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
