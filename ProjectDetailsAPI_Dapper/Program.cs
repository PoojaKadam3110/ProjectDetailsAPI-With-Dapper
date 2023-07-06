using Application.Extensions;
using Application.Middlewares;
using Data;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;

// for cors
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000").AllowAnyHeader().WithMethods("POST", "PUT", "DELETE", "GET");
                      });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//For environment
Environment.SetEnvironmentVariable("ASPNETCORE_APIURL", builder.Configuration.GetSection("Urls").GetSection("APIURL").Value);
var env = Environment.GetEnvironmentVariable("ASPNETCORE_APIURL");


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())   
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();
builder.Services.AddSingleton(configuration);

//For Autorization
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Project Details API", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


// For DbContext()
builder.Services.AddDbContext<ProjectDetailsDbContext>(options =>
 options.UseSqlServer(builder.Configuration.GetConnectionString("ProjectDetailsConnectionStrings")));

// For DbContext()
builder.Services.AddDbContext<ProjectDetailsAuthDbContext>(options =>
 options.UseSqlServer(builder.Configuration.GetConnectionString("ProjectsAuthConnectionStrings")));


//For Dependency injection
AddInfrastructure(builder.Services);



//for Token
builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("ProjectDetails")
    .AddEntityFrameworkStores<ProjectDetailsAuthDbContext>()
    .AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 7;
    options.Password.RequiredUniqueChars = 1;
});


//End of Token
//For Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    });


//End of Authentication

var app = builder.Build();

//For Environment and configuration
IConfiguration Configuration = app.Configuration;

Console.WriteLine("Current Environment is: " + app.Environment.EnvironmentName);
Console.WriteLine("MyConfig value is : " + Configuration.GetValue<string>("MyConfig"));

app.ConfigureBuiltinExceptionHandler(app.Environment);
app.ConfigureExceptionHandler(app.Environment);
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();
app.Run();
static void AddInfrastructure(IServiceCollection services)
{
    DependencyInjection.AddInfrastructure(services);
}


CreateHostBuilder(args).Build().Run();
//static IHostBuilder CreateHostBuilder(string[] args)
//{
//    Host.CreateDefaultBuilder(args)
//        .ConfigureAppConfiguration((a, configuration) =>
//        {
//            configuration.AddJsonFile("appsettings.json");
//            configuration.AddJsonFile($"appsettings.json.{}.json");
//        })
//}

static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath);

                    // Specify the environment-specific configuration file based on the current environment
                    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    // Exclude the original appsettings.json file from being loaded
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                    // Add other configuration sources if needed
                    // ...
                });

