echo off

rem clear any previous build traces
del RumbleHud.zip
del thunderstoreBuild\*
del thunderstoreBuild\Mods\*
del thunderstoreBuild\UserData\*
rmdir thunderstoreBuild\Mods
rmdir thunderstoreBuild\UserData
rmdir thunderstoreBuild

rem make build folder
mkdir thunderstoreBuild
mkdir thunderstoreBuild\Mods
mkdir thunderstoreBuild\UserData
copy CHANGELOG.md thunderstoreBuild
copy icon.png thunderstoreBuild
copy ..\README.md thunderstoreBuild
copy manifest.json thunderstoreBuild

copy ..\AssetBundle\rumblehud thunderstoreBuild\UserData

echo Add DLL File to Mods...
pause

cd thunderstoreBuild
7z a -tzip RumbleHud.zip .

move RumbleHud.zip ..
cd ..