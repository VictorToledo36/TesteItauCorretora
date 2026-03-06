using Microsoft.EntityFrameworkCore;
using TesteItauCorretora.Infrastructure.Persistence;
using TesteItauCorretora.Domain.Gateway;
using TesteItauCorretora.Infrastructure.Gateway;
using TesteItauCorretora.Domain.UseCase.Clientes;
using TesteItauCorretora.Domain.UseCase.Adm;
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
builder.Services.AddScoped<ICestaRecomendacaoRepository, CestaRecomendacaoRepository>();
builder.Services.AddScoped<ICustodiaRepository, CustodiaRepository>();
builder.Services.AddScoped<ICotacaoRepository, CotacaoRepository>();
builder.Services.AddScoped<IDistribuicaoRepository, DistribuicaoRepository>();

// Registrar UseCases
builder.Services.AddScoped<AdesaoClienteUseCase>();
builder.Services.AddScoped<CadastrarCestaUseCase>();
builder.Services.AddScoped<SaidaClienteUseCase>();
builder.Services.AddScoped<AlterarValorMensalUseCase>();
builder.Services.AddScoped<ConsultarCarteiraUseCase>();
builder.Services.AddScoped<ConsultarRentabilidadeUseCase>();

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