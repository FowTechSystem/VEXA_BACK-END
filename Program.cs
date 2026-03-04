using System.Text;
using CRMVEXA.API.Middleware;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://localhost:5001");

// ============ CONFIGURAÇÕES DE SERVIÇOS ============

// Corrige problema de DateTime com PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEmpresaContext, EmpresaContext>();

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

// 5. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
              "http://localhost:3000",
              "http://localhost:5173",
              "http://localhost:5174",
              "http://localhost:5175",
              "http://localhost:8080",
              "https://localhost:3000",
              "https://localhost:5173",
              "https://localhost:5174",
              "https://localhost:5175",
              "https://localhost:8080")
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
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseAuthentication();
app.UseMiddleware<EmpresaContextMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
