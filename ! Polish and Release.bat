@echo off

:start
set /p version=[93;101mRelease Version?[0m 
if "%version%" EQU "" goto start

rem Modify file data using javascript
node mkr %version%

rem Zip the files
mkdir tmp
cd tmp
mkdir BetterEditor
mkdir BetterEditor\x86
mkdir BetterEditor\x64

(
copy "..\BetterEditor\bin\Debug\BetterEditor.dll" BetterEditor
copy "..\BetterEditor\bin\Debug\EntityFramework.dll" BetterEditor
copy "..\BetterEditor\bin\Debug\EntityFramework.SqlServer.dll" BetterEditor
copy "..\BetterEditor\bin\Debug\x86\SQLite.Interop.dll" BetterEditor\x86
copy "..\BetterEditor\bin\Debug\x64\SQLite.Interop.dll" BetterEditor\x64
copy "..\BetterEditor\bin\Debug\System.Data.SQLite.dll" BetterEditor
copy "..\BetterEditor\bin\Debug\System.Data.SQLite.EF6.dll" BetterEditor
copy "..\BetterEditor\bin\Debug\System.Data.SQLite.Linq.dll" BetterEditor
copy "..\Info.json" BetterEditor
)>nul

tar -a -c -f ..\BetterEditor-%version%.zip BetterEditor
cd ..
rmdir /s /q tmp

exit /b 1