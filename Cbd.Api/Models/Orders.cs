using System.ComponentModel.DataAnnotations;

namespace Cbd.Api.Models;

/// <summary>Objednávka produktu.</summary>
/// <param name="ProductId">Identifikátor produktu.</param>
/// <param name="Quantity">Požadované množství (musí být kladné číslo).</param>
public sealed record Order(
    [property: Required] string ProductId,
    [property: Range(1, 100000)] int Quantity);

/// <summary>
/// Záznam o vytvoření objednávky, který se posílá do kanálu pro agregaci objednávek. 
/// </summary>
/// <param name="CreatedUtc">
/// Čas vzniku se používá k oddělení agregací. Kdyby do systému proudilo ticíce orderů za sekundu, agregace by se mohla zahltit a nevracela by
/// žádný výsledek. Díky časové značce se agregace vždy zpracovávají jen pro omezenou množinu objednávek, což zajišťuje, že se agregace nezahltí a vrací výsledky v přiměřeném čase.
/// </param>
public readonly record struct OrderCreated(Order Order, DateTime CreatedUtc);

public sealed record AggregatedOrder(string ProductId, int Quantity);

/// <summary>
/// Obsahuje agregované objednávky pro různé produkty a čas, kdy byla agregace provedena.
/// </summary>
public sealed record AggregatedOrdersCollection(IReadOnlyList<AggregatedOrder> AggregatedOrders, DateTime AggregateTimeUtc);