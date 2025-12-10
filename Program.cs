var builder = WebApplication.CreateBuilder(args);

// Adicionar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // portas comuns do Vite/React
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

// IMPORTANTE: UseCors DEVE vir ANTES de UseAuthorization
app.UseCors("AllowFrontend");

app.UseAuthorization();
app.MapControllers();

app.Run();
