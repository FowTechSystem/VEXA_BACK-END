using CRMVEXA.Application.Features.Clientes.CriarCliente;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============ CONFIGURAÇÕES DE SERVIÇOS ============

// 1. Configurar DbContext (Entity Framework com PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// 2. Registrar MediatR (CQRS Pattern)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CriarClienteRequest).Assembly));

// 3. Adicionar Controllers
builder.Services.AddControllers();

// 4. Configurar CORS para React/Vue frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
              "http://localhost:3000",
              "http://localhost:5173",
              "https://localhost:3000",
              "https://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// 5. Swagger (opcional - para documentação da API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============ BUILD DA APLICAÇÃO ============
var app = builder.Build();

// ============ MIDDLEWARE PIPELINE ============

// Swagger apenas em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

// CORS antes de Authorization
app.UseCors("AllowFrontend");

app.UseAuthorization();

// Mapear Controllers
app.MapControllers();

app.Run();
