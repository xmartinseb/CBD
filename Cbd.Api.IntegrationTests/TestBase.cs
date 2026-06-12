using Cbd.Api.Configuration;
using Cbd.Api.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cbd.Api.IntegrationTests;

public abstract class TestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient client;

    protected TestBase(RepoType ordersRepositoryType)
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IOrdersRepository>();
                    switch (ordersRepositoryType)
                    {
                        case RepoType.InMemory:
                            services.AddSingleton<IOrdersRepository, InMemoryOrdersRepository>();
                            break;
                        case RepoType.Sql:
                            services.AddSingleton<IOrdersRepository, SqlOrdersRepository>();
                            break;
                        default:
                            throw new ArgumentException("Invalid repository type");
                    }
                });

            });
        client = factory.CreateClient();
    }
}
