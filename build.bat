@echo off
setlocal

REM Determine the project directory relative to this script
set "SCRIPT_DIR=%~dp0"
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
