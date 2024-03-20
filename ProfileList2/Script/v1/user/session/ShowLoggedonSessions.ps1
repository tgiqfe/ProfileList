# Show logged on sessions

# Arguments
$argsParam = [pscustomobject]@{
    Refresh = $false
}
for ($i = 0; $i -lt $args.Length; $i++) {
    switch ($args[$i]) {
        "-Refresh" { 
            $tempVal = $argsParam.Refresh
            if ([bool]::TryParse($args[++$i], [ref]$tempVal)) {
                $argsParam.Refresh = $tempVal
            }
        }
    }
}

# Cached parameter
$cacheFile = ".\Cache.json"
if ((Test-Path -Path $cacheFile -PathType Leaf) -and !($argsparam.Refresh)) {
    Get-Content -Path $cacheFile
    exit
}

# Main
. "..\..\Scripts\LogonSession.ps1"
$userLogonSessions = , [UserLogonSession]::GetLoggedOnSession()

$userLogonSessions | ConvertTo-Json | Out-File -FilePath $cacheFile -Encoding utf8
$userLogonSessions | ConvertTo-Json
