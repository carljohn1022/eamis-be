using EAMIS.Core.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
         
            try
            {
                var ctx = services.GetRequiredService<EAMISContext>();
                var aisctx = services.GetRequiredService<AISContext>();
                await ctx.Database.MigrateAsync();
                await aisctx.Database.MigrateAsync();
                
            }
            catch(Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration");
            }
            await host.RunAsync();

        }
        public static string BaseCreatedDirectory
        {
            get
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"StaticFiles\ExcelTemplates");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {

                    webBuilder.UseIISIntegration();
                    webBuilder.UseStartup<Startup>();
                  
                });
    }
}
