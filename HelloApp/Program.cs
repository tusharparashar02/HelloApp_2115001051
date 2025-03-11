using BusinessLayer.Service;
using RepositoryLayer.Service;
using RepositoryLayer.Interface;
using BusinessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StackExchange.Redis;
using RabbitMqConsumerService.Service;
using RabbitMCQProducer.Service;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

//  Register Services
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<RabbitMqConsumer>();
builder.Services.AddSingleton<RabbitMqProducer>();

//  JWT Configuration
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new ArgumentNullException("JWT configuration values are missing in appsettings.json");
}

//  Add Authentication & Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

//  Add Controllers
builder.Services.AddControllers();
builder.Services.AddScoped<IRegisterHelloRL, RegisterHelloRL>();
builder.Services.AddScoped<IRegisterHelloBL, RegisterHelloBL>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<RedisCache>();

//  Database Connection
var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
builder.Services.AddDbContext<HelloAppContext>(options => options.UseSqlServer(connectionString));

//  Redis Connection
var redisConfig = builder.Configuration.GetSection("Redis:ConnectionString").Value;
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));

var app = builder.Build();

// Start RabbitMQ Consumer in Background
var rabbitMqConsumer = app.Services.GetRequiredService<RabbitMqConsumer>();
Task.Run(() => rabbitMqConsumer.StartListeningAsync()); 

// ? Configure Middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
