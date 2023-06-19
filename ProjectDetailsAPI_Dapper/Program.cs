using Data;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;
using System.Data.SqlClient;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//For environment
Environment.SetEnvironmentVariable("ASPNETCORE_APIURL", builder.Configuration.GetSection("Urls").GetSection("APIURL").Value);
var env = Environment.GetEnvironmentVariable("ASPNETCORE_APIURL");



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
string connectionString = "Server=ZCONL-140\\SQL2016EXPADV;DataBase=TSProjectDetailsZohoDapper;User=sa;Password=zcon@123;Trusted_Connection=true; TrustServerCertificate=true; Integrated Security=true;";

builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));
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


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
static void AddInfrastructure(IServiceCollection services)
{
    DependencyInjection.AddInfrastructure(services);
}
