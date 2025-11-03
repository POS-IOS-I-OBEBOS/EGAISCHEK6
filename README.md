# DataMatrixRecognizerApp

Пример консольного Windows-приложения на .NET 8.0, выполняющего распознавание DataMatrix кодов при помощи облачного сервиса [Aspose Barcode Cloud](https://products.aspose.cloud/barcode/).

## Возможности

- Получение OAuth2 токена Aspose Cloud по `client_id` и `client_secret`.
- Отправка изображений на распознавание в Aspose Barcode Cloud c указанием предустановки качества (HighQuality, NormalQuality и др.).
- Вывод найденных DataMatrix кодов, вероятности распознавания, координат контура и дополнительных метаданных.

## Подготовка

1. Установите .NET SDK 8.0 на Windows.
2. Получите учетные данные (Client Id и Client Secret) в [личном кабинете Aspose Cloud](https://dashboard.aspose.cloud/).
3. Склонируйте репозиторий и восстановите зависимости:

   ```powershell
   git clone <repo-url>
   cd EGAISCHEK6/src/DataMatrixRecognizerApp
   dotnet restore
   ```

## Запуск

```powershell
dotnet run -- \
    --file "C:\\path\\to\\datamatrix.png" \
    --client-id "<ваш client id>" \
    --client-secret "<ваш client secret>" \
    --preset HighQuality
```

Параметры `--client-id` и `--client-secret` можно опустить, если задать переменные окружения:

```powershell
$Env:ASPOSE_BARCODE_CLIENT_ID = "<ваш client id>"
$Env:ASPOSE_BARCODE_CLIENT_SECRET = "<ваш client secret>"
```

Для получения более детальной информации о распознавании добавьте флаг `--verbose`.

## Структура проекта

```
src/DataMatrixRecognizerApp/
├── AsposeAccessTokenProvider.cs   // Запрос и кеширование OAuth токена Aspose
├── AsposeCloudOptions.cs          // Загрузка настроек из окружения
├── AsposeHttpClientFactory.cs     // Настройки HTTP клиента
├── DataMatrixRecognitionService.cs// Вызов API распознавания Aspose
├── Models/
│   └── RecognizedBarcode.cs       // Модели данных и DTO
├── Program.cs                     // Входная точка и CLI
└── DataMatrixRecognizerApp.csproj // Конфигурация проекта (.NET 8, NuGet)
```

## Сборка

Для публикации исполняемого файла под Windows можно воспользоваться подготовленным скриптом `build.bat` в корне репозитория:

```powershell
.\build.bat
```

Скрипт последовательно выполняет команды `dotnet restore`, `dotnet build` и `dotnet publish` (Release, win-x64). Готовый exe будет расположен в папке `bin/Release/net8.0-windows/win-x64/publish`.

Каждый запуск автоматически сохраняет полный вывод в файл `logs/build-YYYYMMDD-HHMMSS.log`, что облегчает диагностику ошибок сборки.
Для публикации исполняемого файла под Windows используйте `dotnet publish`:

```powershell
cd src/DataMatrixRecognizerApp
dotnet publish -c Release -r win-x64 --self-contained false
```

Готовый exe будет расположен в папке `bin/Release/net8.0-windows/win-x64/publish`.
