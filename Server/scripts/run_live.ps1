# QahrMobile: run the GameServer under .NET Hot Reload (dotnet watch). Saving a .cs file applies the change to the
# running server in about a second, so players stay logged in. Changes Hot Reload cannot apply (e.g. a changed method
# signature) are reported in watch.err.log and need a normal restart.
# Logs: watch.log (server console), watch.err.log (dotnet watch).
$ErrorActionPreference = 'Stop'
$env:DOTNET_ROOT = 'D:\dotnet10'
$root = Split-Path $PSScriptRoot -Parent
$proj = Join-Path $root 'src\Qahr.Mobile.Game'
$logs = Join-Path (Split-Path $root -Parent) 'Logs'
New-Item -ItemType Directory -Force $logs | Out-Null
$p = Get-NetTCPConnection -LocalPort 5828 -State Listen -ErrorAction SilentlyContinue | Select-Object -First 1 -ExpandProperty OwningProcess
if ($p) {
    # save every online player first: a forced stop skips the server's own shutdown save
    $py = 'D:\conquer\_private\comobile\venv\Scripts\python.exe'
    $admin = 'D:\conquer\_private\comobile\tools\admin.py'
    if ((Test-Path $py) -and (Test-Path $admin)) { & $py -I $admin save all=1 | Select-String 'message' }
}
# stop the old dotnet watch (if any) and the server
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -match 'dotnet-watch|Qahr.Mobile.Game.csproj|Qahr.Mobile.Game.exe' } |
    ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Sleep 2
# Hot Reload needs a non-optimized build
& "$env:DOTNET_ROOT\dotnet.exe" build "$proj\Qahr.Mobile.Game.csproj" -c Debug --property:Optimize=false | Select-String ' error |Error\(s\)'
$env:DOTNET_WATCH_RESTART_ON_RUDE_EDIT = 'false'
Start-Process -FilePath "$env:DOTNET_ROOT\dotnet.exe" -WorkingDirectory $proj -WindowStyle Minimized `
    -ArgumentList 'watch','--non-interactive','run','--project','Qahr.Mobile.Game.csproj','-c','Debug','--property:Optimize=false' `
    -RedirectStandardOutput (Join-Path $logs 'watch.log') -RedirectStandardError (Join-Path $logs 'watch.err.log')
'GameServer started under Hot Reload'
