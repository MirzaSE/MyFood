using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

public class FoodsApiE2ETests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public FoodsApiE2ETests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:7124")
        }); // uses in-memory server
    }

    [Fact]
    public async Task GetFoods_ShouldReturnSuccess()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/v1/foods");

        // Assert
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Baklava", body); // Or validate JSON
    }

    [Fact]
    public async Task CreateFood_ShouldReturnCreated()
    {
        var content = new StringContent(
            "{\"name\":\"Burger\",\"calories\":500}",
            System.Text.Encoding.UTF8,
            "application/json"
        );

        var response = await _client.PostAsync("/api/v1/foods", content);

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }
}
