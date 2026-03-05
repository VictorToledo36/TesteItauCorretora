using Microsoft.EntityFrameworkCore;
using TesteItauCorretora.Infrastructure.Persistence;
using TesteItauCorretora.Domain.Gateway;
using TesteItauCorretora.Infrastructure.Gateway;
using TesteItauCorretora.Core.UseCase.Clientes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// Registrar Repositories
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IContaGraficaRepository, ContaGraficaRepository>();

// Registrar UseCases
builder.Services.AddScoped<AdesaoClienteUseCase>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();