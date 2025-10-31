using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagerApi.Data;
using TaskManagerApi.Services;
using TaskManagerApi.Services.EnvLoad;

// --- CORREÇÃO 1: Mover EnvConfig.Load() para ANTES do builder ---
// Isso garante que as variáveis de ambiente do .env existam
// antes que o builder tente lê-las.
EnvConfig.Load();

var builder = WebApplication.CreateBuilder(args);

// Esta linha agora é lida corretamente após o EnvConfig.Load()
builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration["ConnectionStrings:Default"];
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];


if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("Defina Jwt:Key.");

if (string.IsNullOrWhiteSpace(jwtIssuer))
    throw new InvalidOperationException("Defina Jwt:Issuer.");

if (string.IsNullOrWhiteSpace(jwtAudience))
    throw new InvalidOperationException("Defina Jwt:Audience.");

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseNpgsql(connectionString)
);

var rawOrigins = builder.Configuration["Cors:AllowedOrigins"];
var origins = (rawOrigins ?? "")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(o => o.TrimEnd('/')) // remove barra final se vier
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllHeaders", policy =>
    {
        if (origins.Length == 0)
        {
            // DEV fallback: permite tudo (sem cookies)
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod();
            // Se usar cookies/autenticação via cookie: adicione .AllowCredentials()
            // e NÃO use AllowAnyOrigin nesse caso.
        }
    });
});

// --- JWT ---
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Controllers + OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// --- CORREÇÃO 2: Reordenar o pipeline da aplicação ---

// Mantenha o HTTPS comentado se você está testando em HTTP
//app.UseHttpsRedirection();

// 1. CORS deve vir antes de Autenticação/Autorização
app.UseCors("AllowAllHeaders");

// 2. Autenticação (Quem é você?) deve vir ANTES de Autorização
app.UseAuthentication();

// 3. Autorização (O que você pode fazer?) deve vir DEPOIS de Autenticação
app.UseAuthorization();

app.MapControllers();

app.Run();