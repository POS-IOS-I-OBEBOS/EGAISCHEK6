using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataMatrixRecognizerApp;

public sealed class AsposeAccessTokenProvider
{
    private readonly HttpClient _httpClient;
    private readonly AsposeCloudOptions _options;
    private readonly SemaphoreSlim _mutex = new(1, 1);
    private string? _accessToken;
    private DateTimeOffset _expiresAt;

    public AsposeAccessTokenProvider(HttpClient httpClient, AsposeCloudOptions options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        await _mutex.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!string.IsNullOrEmpty(_accessToken) && _expiresAt > DateTimeOffset.UtcNow.AddMinutes(-1))
            {
                return _accessToken;
            }

            var token = await RequestTokenAsync(cancellationToken).ConfigureAwait(false);
            _accessToken = token.AccessToken;
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn);
            return _accessToken!;
        }
        finally
        {
            _mutex.Release();
        }
    }

    private async Task<TokenResponse> RequestTokenAsync(CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, _options.TokenUrl);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret
        });

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>(options, cancellationToken).ConfigureAwait(false);
        if (token is null || string.IsNullOrEmpty(token.AccessToken))
        {
            throw new InvalidOperationException("Не удалось получить токен доступа Aspose.");
        }

        return token;
    }

    private sealed class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; init; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }
    }
}
