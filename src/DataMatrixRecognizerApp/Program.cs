using System.CommandLine;
using DataMatrixRecognizerApp;
using DataMatrixRecognizerApp.Models;

var fileOption = new Option<FileInfo>("--file", "Путь к изображению с DataMatrix кодом")
{
    IsRequired = true
};

var clientIdOption = new Option<string?>("--client-id", () => null, "Идентификатор клиента Aspose Cloud. Если не указан, используется переменная окружения ASPOSE_BARCODE_CLIENT_ID.");
var clientSecretOption = new Option<string?>("--client-secret", () => null, "Секрет клиента Aspose Cloud. Если не указан, используется переменная окружения ASPOSE_BARCODE_CLIENT_SECRET.");
var presetOption = new Option<string>("--preset", () => "HighQuality", "Предустановка распознавания Aspose (например, NormalQuality, HighQuality, FastQuality).");
var verboseOption = new Option<bool>(name: "--verbose", description: "Показывать подробные сведения о распознавании");

var rootCommand = new RootCommand("Утилита распознавания DataMatrix кодов через Aspose Barcode Cloud");
rootCommand.AddOption(fileOption);
rootCommand.AddOption(clientIdOption);
rootCommand.AddOption(clientSecretOption);
rootCommand.AddOption(presetOption);
rootCommand.AddOption(verboseOption);

rootCommand.SetHandler(async (FileInfo file, string? clientId, string? clientSecret, string preset, bool verbose, CancellationToken cancellationToken) =>
{
    var options = AsposeCloudOptions.FromEnvironment(clientId, clientSecret);
    using var httpClient = AsposeHttpClientFactory.Create();
    var tokenProvider = new AsposeAccessTokenProvider(httpClient, options);
    var service = new DataMatrixRecognitionService(httpClient, tokenProvider, options);

    Console.WriteLine($"Файл: {file.FullName}");
    Console.WriteLine($"Предустановка: {preset}");
    Console.WriteLine("\nРезультаты:\n");

    IReadOnlyList<RecognizedBarcode> results;
    try
    {
        results = await service.RecognizeAsync(file, preset, cancellationToken);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("Ошибка при распознавании: " + ex.Message);
        return;
    }

    if (results.Count == 0)
    {
        Console.WriteLine("DataMatrix коды не найдены.");
        return;
    }

    for (var index = 0; index < results.Count; index++)
    {
        var barcode = results[index];
        Console.WriteLine($"#{index + 1}: {barcode.Value ?? "<пустое значение>"}");
        Console.WriteLine($"   Тип: {barcode.Type ?? "Неизвестно"}");

        if (barcode.Confidence.HasValue)
        {
            Console.WriteLine($"   Достоверность: {barcode.Confidence:P2}");
        }

        if (barcode.Region is { Points: { Count: > 0 } points })
        {
            Console.WriteLine("   Контур:");
            foreach (var point in points)
            {
                Console.WriteLine($"      ({point.X}, {point.Y})");
            }
        }

        if (verbose && barcode.Metadata?.Count > 0)
        {
            Console.WriteLine("   Дополнительно:");
            foreach (var kv in barcode.Metadata)
            {
                Console.WriteLine($"      {kv.Key}: {kv.Value}");
            }
        }

        Console.WriteLine();
    }
}, fileOption, clientIdOption, clientSecretOption, presetOption, verboseOption);

return await rootCommand.InvokeAsync(args);
