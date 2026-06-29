using System.Text;
using CentralAlertas.Api.Security;
using CentralAlertas.Api.Workers.Heartbeat;
using CentralAlertas.Application;
using CentralAlertas.Application.Authentication;
using CentralAlertas.Application.Cors;
using CentralAlertas.Infrastructure;
using CentralAlertas.Infrastructure.Authentication;
using CentralAlertas.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

var jwtOptions = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtOptions>() ?? new JwtOptions();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(jwtOptions.Secret);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorization();

// CORS dinâmico: as origens permitidas vêm da configuração (Cors:AllowedOrigins,
// via env Cors__AllowedOrigins) somadas às origens cadastradas no banco (CRUD).
// O DynamicCorsPolicyProvider decide por requisição consultando o cache.
builder.Services.AddCors();
builder.Services.AddSingleton<ICorsPolicyProvider, DynamicCorsPolicyProvider>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Central Alertas API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT no campo abaixo."
    });

    options.AddSecurityRequirement(_ =>
        new OpenApiSecurityRequirement
        {
            { new OpenApiSecuritySchemeReference("Bearer"), [] }
        });
});

builder.Services.Configure<HeartbeatWorkerOptions>(
    builder.Configuration.GetSection("Workers:Heartbeat"));

builder.Services.AddHostedService<HeartbeatWorker>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Central Alertas API v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseCors("spa");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "central-alertas-api",
    timestamp = DateTime.UtcNow
}));

using var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider
    .GetRequiredService<CentralAlertasDbContext>();

dbContext.Database.Migrate();

var adminUserSeeder = scope.ServiceProvider
    .GetRequiredService<AdminUserSeeder>();

await adminUserSeeder.SeedAsync();

var dashboardViewSeeder = scope.ServiceProvider
    .GetRequiredService<DashboardViewSeeder>();

await dashboardViewSeeder.SeedAsync();

// Carrega as origens de CORS cadastradas no banco para o cache em memória.
var allowedOriginsCache = scope.ServiceProvider
    .GetRequiredService<IAllowedOriginsCache>();

await allowedOriginsCache.RefreshAsync();

app.Run();
