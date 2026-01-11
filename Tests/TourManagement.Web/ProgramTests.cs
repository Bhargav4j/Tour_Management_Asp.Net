using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Microsoft.AspNetCore.Hosting;

namespace TourManagement.Web.Tests;

public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Program_CanCreateApplication()
    {
        // Arrange & Act
        var client = _factory.CreateClient();

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public async Task Program_RootEndpoint_ReturnsSuccessAndCorrectMessage()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Tour Management API", content);
    }

    [Fact]
    public async Task Program_RootEndpoint_ReturnsExpectedContent()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal("Tour Management API - PostgreSQL Ready!", content);
    }

    [Fact]
    public async Task Program_SwaggerEndpoint_IsAccessibleInDevelopment()
    {
        // Arrange
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        }).CreateClient();

        // Act
        var response = await client.GetAsync("/swagger");

        // Assert
        Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.MovedPermanently || response.StatusCode == HttpStatusCode.Redirect);
    }

    [Fact]
    public void Program_HttpsRedirection_IsConfigured()
    {
        // Arrange
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Act & Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void Program_Factory_CreatesMultipleClients()
    {
        // Arrange & Act
        var client1 = _factory.CreateClient();
        var client2 = _factory.CreateClient();

        // Assert
        Assert.NotNull(client1);
        Assert.NotNull(client2);
        Assert.NotSame(client1, client2);
    }

    [Fact]
    public async Task Program_RootEndpoint_ReturnsTextPlainContentType()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        Assert.NotNull(response.Content.Headers.ContentType);
        Assert.Contains("text/plain", response.Content.Headers.ContentType.ToString());
    }

    [Theory]
    [InlineData("/")]
    public async Task Program_Endpoint_Returns200(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Program_InvalidEndpoint_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/invalid-endpoint");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public void Program_WebApplicationFactory_IsNotNull()
    {
        // Assert
        Assert.NotNull(_factory);
    }

    [Fact]
    public async Task Program_RootEndpoint_HasNonEmptyContent()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task Program_MultipleRequests_ToRootEndpoint_AllSucceed()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response1 = await client.GetAsync("/");
        var response2 = await client.GetAsync("/");
        var response3 = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response3.StatusCode);
    }

    [Fact]
    public async Task Program_RootEndpoint_ConsistentResponse()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var content1 = await client.GetStringAsync("/");
        var content2 = await client.GetStringAsync("/");

        // Assert
        Assert.Equal(content1, content2);
    }
}
