param([string]$OutputPath = (Join-Path $PSScriptRoot 'bin\publish'))

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'The .NET 10 SDK was not found.' }

# Framework-dependent single-file Alter.exe; assets\ and community\ are copied next to it by Alter.csproj
& dotnet publish (Join-Path $PSScriptRoot 'Alter.csproj') -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:DebugType=none -o $OutputPath
if ($LASTEXITCODE -ne 0) { throw "Build failed with exit code $LASTEXITCODE" }
Write-Output "Built $(Join-Path $OutputPath 'Alter.exe')"
