using Microsoft.EntityFrameworkCore;
using OnibusExpress.Application.Interfaces;
using OnibusExpress.Application.Services;
using OnibusExpress.Domain.Interfaces;
using OnibusExpress.Infrastructure.Context;
using OnibusExpress.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Banco de Dados PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositórios (Infrastructure) implementam as interfaces do Domain
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IViagemRepository, ViagemRepository>();
builder.Services.AddScoped<IPassageiroRepository, PassageiroRepository>();
builder.Services.AddScoped<IRotaRepository, RotaRepository>();

builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IViagemService, ViagemService>();
builder.Services.AddScoped<IRotaService, RotaService>();

// Tratamento global de exceções (Middleware/ExcecaoGlobalHandler)
builder.Services.AddExceptionHandler<OnibusExpress.Api.Middleware.ExcecaoGlobalHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS: qualquer origem só em Development; nos demais ambientes apenas as origens de "Cors:Origens"
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
            policy.AllowAnyOrigin();
        else
            policy.WithOrigins(builder.Configuration.GetSection("Cors:Origens").Get<string[]>() ?? []);

        policy.AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// --- INÍCIO DA MIGRATION AUTOMÁTICA E SEED ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // 1. Puxa o contexto e cria as tabelas no banco de dados vazio
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();

        // 2. Popula o banco usando o mesmo contexto do escopo
        DbSeeder.Seed(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogCritical(ex, "Ocorreu um erro ao rodar as migrations na inicialização.");
        // Sem banco a API não funciona: falhar aqui deixa o erro visível em vez de subir quebrada
        throw;
    }
}
// FIM DA MIGRATION AUTOMÁTICA E SEED

app.UseExceptionHandler();

// Configura o Swagger para documentação da API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();

app.Run();