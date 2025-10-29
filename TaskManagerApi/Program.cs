using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagerApi.Data;
using TaskManagerApi.Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    Env.Load();
}

builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration["ConnectionStrings__Default"]?? builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseNpgsql(connectionString)
);

var allowedOrigins = builder.Configuration["Cors__AllowedOrigins"]?
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? ["*", "*"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

var jwtKey = builder.Configuration["Jwt__Key"];
var jwtIssuer = builder.Configuration["Jwt__Issuer"];
var jwtAudience = builder.Configuration["Jwt__Audience"];

byte[] key;

if (string.IsNullOrEmpty(jwtKey))
{
    var jwtSection = builder.Configuration.GetSection("Jwt");
    key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);
}
else
{
    builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
    builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
    key = Encoding.UTF8.GetBytes(jwtKey);
}



builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
