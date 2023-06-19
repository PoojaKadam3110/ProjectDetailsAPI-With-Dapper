using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Domain_Data.Mappings;

namespace Infrastructure
{
    public class DependencyInjection
    {
        public static void AddInfrastructure(IServiceCollection services)
        {
            services.AddScoped<IProjectsRepo, ProjectsRepo>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddLogging();
            services.AddAutoMapper(typeof(AutoMapperProfiles));

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
        }
    }
}
