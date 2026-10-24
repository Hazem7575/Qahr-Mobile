# Resource CDN mirror of the mobile client (official_mirror.py cdn-serve), see conquer-mobile\docs\OFFICIAL_MIRROR.md
#   powershell -ExecutionPolicy Bypass -File D:\conquer\QahrMobile\run_cdn.ps1            start (if not running)
#   powershell -ExecutionPolicy Bypass -File D:\conquer\QahrMobile\run_cdn.ps1 -Status    print RUNNING pid=.. / STOPPED
#   powershell -ExecutionPolicy Bypass -File D:\conquer\QahrMobile\run_cdn.ps1 -Stop      stop it
# Exit code: 0 = running (start/status) or stopped (stop), 1 = failed / not running (status).
param([switch]$Stop, [switch]$Status, [int]$Port = 9550)

$Root    = 'D:\Qahr Mobile\Server'
$Python  = 'D:\conquer\_private\comobile\venv\Scripts\python.exe'
$Tool    = Join-Path $Root 'Tools\OfficialMirror\official_mirror.py'
$Exe     = Join-Path $Root 'Tools\OfficialMirror\OfficialMirror.exe'   # the published build (no Python needed)
$DataDir = 'D:\Qahr Mobile\CDN'
$PidFile = Join-Path $DataDir 'cdn_serve.pid'
$OutLog  = Join-Path $DataDir 'cdn_serve.log'
$ErrLog  = Join-Path $DataDir 'cdn_serve.err.log'

function Get-CdnProcess {
    # the listener on the port, if it is our python
    $c = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($null -eq $c) { return $null }
    $p = Get-Process -Id $c.OwningProcess -ErrorAction SilentlyContinue
    return $p
}

$p = Get-CdnProcess
if ($Status) {
    if ($null -ne $p) { Write-Output "RUNNING pid=$($p.Id) port=$Port"; exit 0 }
    Write-Output "STOPPED port=$Port"; exit 1
}
if ($Stop) {
    if ($null -eq $p) { Write-Output "STOPPED port=$Port (was not running)"; exit 0 }
    Stop-Process -Id $p.Id -Force
    Remove-Item $PidFile -ErrorAction SilentlyContinue
    Write-Output "STOPPED pid=$($p.Id)"; exit 0
}
if ($null -ne $p) { Write-Output "RUNNING pid=$($p.Id) port=$Port (already)"; exit 0 }

$serve = @('cdn-serve', '--port', "$Port", '--bind', '0.0.0.0')
if (Test-Path $Exe) { $Run = $Exe; $argsList = $serve }
else { $Run = $Python; $argsList = @('-I', "`"$Tool`"") + $serve }
$proc = Start-Process -FilePath $Run -ArgumentList $argsList -WindowStyle Hidden -PassThru `
        -RedirectStandardOutput $OutLog -RedirectStandardError $ErrLog
Set-Content -Path $PidFile -Value $proc.Id -Encoding ascii
for ($i = 0; $i -lt 20; $i++) {
    Start-Sleep -Milliseconds 250
    $p = Get-CdnProcess
    if ($null -ne $p) { Write-Output "RUNNING pid=$($p.Id) port=$Port (started, log $OutLog)"; exit 0 }
    if ($proc.HasExited) { break }
}
Write-Output "FAILED to start, see $ErrLog"; exit 1
