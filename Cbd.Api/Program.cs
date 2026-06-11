using Cbd.Api;
using Cbd.Api.Configuration;
using Cbd.Api.Data;
using Cbd.Api.HostedServices;
using Cbd.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<OrderAggregationTask>();
builder.Services.AddHostedService<AggregatedOrdersInternalTask>();

RegisterOrdersRepository();
builder.Services.AddSingleton<AggregatedOrdersChannel>();
builder.Services.AddSingleton<CreatedOrdersChannel>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();
app.Run();


void RegisterOrdersRepository()
{
    var repoConfig = builder.Configuration.GetSection("OrdersRepository").Get<OrdersRepositoryConfig>()
        ?? throw new InvalidOperationException("Konfigurace 'OrdersRepository' nebyla nalezena v appsettings.json");

    switch (repoConfig.RepositoryType)
    {
        case OrdersRepositoryType.InMemory:
            // InMemory drží data v paměti aplikace, proto MUSÍ být Singleton
            builder.Services.AddSingleton<IOrdersRepository, InMemoryOrdersRepository>();
            break;

        case OrdersRepositoryType.Sql:
            // SQL repozitáře (např. s Entity Frameworkem) typicky vyžadují Scoped životnost
            builder.Services.AddScoped<IOrdersRepository, SqlOrdersRepository>();
            break;

        default:
            throw new ArgumentOutOfRangeException(nameof(repoConfig.RepositoryType), "Neznámý typ repozitáře");
    }
}