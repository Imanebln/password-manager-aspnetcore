using AuthenticationService;
using Data.Models;
using Data.Settings;
using Serilog;
using PasswordEncryption.Contracts;
using PasswordEncryption.Impl;
using EmailingService.Contracts;
using EmailingService.Impl;
using Data.Models.Email;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var mongoDbConfig = builder.Configuration.GetSection(nameof(MongoDbConfig)).Get<MongoDbConfig>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(
    opt => {
        opt.SignIn.RequireConfirmedEmail = true;

    })
        .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
        (
            mongoDbConfig.ConnectionString, mongoDbConfig.Name
        ).AddDefaultTokenProviders();

builder.Services.AddScoped<ITokensManager,TokensManager>();
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

// Dependency Injection goes here
builder.Services.AddScoped<ISymmetricEncryptDecrypt, SymmetricEncryptDecrypt>();
builder.Services.AddScoped<IEmailService, EmailService>();

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