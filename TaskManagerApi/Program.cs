using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagerApi.Data;
using TaskManagerApi.Services;
using TaskManagerApi.Services.EnvLoad;

EnvConfig.Load();

var builder = WebApplication.CreateBuilder(args);

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
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

// --- JWT ---
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

// --- CORREÇÃO AQUI ---
// No seu código estava "builder.services" (minúsculo), o que causa um erro.
// O correto é "builder.Services" (maiúsculo).
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

// Bloco de Migração Automática (Está correto)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TaskDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
            Console.WriteLine("Migrações do banco de dados aplicadas com sucesso.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao aplicar as migrações do banco de dados.");
        // Adicionado 'throw' para impedir que a app inicie se o banco falhar.
        // Isso é mais seguro para o deploy.
        throw; 
    }
}

app.UseCors("AllowAllHeaders");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();