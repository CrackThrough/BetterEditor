:start
set /p version=[93;101mRelease Version? [0m
if "%version%" EQU "" goto start

rem Modify file data using javascript
node mkr %version%

rem Zip the files
mkdir tmp
cd tmp
mkdir BetterEditor
cp "bin\Debug\BetterEditor.dll" BetterEditor
cp "Info.json" BetterEditor
tar -a -c -f BetterEditor-%version%.zip
move BetterEditor-%version%.zip ..
cd ..
rmdir tmp

exit /b 1