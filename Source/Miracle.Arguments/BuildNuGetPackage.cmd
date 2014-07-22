@echo off
..\..\NuGet\NuGet.exe pack Miracle.Arguments.csproj -prop Configuration=release
echo "run ..\..\NuGet\NuGet.exe push Miracle.Arguments.x.x.x.x.nupkg to publish"
pause