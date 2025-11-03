using System.Text.Json.Serialization;

using System.Collections.Generic;
using System.Text.Json;

namespace DataMatrixRecognizerApp.Models;

public sealed record RecognizedBarcode(
    string? Value,
    string? Type,
    double? Confidence,
    BarcodeRegion? Region,
    IReadOnlyDictionary<string, string>? Metadata);

public sealed record BarcodeRegion(IReadOnlyList<BarcodePoint> Points);

public sealed record BarcodePoint(double X, double Y);

internal sealed class RecognitionResponse
{
    [JsonPropertyName("Barcodes")]
    public List<BarcodeResponse> Barcodes { get; init; } = new();
}

internal sealed class BarcodeResponse
{
    [JsonPropertyName("Type")]
    public string? Type { get; init; }

    [JsonPropertyName("BarcodeValue")]
    public string? Value { get; init; }

    [JsonPropertyName("Confidence")]
    public double? Confidence { get; init; }

    [JsonPropertyName("Region")]
    public RegionResponse? Region { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; init; }
}

internal sealed class RegionResponse
{
    [JsonPropertyName("Points")]
    public List<PointResponse> Points { get; init; } = new();
}

internal sealed class PointResponse
{
    [JsonPropertyName("X")]
    public double X { get; init; }

    [JsonPropertyName("Y")]
    public double Y { get; init; }
}
