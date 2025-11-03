@echo off
setlocal ENABLEEXTENSIONS

rem Launch prompt window if not already in prompt mode
if /I not "%~1"=="--prompt" (
    start "DataMatrix Recognizer" cmd /k "\"%~f0\" --prompt"
    goto :eof
)

setlocal ENABLEDELAYEDEXPANSION

set "SCRIPT_DIR=%~dp0"
set "APP_DIR=%SCRIPT_DIR%src\DataMatrixRecognizerApp"
set "PUBLISH_DIR=%APP_DIR%\bin\Release\net8.0-windows\win-x64\publish"
set "EXE_PATH=%PUBLISH_DIR%\DataMatrixRecognizerApp.exe"

set "USE_DOTNET_RUN="
if not exist "%EXE_PATH%" (
    set "USE_DOTNET_RUN=1"
)

title Aspose DataMatrix Recognizer
cls
echo ===============================================
echo        Aspose DataMatrix Cloud Recognizer
echo ===============================================
echo.

set "clientId="
set /p clientId=Введите Aspose Client ID ^(Enter, если задан в окружении^): 

set "clientSecret="
set /p clientSecret=Введите Aspose Client Secret ^(Enter, если задан в окружении^): 

:askFile
echo.
set "imagePath="
set /p imagePath=Введите полный путь к изображению DataMatrix: 
if not defined imagePath goto :askFile
if not exist "%imagePath%" (
    echo [!] Файл "^%imagePath^%" не найден. Попробуйте снова.
    goto :askFile
)

echo.
set "preset=HighQuality"
set /p preset=Введите пресет качества ^(HighQuality по умолчанию^): 
if not defined preset set "preset=HighQuality"

echo.
echo Запуск распознавания...
echo.

if defined clientId set "ASPOSE_BARCODE_CLIENT_ID=%clientId%"
if defined clientSecret set "ASPOSE_BARCODE_CLIENT_SECRET=%clientSecret%"

if defined USE_DOTNET_RUN (
    if not exist "%APP_DIR%\DataMatrixRecognizerApp.csproj" (
        echo [!] Не удалось найти проект .NET по пути:
        echo     %APP_DIR%
        echo Проверьте расположение репозитория.
        goto :done
    )
    pushd "%APP_DIR%"
    call dotnet run -- --file "%imagePath%" --preset "%preset%"
    set "EXIT_CODE=%ERRORLEVEL%"
    popd
) else (
    call "%EXE_PATH%" --file "%imagePath%" --preset "%preset%"
    set "EXIT_CODE=%ERRORLEVEL%"
)

echo.
if "%EXIT_CODE%"=="0" (
    echo Распознавание завершено успешно.
) else (
    echo Команда завершилась с кодом ошибки %EXIT_CODE%.
)

echo.
echo Готово. Нажмите любую клавишу для выхода.
pause >nul
endlocal
endlocal
exit /b
