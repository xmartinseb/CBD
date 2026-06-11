namespace Cbd.Api.Configuration;

public sealed class OrdersRepositoryConfig
{
    public OrdersRepositoryType RepositoryType { get; set; }
}

public enum OrdersRepositoryType
{
    InMemory, Sql
}