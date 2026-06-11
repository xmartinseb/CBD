using Cbd.Api.Configuration;
using Cbd.Api.Models;
using FluentAssertions;
using System.Net.Http.Json;

namespace Cbd.Api.IntegrationTests;

public sealed class ApiTests() : TestBase(OrdersRepositoryType.InMemory)
{
    [Fact]
    public async Task CreateNewOrderTest()
    {
        Order[] newOrders = [new Order("Product123", Quantity: 5)];
        var response = await AcceptNewOrderAsync(newOrders);
        
        response.IsSuccessStatusCode.Should().BeTrue();
        var loadedOrders = await GetAllOrders();
        loadedOrders.Length.Should().Be(1);
        loadedOrders[0].Should().Be(newOrders[0]);
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