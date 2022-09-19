using AuthenticationService;
using Data.DataAccess;
using Data.Models;
using Data.Models.Email;
using Data.Settings;
using EmailingService.Contracts;
using EmailingService.Impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PasswordEncryption.Contracts;
using PasswordEncryption.Impl;
using PasswordManager.ActionFilters;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization using bearer token : \"Bearer {Token}\" ",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});


var mongoDbConfig = builder.Configuration.GetSection(nameof(MongoDbConfig)).Get<MongoDbConfig>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(
    opt =>
    {
        opt.SignIn.RequireConfirmedEmail = true;

    })
        .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
        (
            mongoDbConfig.ConnectionString, mongoDbConfig.Name
        ).AddDefaultTokenProviders();

// Inject mongoDbConfig
builder.Services.AddSingleton(mongoDbConfig);

// Inject Mongodb Data Access repo
builder.Services.AddSingleton<IMongoDbDataAccess, MongoDbDataAccess>();
// Inject userDataRepository
builder.Services.AddSingleton<IUserDataRepository, UserDataRepository>();

//Registered Action filter
builder.Services.AddScoped<GetCurrentUserActionFilter>();

builder.Services.AddHttpContextAccessor();
//creating a logger from configuration
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .CreateLogger();

//Removing default logging providers
builder.Logging.ClearProviders();

//Adding serilog as a logger
builder.Logging.AddSerilog(logger);

// Email config
var emailConfig = builder.Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

// Add Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowCredentials().AllowAnyHeader());
});

// Dependency Injection goes here
builder.Services.AddScoped<ISymmetricEncryptDecrypt, SymmetricEncryptDecrypt>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPrettyEmail, PrettyEmail>();
builder.Services.AddScoped<ITokensManager, TokensManager>();


//Adding authentication scheme
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:SecretKey").Value)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CorsPolicy");
app.MapControllers();

try
{
    logger.Information("Application is starting");
    app.Run();

}
catch (Exception ex)
{
    logger.Fatal(ex, "Application has not started properly");
}