$csproj = "$PSScriptRoot\..\src\NuExt.Minimal.Mvvm.MahApps.Metro.csproj"
$Configuration = "Release"
$outDir = $PSScriptRoot

dotnet clean $csproj -c $Configuration
dotnet pack $csproj -c $Configuration -o $outDir