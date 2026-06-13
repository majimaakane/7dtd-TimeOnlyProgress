@echo off
echo ==========================================
echo  TimeOnlyProgress ビルドスクリプト
echo ==========================================
echo.

REM ★ 7DTDサーバーのパスを変更してください（csprojのGameDirと同じ）
set GAME_DIR=C:\Program Files (x86)\Steam\steamapps\common\7 Days to Die Dedicated Server

REM ビルド実行
echo ビルド中...
dotnet build src\TimeOnlyProgress.csproj -c Release -p:GameDir="%GAME_DIR%"

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ★ ビルドに失敗しました。
    echo   GameDirのパスが正しいか確認してください。
    pause
    exit /b 1
)

REM DLLをモッドフォルダにコピー
echo.
echo DLLをモッドフォルダにコピー中...
copy /Y "src\bin\Release\TimeOnlyProgress.dll" "TimeOnlyProgress\TimeOnlyProgress.dll"

echo.
echo ==========================================
echo  ビルド完了！
echo  TimeOnlyProgress フォルダを
echo  サーバーの Mods フォルダにコピーしてください。
echo.
echo  設定変更: TimeOnlyProgress\config.xml
echo ==========================================
pause
