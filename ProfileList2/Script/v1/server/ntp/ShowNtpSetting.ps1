# Show server machine information

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
function Using-Object {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)][AllowEmptyString()][AllowEmptyCollection()][AllowNull()][object]$InputObject,
        [Parameter(Mandatory = $true)][scriptblock]$ScriptBlock
    )
    try { . $ScriptBlock }
    finally { if (($null -ne $InputObject) -and ($InputObject -is [System.IDisposable])) { $InputObject.DIspose() } }
}

$ntpSettingInfo = [pscustomobject]@{
    NtpServerSetting = [pscustomobject]@{
        Servers        = ""
        SelectedServer = ""
    }
    NtpService = [pscustomobject]@{
        ServiceName = "W32Time"
        DisplayName = ""
        Status = ""
        StartupType = ""
    }
    W32TimeResult = [pscustomobject]@{
        Peers = @()
        Status = [pscustomobject]@{
            LastSucdssfulSync = ""
            Source = ""
        }
    }
}

# Get NTP server setting from registry.
Using-Object($regKey = [Microsoft.Win32.Registry]::LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\DateTime\Servers", $false)) {
    $index = $regKey.GetValue("", "1") -as [string]
    $ntpSettingInfo.NtpServerSetting.Servers = $regKey.GetValueNames() | `
        Where-Object { $_ -ne "" } | `
        ForEach-Object { $regKey.getValue($_) -as [string] }
    $ntpSettingInfo.NtpServerSetting.SelectedServer = $regKey.GetValue($index, "") -as [string]
}

# Get NTP service status
$sv = Get-Service -Name $ntpSettingInfo.NtpService.ServiceName
$ntpSettingInfo.NtpService.DisplayName = $sv.DisplayName
$ntpSettingInfo.NtpService.Status = $sv.Status
$ntpSettingInfo.NtpService.StartupType = $sv.StartType

# Get NTP status from w32tm command
$keyword_peer = "ピア: "
$keyword_status = "状態: "
$keyword_lastSucdssfulSync = "最終正常同期時刻: "
$keyword_source = "ソース: "
if($sv.Status -eq "Running") {
    # Get NTP peers
    $peer_peer = @()
    $peer_status = @()
    $output = (& w32tm /query /peers) -split "\r\n" | Select-Object -Skip 2
    foreach($line in $output) {
        if($line.StartsWith($keyword_peer)){
            $peer_peer += $line -replace $keyword_peer, ""
        }elseif($line.StartsWith($keyword_status)){   
            $peer_status += $line -replace $keyword_status, ""
        }
    }
    for($i = 0; $i -lt $peer_peer.Count; $i++) {
        $ntpSettingInfo.W32TimeResult.Peers += [pscustomobject]@{
            Peer = $peer_peer[$i]
            Status = $peer_status[$i]
        }
    }

    # Get NTP status
    $output = (& w32tm /query /status) -split "\r\n"
    foreach($line in $output) {
        if($line.StartsWith($keyword_lastSucdssfulSync)){
            $ntpSettingInfo.W32TimeResult.Status.LastSucdssfulSync = $line -replace $keyword_lastSucdssfulSync, ""
        }elseif($line.StartsWith($keyword_source)){   
            $ntpSettingInfo.W32TimeResult.Status.Source = $line -replace $keyword_source, ""
        }
    }
}

$ntpSettingInfo | ConvertTo-Json -Depth 5 -EnumsAsStrings | Out-File -FilePath $cacheFile -Encoding utf8
$ntpSettingInfo | ConvertTo-Json -Depth 5 -EnumsAsStrings
