using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ============ CONFIGURAÇÕES DE SERVIÇOS ============

// Corrige problema de DateTime com PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// 1. Configurar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// 2. MediatR - REGISTRA AUTOMATICAMENTE TODOS OS HANDLERS
builder.Services.AddMediatR(cfg =>
{
    var assemblies = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => a.FullName != null && a.FullName.StartsWith("CRMVEXA"))
        .ToArray();

    cfg.RegisterServicesFromAssemblies(assemblies);
});

// 3. Controllers COM CONFIGURAÇÃO JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// 4. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
              "http://localhost:3000",
              "http://localhost:5173",
              "http://localhost:8080",
              "https://localhost:3000",
              "https://localhost:5173",
              "https://localhost:8080")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// 5. Swagger - ✅ VERSÃO SIMPLES (SEM ERRO)
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
app.UseAuthorization();
app.MapControllers();

app.Run();
