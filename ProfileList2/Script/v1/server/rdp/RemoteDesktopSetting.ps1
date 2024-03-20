# Remote desktop setting, show or change.

# Arguments
$argsParam = [pscustomobject]@{
    Refresh           = $false
    ToEnable          = $false
    ToDisable         = $false
    UseAuthentication = $true
}
for ($i = 0; $i -lt $args.Length; $i++) {
    switch ($args[$i]) {
        "-Refresh" { 
            $tempVal = $argsParam.Refresh
            if ([bool]::TryParse($args[++$i], [ref]$tempVal)) {
                $argsParam.Refresh = $tempVal
            }
        }
        "-ToEnable" { 
            $tempVal = $argsParam.ToEnable
            if ([bool]::TryParse($args[++$i], [ref]$tempVal)) {
                $argsParam.ToEnable = $tempVal
            }
        }
        "-ToDisable" { 
            $tempVal = $argsParam.ToDisable
            if ([bool]::TryParse($args[++$i], [ref]$tempVal)) {
                $argsParam.ToDisable = $tempVal
            }
        }
        "-UseAuthentication" { 
            $tempVal = $argsParam.UseAuthentication
            if ([bool]::TryParse($args[++$i], [ref]$tempVal)) {
                $argsParam.UseAuthentication = $tempVal
            }
        }
    }
}

# Cached parameter
$cacheFile = ".\Cache.json"
if ((Test-Path -Path $cacheFile -PathType Leaf) -and 
    !($argsparam.Refresh) -and
    !($argsparam.ToEnable) -and
    !($argsparam.ToDisable)) {
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

$RDPInfo = [pscustomobject]@{
    Enabled           = $false
    UseAuthentication = $false
    Firewall          = @()
    Service           = @()
    Action            = ""
}

function CheckRDPSetting() {
    Using-Object($regKey = [Microsoft.Win32.Registry]::LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Terminal Server", $false)) {
        if ($null -ne $regKey) {
            $RDPInfo.Enabled = $regKey.GetValue("fDenyTSConnections") -eq 0
        }
    }
    Using-Object($regKey = [Microsoft.Win32.Registry]::LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp", $false)) {
        if ($null -ne $regKey) {
            $RDPInfo.UseAuthentication = $regKey.GetValue("UserAuthentication") -eq 1
        }
    }
}
function CheckServiceAndFW() {
    @("RemoteDesktop-UserMode-In-TCP", "RemoteDesktop-UserMode-In-UDP", "RemoteDesktop-Shadow-In-TCP") | ForEach-Object { 
        $fw = Get-NetFirewallRule -Name $_
        $RDPInfo.Firewall += [pscustomobject]@{
            Name        = $fw.Name
            DisplayName = $fw.DisplayName
            Enabled     = $fw.Enabled
        }
    }
    @("SessionEnv", "TermService", "UmRdpService") | ForEach-Object { 
        $sv = Get-Service -Name $_
        $RDPInfo.Service += [pscustomobject]@{
            Name        = $sv.Name
            DisplayName = $sv.DisplayName
            StartupType = $sv.StartType
            Status      = $sv.Status
        }
    }
}

CheckRDPSetting
if (!($RDPInfo.Enabled) -and $argsParam.ToEnable) {
    # RDP: disable -> enable
    $isAdmin = (New-Object System.Security.Principal.WindowsPrincipal(
            [System.Security.Principal.WindowsIdentity]::GetCurrent())).IsInRole(
        [System.Security.Principal.WindowsBuiltInRole]::Administrator)
    if ($isAdmin) {
        $RDPInfo.Action = "ToEnable"
        Using-Object($regKey = [Microsoft.Win32.Registry]::LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Terminal Server", $true)) {
            $regKey.SetValue("fDenyTSConnections", 0, [Microsoft.Win32.RegistryValueKind]::DWord)
        }
        if ($RDPInfo.UseAuthentication -ne $argsParam.UseAuthentication) {
            $changeValue = . { if ($argsParam.UseAuthentication) { 1 } else { 0 } }
            Using-Object($regKey = [Microsoft.Win32.Registry]::LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp", $true)) {
                $regKey.SetValue("UserAuthentication", $changeValue, [Microsoft.Win32.RegistryValueKind]::DWord)
            }
        }
        @("RemoteDesktop-UserMode-In-TCP", "RemoteDesktop-UserMode-In-UDP", "RemoteDesktop-Shadow-In-TCP") | ForEach-Object { 
            Get-NetFirewallRule -Name $_ | Set-NetFirewallRule -Enabled true 
        }
        @("SessionEnv", "TermService", "UmRdpService") | ForEach-Object { 
            $sv = Get-Service -Name $_
            if ($sv.Status -eq "Stopped" -or $sv.Status -eq "StopPending") {
                Start-Service -Name $_ -ErrorAction SilentlyContinue 
            }
        }
        CheckRDPSetting
    }
    else {
        $RDPInfo.Action = "ToEnable failed, NotAdmin"
    }
    CheckServiceAndFW
}
elseif ($RDPInfo.Enabled -and $argsParam.ToDisable) {
    # RDP: enable -> disable
    $isAdmin = (New-Object System.Security.Principal.WindowsPrincipal(
            [System.Security.Principal.WindowsIdentity]::GetCurrent())).IsInRole(
        [System.Security.Principal.WindowsBuiltInRole]::Administrator)
    if ($isAdmin) {
        $RDPInfo.Action = "ToDisable"
        Using-Object($regKey = [Microsoft.Win32.Registry]::LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Terminal Server", $true)) {
            $regKey.SetValue("fDenyTSConnections", 1, [Microsoft.Win32.RegistryValueKind]::DWord)
        }
        @("RemoteDesktop-UserMode-In-TCP", "RemoteDesktop-UserMode-In-UDP", "RemoteDesktop-Shadow-In-TCP") | ForEach-Object {
            Get-NetFirewallRule -Name $_ | Set-NetFirewallRule -Enabled false
        }
        CheckRDPSetting
    }
    else {
        $RDPInfo.Action = "ToDisable failed, NotAdmin"
    }
    CheckServiceAndFW
}
else {
    # No action
    $RDPInfo.Action = "NoAction"
    CheckServiceAndFW
}

$RDPInfo | ConvertTo-Json -Depth 5 -EnumsAsStrings | Out-File -FilePath $cacheFile -Encoding utf8
$RDPInfo | ConvertTo-Json -Depth 5 -EnumsAsStrings

