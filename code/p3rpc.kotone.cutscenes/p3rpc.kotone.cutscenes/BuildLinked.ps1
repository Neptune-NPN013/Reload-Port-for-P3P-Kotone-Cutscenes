# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/p3rpc.kotone.cutscenes/*" -Force -Recurse
dotnet publish "./p3rpc.kotone.cutscenes.csproj" -c Release -o "$env:RELOADEDIIMODS/p3rpc.kotone.cutscenes" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location