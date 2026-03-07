using Microsoft.EntityFrameworkCore;
using TesteItauCorretora.Infrastructure.Persistence;
using TesteItauCorretora.Domain.Gateway;
using TesteItauCorretora.Infrastructure.Gateway;
using TesteItauCorretora.Domain.UseCase.Clientes;
using TesteItauCorretora.Domain.UseCase.Adm;
using TesteItauCorretora.Core.UseCase.Clientes;
using TesteItauCorretora.Domain.UseCases;
using TesteItauCorretora.Domain.UseCase;
using TesteItauCorretora.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using TesteItauCorretora.Middleware;
using Microsoft.OpenApi.Models;
using TesteItauCorretora.API.Middleware;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Itau Corretora Sistema de Compra Programada",
        Version = "v1",
        Description = "API para gerenciamento do sistema de compra programada de acoes da Itau Corretora.",
        Contact = new OpenApiContact
        {
            Name = "Victor Toledo"
        }
    });

    options.EnableAnnotations();
    options.OperationFilter<RequestIdHeaderFilter>();
});

//DataBase
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

//Kafka
builder.Services.Configure<KafkaSettings>(
    builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<IEventoIRPublisher, EventoIRPublisher>();

// Registrar Repositories
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IContaGraficaRepository, ContaGraficaRepository>();
builder.Services.AddScoped<ICestaRecomendacaoRepository, CestaRecomendacaoRepository>();
builder.Services.AddScoped<ICustodiaRepository, CustodiaRepository>();
builder.Services.AddScoped<ICotacaoRepository, CotacaoRepository>();
builder.Services.AddScoped<IDistribuicaoRepository, DistribuicaoRepository>();
builder.Services.AddScoped<ConsultarCustodiaMasterUseCase>();
builder.Services.AddScoped<IOrdemCompraRepository, OrdemCompraRepository>();
builder.Services.AddScoped<IEventoIRPublisher, EventoIRPublisher>();

// Registrar UseCases
builder.Services.AddScoped<AdesaoClienteUseCase>();
builder.Services.AddScoped<CadastrarCestaUseCase>();
builder.Services.AddScoped<SaidaClienteUseCase>();
builder.Services.AddScoped<AlterarValorMensalUseCase>();
builder.Services.AddScoped<ConsultarCarteiraUseCase>();
builder.Services.AddScoped<ConsultarRentabilidadeUseCase>();
builder.Services.AddScoped<ConsultarCestaAtualUseCase>();
builder.Services.AddScoped<ConsultarHistoricoCestasUseCase>();
builder.Services.AddScoped<MotorCompra>();

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

app.UseMiddleware<RequestIdMiddleware>();
app.UseHttpsRedirection();

app.Run();