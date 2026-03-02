using System.Net;
using System.Text.Json;
using azure_friday.core.services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace azure_friday.tests;

/// <summary>
/// Fake IAzureFridayDB that returns canned episode data for testing.
/// </summary>
public class FakeAzureFridayDB : IAzureFridayDB
{
    public List<Episode> Episodes { get; set; } = new()
    {
        new Episode
        {
            title = "Test Episode 1",
            url = "https://learn.microsoft.com/shows/azure-friday/test-1",
            description = "First test episode",
            descriptionAsHtml = "<p>First test episode</p>",
            entryId = "entry-1",
            uploadDate = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            thumbnailUrl = "https://example.com/thumb1.jpg",
            youTubeUrl = "https://youtube.com/watch?v=test1"
        },
        new Episode
        {
            title = "Test Episode 2",
            url = "https://learn.microsoft.com/shows/azure-friday/test-2",
            description = "Second test episode",
            descriptionAsHtml = "<p>Second test episode</p>",
            entryId = "entry-2",
            uploadDate = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),
            thumbnailUrl = "https://example.com/thumb2.jpg",
            youTubeUrl = "https://youtube.com/watch?v=test2"
        }
    };

    public bool PurgeCalled { get; private set; }

    public Task<List<Episode>> GetVideos() => Task.FromResult(Episodes);
    public Task<List<Episode>> PopulateVideosCache() => Task.FromResult(Episodes);
    public bool PurgeCache() { PurgeCalled = true; return true; }
}

/// <summary>
/// WebApplicationFactory that replaces IAzureFridayDB with our fake.
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    public FakeAzureFridayDB FakeDb { get; } = new();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove real IAzureFridayDB registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IAzureFridayDB));
            if (descriptor != null) services.Remove(descriptor);

            // Add fake
            services.AddSingleton<IAzureFridayDB>(FakeDb);
        });
    }
}

#region Integration Tests - Homepage & Pages

public class HomepageTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public HomepageTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Homepage_ReturnsSuccessStatusCode()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Homepage_ReturnsHtmlContent()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Privacy_ReturnsSuccessStatusCode()
    {
        var response = await _client.GetAsync("/Privacy");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

#endregion

#region Integration Tests - Episode ID Redirect

public class EpisodeRedirectTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EpisodeRedirectTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Theory]
    [InlineData(12, "https://aka.ms/azfr/012")]
    [InlineData(1, "https://aka.ms/azfr/001")]
    [InlineData(999, "https://aka.ms/azfr/999")]
    public async Task Homepage_WithId_RedirectsToAkams(int id, string expectedUrl)
    {
        var response = await _client.GetAsync($"/?id={id}");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal(expectedUrl, response.Headers.Location?.ToString());
    }
}

#endregion

#region Integration Tests - Videos API Endpoint

public class VideosApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public VideosApiTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task LoadVideos_ReturnsJson()
    {
        var response = await _client.GetAsync("/?handler=LoadVideos");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task LoadVideos_ReturnsExpectedEpisodes()
    {
        var response = await _client.GetAsync("/?handler=LoadVideos");
        var content = await response.Content.ReadAsStringAsync();
        var episodes = JsonSerializer.Deserialize<List<JsonElement>>(content);

        Assert.NotNull(episodes);
        Assert.Equal(2, episodes.Count);
        Assert.Equal("Test Episode 1", episodes[0].GetProperty("title").GetString());
    }

    [Fact]
    public async Task LoadVideos_HasCacheControlHeader()
    {
        var response = await _client.GetAsync("/?handler=LoadVideos");
        var cacheControl = response.Headers.CacheControl;

        Assert.True(cacheControl?.Public);
        Assert.Equal(TimeSpan.FromHours(4), cacheControl?.MaxAge);
    }
}

#endregion

#region Integration Tests - Security Headers

