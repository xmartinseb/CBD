using Cbd.Api;
using Cbd.Api.Configuration;
using Cbd.Api.Data;
using Cbd.Api.HostedServices;
using Cbd.Api.Services;
using Microsoft.OpenApi;
using Serilog;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CBD API",
        Version = "v1",
        Description = "API pro příjem a agregaci objednávek"
    });
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Cbd.Api.xml"));
});

// Periodické úlohy: agregace dat a zpracování zagregovaných objednávek
builder.Services.AddHostedService<OrderAggregationTask>();
builder.Services.AddHostedService<AggregatedOrdersInternalTask>();

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("default", httpContext =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            // V produkci  stačí 10, ale kvůli zátěžovému testování nechám pro debug vyšší limit
            PermitLimit = builder.Environment.IsDevelopment() ? 1000 : 10,
            Window = TimeSpan.FromSeconds(10)
        }));
});

RegisterOrdersRepository();

// Aplikace využívá asynchronní kanály pro dodávání informací do hosted services, čímž se oddělí logika zpracování objednávek od logiky jejich přijímání a ukládání.
// Tento mechanismus nijak neošetřuje ztrátu dat (např. při restartu aplikace), ale pro demonstrační účely je zcela dostačující.
builder.Services.AddSingleton<AggregatedOrdersChannel>();
builder.Services.AddSingleton<CreatedOrdersChannel>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRateLimiter();

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Dle konfigurace zaregistruje správný typ repozitáře pro objednávky.
void RegisterOrdersRepository()
{
    var repoConfig = builder.Configuration.GetSection("OrdersRepository").Get<OrdersRepositoryConfig>()
        ?? throw new InvalidOperationException("Konfigurace 'OrdersRepository' nebyla nalezena v appsettings.json");

    switch (repoConfig.RepositoryType)
    {
        case RepoType.InMemory:
            // InMemory drží data v paměti aplikace, proto MUSÍ být Singleton
            builder.Services.AddSingleton<IOrdersRepository, InMemoryOrdersRepository>();
            break;

        case RepoType.Sql:
            // SQL repozitáře (např. s Entity Frameworkem) typicky vyžadují Scoped životnost
            builder.Services.AddScoped<IOrdersRepository, SqlOrdersRepository>();
            break;

        default:
            throw new ArgumentOutOfRangeException(nameof(repoConfig.RepositoryType), "Neznámý typ repozitáře");
    }
}