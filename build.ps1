param([string]$OutputPath = (Join-Path $PSScriptRoot 'Alter.exe'))

$compiler = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) { throw 'The .NET Framework 4.x C# compiler was not found.' }

$referenceNames = @('PresentationCore', 'PresentationFramework', 'WindowsBase', 'System.Xaml')
$references = foreach ($name in $referenceNames) {
    $assembly = Get-ChildItem -LiteralPath 'C:\Windows\Microsoft.NET\assembly' -Filter "$name.dll" -Recurse -ErrorAction Stop | Select-Object -First 1 -ExpandProperty FullName
    if (-not $assembly) { throw "Missing WPF assembly: $name" }
    "/reference:$assembly"
}

$sources = @('Alter.cs', 'Design.cs', 'Engine.cs', 'Studio.cs', 'Preferences.cs', 'Tray.cs', 'AssemblyInfo.cs', 'Fling.cs', 'FlingExtract.cs', 'ScriptRuntime.cs', 'CreatorStudio.cs', 'HubExtras.cs', 'LibraryPolish.cs', 'CommunityHub.cs', 'UpdateCheck.cs') | ForEach-Object { Join-Path $PSScriptRoot $_ }
$icon = Join-Path $PSScriptRoot 'assets\Alter.ico'
& $compiler /nologo /target:winexe "/out:$OutputPath" "/win32icon:$icon" $references $sources
if ($LASTEXITCODE -ne 0) { throw "Build failed with exit code $LASTEXITCODE" }
Write-Output "Built $OutputPath"
