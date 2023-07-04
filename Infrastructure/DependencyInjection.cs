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
using System.Data;
using System.Data.SqlClient;
using Data;
using Microsoft.EntityFrameworkCore;

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

            string connectionString = "Server=10.235.3.8\\SQL2019STDMPNNEW;DataBase=TSProjectDetailsZoho;User=sa;Password=zcon@123;TrustServerCertificate=true;";

            services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
        }
    }
}
