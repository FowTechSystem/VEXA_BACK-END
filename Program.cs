using System.Text;
using System.Threading.RateLimiting;
using CRMVEXA.API.Middleware;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using CRMVEXA.Infrastructure.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// URLs de escuta (ordem):
// 1) "Urls" no appsettings / variáveis de ambiente (ex.: Urls=http://0.0.0.0:8080)
// 2) ASPNETCORE_URLS (Docker, systemd, IIS, launchSettings) — se definido, não chamar UseUrls
// 3) fallback: https://localhost:5001 (máquina local sem env)
var urlsConfig = builder.Configuration["Urls"];
if (!string.IsNullOrWhiteSpace(urlsConfig))
{
    builder.WebHost.UseUrls(urlsConfig.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
}
else if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.UseUrls("https://localhost:5001");
}

// ============ CONFIGURAÇÕES DE SERVIÇOS ============

// Corrige problema de DateTime com PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEmpresaContext, EmpresaContext>();
builder.Services.AddScoped<IEntradaEstoquePorCompraService, EntradaEstoquePorCompraService>();
if (!string.IsNullOrWhiteSpace(builder.Configuration["Email:SmtpHost"]))
    builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
else
    builder.Services.AddScoped<IEmailSender, LogOnlyEmailSender>();

// 1. Configurar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    // Permite rodar "dotnet ef database update" mesmo com pequenas diferenças modelo/snapshot
    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// 2. AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// 3. MediatR - REGISTRA AUTOMATICAMENTE TODOS OS HANDLERS
builder.Services.AddMediatR(cfg =>
{
    var assemblies = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => a.FullName != null && a.FullName.StartsWith("CRMVEXA"))
        .ToArray();

    cfg.RegisterServicesFromAssemblies(assemblies);
});

// 4. Controllers COM CONFIGURAÇÃO JSON (exige autenticação por padrão)
builder.Services.AddControllers(o =>
{
    o.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Resposta de validação no formato do frontend (sucesso, mensagem, message, errors)
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());
        var firstError = context.ModelState
            .SelectMany(m => m.Value?.Errors ?? Enumerable.Empty<Microsoft.AspNetCore.Mvc.ModelBinding.ModelError>())
            .FirstOrDefault();
        var message = firstError?.ErrorMessage ?? "Um ou mais erros de validação.";
        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new
        {
            sucesso = false,
            mensagem = message,
            message,
            errors
        });
    };
});

// 5. CORS — dev local + entradas em Cors:AdditionalOrigins (ex.: front na VPS :8081)
var corsBaseOrigins = new[]
{
    "http://localhost:3000",
    "http://localhost:5173",
    "http://localhost:5174",
    "http://localhost:5175",
    "http://localhost:8080",
    "https://localhost:3000",
    "https://localhost:5173",
    "https://localhost:5174",
    "https://localhost:5175",
    "https://localhost:8080"
};
var corsExtra = builder.Configuration.GetSection("Cors:AdditionalOrigins").Get<string[]>() ?? Array.Empty<string>();
var corsOrigins = corsBaseOrigins
    .Concat(corsExtra.Where(static o => !string.IsNullOrWhiteSpace(o)))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// 6. JWT - Autenticação
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "ChaveSecretaJwtCRMVEXA2026Minimo32Caracteres!";
var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwtKey,
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "CRMVEXA",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "CRMVEXA",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, _) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            message = "Muitas tentativas. Aguarde alguns minutos e tente novamente.",
            Message = "Muitas tentativas. Aguarde alguns minutos e tente novamente."
        }, cancellationToken: context.HttpContext.RequestAborted);
    };
    options.AddPolicy("auth-password-recovery", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            ip,
            _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(15),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });
});

// 7. Swagger - ✅ VERSÃO SIMPLES (SEM ERRO)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============ BUILD DA APLICAÇÃO ============
var app = builder.Build();

// ============ MIDDLEWARE PIPELINE ============

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseRateLimiter();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseAuthentication();
app.UseMiddleware<EmpresaContextMiddleware>();
app.UseAuthorization();

// Rotas mínimas antes dos controllers (/, /health) — após deploy: GET / e GET /health devem retornar 200, não 404
app.MapGet("/", () => Results.Json(new
{
    name = "CRMVEXA API",
    ok = true,
    endpoints = new
    {
        health = "/health",
        login = "POST /api/Auth/login",
        forgotPassword = "POST /api/Auth/forgot-password",
        resetPassword = "POST /api/Auth/reset-password"
    }
})).AllowAnonymous();

app.MapGet("/health", () => Results.Json(new { status = "ok", service = "CRMVEXA" }))
    .AllowAnonymous()
    .WithName("Health");

app.MapControllers();

app.Run();
