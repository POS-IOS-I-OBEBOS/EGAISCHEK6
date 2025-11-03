using System.Globalization;

namespace DataMatrixRecognizerApp;

public sealed class AsposeCloudOptions
{
    public AsposeCloudOptions(string clientId, string clientSecret, Uri baseApiUrl, Uri tokenUrl)
    {
        ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        ClientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
        BaseApiUrl = baseApiUrl ?? throw new ArgumentNullException(nameof(baseApiUrl));
        TokenUrl = tokenUrl ?? throw new ArgumentNullException(nameof(tokenUrl));
    }

    public string ClientId { get; }

    public string ClientSecret { get; }

    public Uri BaseApiUrl { get; }

    public Uri TokenUrl { get; }

    public static AsposeCloudOptions FromEnvironment(string? clientId, string? clientSecret)
    {
        var resolvedClientId = !string.IsNullOrWhiteSpace(clientId)
            ? clientId
            : Environment.GetEnvironmentVariable("ASPOSE_BARCODE_CLIENT_ID");

        var resolvedClientSecret = !string.IsNullOrWhiteSpace(clientSecret)
            ? clientSecret
            : Environment.GetEnvironmentVariable("ASPOSE_BARCODE_CLIENT_SECRET");

        if (string.IsNullOrWhiteSpace(resolvedClientId) || string.IsNullOrWhiteSpace(resolvedClientSecret))
        {
            throw new InvalidOperationException("Укажите client id/secret параметрами или через переменные окружения ASPOSE_BARCODE_CLIENT_ID и ASPOSE_BARCODE_CLIENT_SECRET.");
        }

        return new AsposeCloudOptions(
            resolvedClientId,
            resolvedClientSecret,
            new Uri("https://api.aspose.cloud/v3.0/"),
            new Uri("https://api.aspose.cloud/connect/token"));
    }
}
