using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Linq;
using System.Text.Json;
using DataMatrixRecognizerApp.Models;

namespace DataMatrixRecognizerApp;

public sealed class DataMatrixRecognitionService
{
    private readonly HttpClient _httpClient;
    private readonly AsposeAccessTokenProvider _tokenProvider;
    private readonly AsposeCloudOptions _options;

    public DataMatrixRecognitionService(HttpClient httpClient, AsposeAccessTokenProvider tokenProvider, AsposeCloudOptions options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<IReadOnlyList<RecognizedBarcode>> RecognizeAsync(FileInfo imageFile, string preset, CancellationToken cancellationToken)
    {
        if (!imageFile.Exists)
        {
            throw new FileNotFoundException("Файл не найден", imageFile.FullName);
        }

        var accessToken = await _tokenProvider.GetTokenAsync(cancellationToken).ConfigureAwait(false);
        var recognizeUri = new Uri(_options.BaseApiUrl, $"barcode/recognize?type=DataMatrix&preset={Uri.EscapeDataString(preset)}");

        await using var fileStream = imageFile.OpenRead();
        using var request = new HttpRequestMessage(HttpMethod.Post, recognizeUri)
        {
            Content = new StreamContent(fileStream)
        };

        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var payload = await response.Content.ReadFromJsonAsync<RecognitionResponse>(serializerOptions, cancellationToken).ConfigureAwait(false);
        if (payload is null)
        {
            return Array.Empty<RecognizedBarcode>();
        }

        return payload.Barcodes
            .Select(ToModel)
            .ToArray();
    }

    private static RecognizedBarcode ToModel(BarcodeResponse barcode)
    {
        BarcodeRegion? region = null;
        if (barcode.Region is { Points: { Count: > 0 } rawPoints })
        {
            var points = rawPoints
                .Select(point => new BarcodePoint(point.X, point.Y))
                .ToArray();
            region = new BarcodeRegion(points);
        }

        IReadOnlyDictionary<string, string>? metadata = null;
        if (barcode.AdditionalProperties is { Count: > 0 })
        {
            metadata = barcode.AdditionalProperties
                .Where(pair => pair.Value.ValueKind != JsonValueKind.Object && pair.Value.ValueKind != JsonValueKind.Array)
                .ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.ToString());
        }

        return new RecognizedBarcode(barcode.Value, barcode.Type, barcode.Confidence, region, metadata);
    }
}
