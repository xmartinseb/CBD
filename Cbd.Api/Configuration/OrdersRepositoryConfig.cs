namespace Cbd.Api.Configuration;

/// <summary>
/// Reprezentuje část appsettings.json, která určuje, jaký typ repository se má použít pro ukládání objednávek.
/// </summary>
public sealed class OrdersRepositoryConfig
{
    public RepoType RepositoryType { get; set; }
}

public enum RepoType
{
    InMemory, Sql
}