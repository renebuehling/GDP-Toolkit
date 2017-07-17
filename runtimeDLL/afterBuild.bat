@echo off
REM Call as AfterBuilt-Action: $(ProjectDir)afterBuild.bat ${TargetDir}

SET batDir=%~dp0
SET builtDir=%1
SET targetDir=%batDir%..\..\SmallDudes\Assets\SmallDudes\Resources\

echo batDir=%batDir%
echo builtDir=%builtDir%
echo targetDir=%targetDir%

REM robocopy /XO %builtDir% %targetDir% SmallDudesRuntime.dll

REM robocopy: XO=nur neue, NFL=nur fehler loggen, LOG=ausgabe in log-file (anhängen mit LOG+=...)
REM robocopy /XO /LOG:robocopy.log %src%\cef_binary_3.1750.1738_windows32_client\Release\ %zieldir% *.dll
REM robocopy /XO /LOG+:robocopy.log %src%\cef_binary_3.1750.1738_windows32_client\Release\locales %zieldir%\locales 

REM Robocopy setzt errorlevel 1 wenn dateien kopiert wurden. In dem Fall errorlevel mit exit auf 0 zurück setzen, damit compiler weiter läuft. Robocopy-Ergebnisse: 0=keine Kopien, >1 fehler, siehe https://support.microsoft.com/en-us/kb/954404
REM if errorlevel 1 exit /b 0
