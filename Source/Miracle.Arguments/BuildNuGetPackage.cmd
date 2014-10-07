@echo off

MSBUILD.exe Miracle.Arguments.csproj /t:BeforeBuild
MSBUILD.exe Miracle.Arguments.csproj /t:Build /p:Configuration="Release 3.5"
MSBUILD.exe Miracle.Arguments.csproj /t:Build /p:Configuration="Release 4.0"
MSBUILD.exe Miracle.Arguments.csproj /t:Build /p:Configuration="Release 4.5"
MSBUILD.exe Miracle.Arguments.csproj /t:AfterBuild;Package /p:Configuration="Release 4.5"

echo "run ..\..\NuGet\NuGet.exe push NuGet\Miracle.Arguments... to publish"
pause