public class SecurityHeaderTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SecurityHeaderTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Response_HasXContentTypeOptions()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").First());
    }

    [Fact]
    public async Task Response_HasXFrameOptions()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").First());
    }

    [Fact]
    public async Task Response_HasReferrerPolicy()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal("strict-origin-when-cross-origin",
            response.Headers.GetValues("Referrer-Policy").First());
    }

    [Fact]
    public async Task Response_HasPermissionsPolicy()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal("camera=(), microphone=(), geolocation=()",
            response.Headers.GetValues("Permissions-Policy").First());
    }

    [Fact]
    public async Task Response_HasContentSecurityPolicy()
    {
        var response = await _client.GetAsync("/");
        // CSP may appear on response headers or content headers depending on the server
        IEnumerable<string> values;
        if (!response.Headers.TryGetValues("Content-Security-Policy", out values))
        {
            Assert.True(response.Content.Headers.TryGetValues("Content-Security-Policy", out values),
                "Content-Security-Policy header not found on response or content headers");
        }
        var csp = values.First();
        Assert.Contains("default-src 'self'", csp);
        Assert.Contains("script-src 'self'", csp);
        Assert.Contains("frame-ancestors 'none'", csp);
        Assert.Contains("object-src 'none'", csp);
        Assert.Contains("base-uri 'self'", csp);
        Assert.Contains("form-action 'self'", csp);
        Assert.Contains("upgrade-insecure-requests", csp);
        Assert.DoesNotContain("unsafe-inline", csp);
        Assert.DoesNotContain("unsafe-eval", csp);
        Assert.DoesNotContain("cdn.tailwindcss.com", csp);
    }
}

#endregion

#region Integration Tests - RSS Redirects

public class RssRedirectTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RssRedirectTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Rss_RedirectsToBlobStorage()
    {
        var response = await _client.GetAsync("/rss");
        Assert.True(response.StatusCode == HttpStatusCode.Moved ||
                    response.StatusCode == HttpStatusCode.Redirect ||
                    (int)response.StatusCode == 301 || (int)response.StatusCode == 302);
        Assert.Contains("azurefriday.rss", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task RssAudio_RedirectsToBlobStorage()
    {
        var response = await _client.GetAsync("/rssaudio");
        Assert.True(response.StatusCode == HttpStatusCode.Moved ||
                    response.StatusCode == HttpStatusCode.Redirect ||
                    (int)response.StatusCode == 301 || (int)response.StatusCode == 302);
        Assert.Contains("azurefridayaudio.rss", response.Headers.Location?.ToString());
    }
}

#endregion

#region Integration Tests - Domain Redirect

public class DomainRedirectTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DomainRedirectTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task AzureWebsitesDomain_RedirectsToCanonical()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Host = "its-azure-friday.azurewebsites.net";

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
        Assert.StartsWith("http://azurefriday.com", response.Headers.Location?.ToString() ?? "");
    }
}

#endregion

#region Unit Tests - Episode Model

public class EpisodeModelTests
{
    [Fact]
    public void Episode_Properties_CanBeSetAndRead()
    {
        var episode = new Episode
        {
            title = "Test Title",
            url = "https://example.com/episode",
            description = "A description",
            descriptionAsHtml = "<p>A description</p>",
            entryId = "abc-123",
            uploadDate = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            thumbnailUrl = "https://example.com/thumb.jpg",
            youTubeUrl = "https://youtube.com/watch?v=abc"
        };

        Assert.Equal("Test Title", episode.title);
        Assert.Equal("abc-123", episode.entryId);
        Assert.Equal(2025, episode.uploadDate.Year);
    }

    [Fact]
    public void Episode_SerializesToJson()
    {
        var episode = new Episode
        {
            title = "JSON Test",
            url = "https://example.com",
            entryId = "json-1",
            uploadDate = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(episode);
        Assert.Contains("\"title\":\"JSON Test\"", json);
        Assert.Contains("\"entryId\":\"json-1\"", json);
    }
}

#endregion

#region Unit Tests - AzureFridayDB

public class AzureFridayDBTests
{
    [Fact]
    public void FakeDb_PurgeCache_SetsFlag()
    {
        var db = new FakeAzureFridayDB();
        Assert.False(db.PurgeCalled);

        db.PurgeCache();
        Assert.True(db.PurgeCalled);
    }

    [Fact]
    public async Task FakeDb_GetVideos_ReturnsEpisodes()
    {
        var db = new FakeAzureFridayDB();
        var videos = await db.GetVideos();

        Assert.Equal(2, videos.Count);
        Assert.Equal("Test Episode 1", videos[0].title);
    }

    [Fact]
    public async Task FakeDb_EmptyList_ReturnsEmpty()
    {
        var db = new FakeAzureFridayDB { Episodes = new List<Episode>() };
        var videos = await db.GetVideos();

        Assert.Empty(videos);
    }
}

#endregion

#region Integration Tests - 404 Handling

public class NotFoundTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public NotFoundTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task NonExistentPage_Returns404StatusCode()
    {
        var response = await _client.GetAsync("/this-page-does-not-exist");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

#endregion
