using System.Text;
using CRMVEXA.API.Middleware;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// URLs de escuta — alterne manualmente antes de publicar no servidor (Docker/VPS + Nginx).
//
// Uso LOCAL (npm run dev / Vite): mantenha a linha LOCAL descomentada e a SERVIDOR comentada.
// Deploy no SERVIDOR: comente a linha LOCAL e descomente a linha SERVIDOR; depois build/push da imagem.
//
// LOCAL — front em http://localhost:5173 com proxy para https://localhost:5001
builder.WebHost.UseUrls("https://localhost:5001");

// SERVIDOR — HTTP na 8080 (TLS fica no Nginx na frente)
// builder.WebHost.UseUrls("http://+:8080");

// ============ CONFIGURAÇÕES DE SERVIÇOS ============

// Corrige problema de DateTime com PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEmpresaContext, EmpresaContext>();
builder.Services.AddScoped<IEntradaEstoquePorCompraService, EntradaEstoquePorCompraService>();

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
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseAuthentication();
app.UseMiddleware<EmpresaContextMiddleware>();
app.UseAuthorization();

// Rotas mínimas antes dos controllers (/, /health) — após deploy: GET / e GET /health devem retornar 200, não 404
app.MapGet("/", () => Results.Json(new
{
    name = "CRMVEXA API",
    ok = true,
    endpoints = new { health = "/health", login = "POST /api/Auth/login" }
})).AllowAnonymous();

app.MapGet("/health", () => Results.Json(new { status = "ok", service = "CRMVEXA" }))
    .AllowAnonymous()
    .WithName("Health");

app.MapControllers();

app.Run();
