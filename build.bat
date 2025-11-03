@echo off
setlocal

REM Determine the project directory relative to this script
set "SCRIPT_DIR=%~dp0"

REM Ensure every run writes a transcript to a timestamped log file
if not defined LOGGING_ACTIVE (
    for /f "delims=" %%I in ('powershell -NoProfile -Command "(Get-Date).ToString(^"yyyyMMdd-HHmmss^")"') do set "TIMESTAMP=%%I"
    if not defined TIMESTAMP (
        set "TIMESTAMP=manual-log"
    )
    set "LOG_DIR=%SCRIPT_DIR%logs"
    if not exist "%LOG_DIR%" mkdir "%LOG_DIR%"
    set "LOG_FILE=%LOG_DIR%\build-%TIMESTAMP%.log"
    set "LOGGING_ACTIVE=1"
    echo [INFO] Full build output will be logged to: %LOG_FILE%
    powershell -NoProfile -Command ^
        "$logPath = '%LOG_FILE%';" ^
        "$dir = Split-Path -Path $logPath;" ^
        "if (-not (Test-Path -LiteralPath $dir)) { New-Item -ItemType Directory -Path $dir | Out-Null };" ^
        "Start-Transcript -Path $logPath -Append | Out-Null;" ^
        "& cmd /c \""%~f0" %*\";" ^
        "$exitCode = $LASTEXITCODE;" ^
        "Stop-Transcript | Out-Null;" ^
        "exit $exitCode"
    exit /b %errorlevel%
)

set "PROJECT_DIR=%SCRIPT_DIR%src\DataMatrixRecognizerApp"
set "PROJECT_FILE=%PROJECT_DIR%\DataMatrixRecognizerApp.csproj"

if not exist "%PROJECT_FILE%" (
    echo [ERROR] Could not find project file: %PROJECT_FILE%
    exit /b 1
)

echo [INFO] Restoring NuGet packages...
dotnet restore "%PROJECT_FILE%"
if errorlevel 1 goto :error

echo [INFO] Building project (Release)...
dotnet build "%PROJECT_FILE%" --configuration Release
if errorlevel 1 goto :error

echo [INFO] Publishing win-x64 artifacts...
dotnet publish "%PROJECT_FILE%" --configuration Release --runtime win-x64 --self-contained false
if errorlevel 1 goto :error

echo [SUCCESS] Build and publish completed successfully.
goto :eof

:error
echo [FAIL] Build failed with exit code %errorlevel%.
exit /b %errorlevel%

:eof
endlocal
