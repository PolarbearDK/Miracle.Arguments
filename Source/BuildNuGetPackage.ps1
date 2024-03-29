param (
	[switch]$Pack = $true,
	[switch]$Push
)

Set-StrictMode -Version latest
$ErrorActionPreference = "Stop"

if($pack) {
  & dotnet build --configuration Release
	if(!$?){throw "NuGet returned exit code $LASTEXITCODE"}
	dotnet test .\Miracle.Arguments.Tests\ --configuration Release
	if(!$?){throw "NuGet returned exit code $LASTEXITCODE"}
}

if($push) {
	$filename = Get-ChildItem ".\Miracle.Arguments\bin\Release\Miracle.Arguments.*" | Sort-Object LastWriteTime -Descending | Select -First 1
	echo "Pushing $filename"
  & .NuGet\NuGet.exe push "$filename" -Source https://api.nuget.org/v3/index.json
}
