using Cbd.Api.Configuration;
using Cbd.Api.Models;
using FluentAssertions;
using System.Net.Http.Json;

namespace Cbd.Api.IntegrationTests;

public sealed class ApiTests() : TestBase(RepoType.InMemory)
{
    [Fact]
    public async Task CreateNewOrderTest()
    {
        Order[] newOrder = [new Order("Product123", Quantity: 5)];
        var response = await AcceptNewOrderAsync(newOrder);
        
        response.IsSuccessStatusCode.Should().BeTrue();
        var loadedOrders = await GetAllOrders();
        loadedOrders.Length.Should().Be(1);
        loadedOrders[0].Should().Be(newOrder[0]);
    }

    /// <summary>
    /// Otestuje schopnost přijmout mnoho objednávek najednou.
    /// </summary>
    [Fact]
    public async Task CreateManyOrdersTest()
    {
        const int ORDERS = 200;
        const string PRODUCT_ID = "Product123";
        foreach (var i in Enumerable.Range(0, ORDERS))
        {
            Order[] newOrder = [new Order(PRODUCT_ID, Quantity: 1)];
            var response = await AcceptNewOrderAsync(newOrder);
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        // TODO: sem bych doplnil delay, načetl agregované objednávky a zkontroloval součet, ale to by vyžadovalo lepší ukládání agregací než pouze WriteLine do konzole.
        var loadedOrders = await GetAllOrders();
        loadedOrders.Length.Should().Be(ORDERS);
        foreach (var order in loadedOrders)
        {
            order.ProductId.Should().Be(PRODUCT_ID);
            order.Quantity.Should().Be(1);
        }
    }

    async Task<HttpResponseMessage> AcceptNewOrderAsync(IEnumerable<Order> newOrders)
    {
        var response = await client.PostAsync($"/Order/Accept", JsonContent.Create(newOrders));
        response.Should().NotBeNull();
        response.EnsureSuccessStatusCode();
        return response;
    }

    async Task<Order[]> GetAllOrders()
    {
        var response = await client.GetAsync($"/Order/GetAll");
        response.Should().NotBeNull();
        response.EnsureSuccessStatusCode();
        var orders = await response.Content.ReadFromJsonAsync<Order[]>();
        orders.Should().NotBeNull();
        return orders!;
    }
}