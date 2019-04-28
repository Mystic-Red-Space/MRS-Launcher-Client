@echo off

set folderName=MRSLauncherClient

if exist %folderName% (
echo Remove preview version
rd %folderName% /s /q
)
echo Create Output Directory
mkdir %folderName%

echo Copy files
copy CmlLib.dll %folderName%\CmlLib.dll
copy DiscordRPC.dll %folderName%\DiscordRPC.dll
copy DotNetZip.dll %folderName%\DotNetZip.dll
copy %folderName%.exe %folderName%\%folderName%.exe
copy Newtonsoft.Json.dll %folderName%\Newtonsoft.Json.dll
copy license.txt %folderName%\license.txt
copy log4net.dll %folderName%\log4net.dll
copy %folderName%.exe.config %folderName%\%folderName%.exe.config

cd %folderName%
..\..\..\zip.exe MRSLauncher *

echo Done.
pause
