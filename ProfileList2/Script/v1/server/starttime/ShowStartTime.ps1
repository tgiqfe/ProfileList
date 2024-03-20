# Show server machine information

# Arguments
$argsParam = [pscustomobject]@{
    Refresh = $false
    After   = [System.DateTime]::Today
    Before  = [System.DateTime]::Today.AddDays(1)
}
function ShapeDateText($text) {
    if ($text -match "^\d{14}$") {
        return $text.Substring(0, 4) + "/" + $text.Substring(4, 2) + "/" + $text.Substring(6, 2) + " " + $text.Substring(8, 2) + ":" + $text.Substring(10, 2) + ":" + $text.Substring(12, 2)
    }
    elseif ($text -match "^\d{8}$") {
        return $text.Substring(0, 4) + "/" + $text.Substring(4, 2) + "/" + $text.Substring(6, 2)
    }
    elseif ($text -match "^\d{8} \d{6}$") {
        return $text.Substring(0, 4) + "/" + $text.Substring(4, 2) + "/" + $text.Substring(6, 2) + " " + $text.Substring(9, 2) + ":" + $text.Substring(11, 2) + ":" + $text.Substring(13, 2)
    }
    else {
        return $text
    }
}
for ($i = 0; $i -lt $args.Length; $i++) {
    switch ($args[$i]) {
        "-Refresh" { 
            $tempVal = $argsParam.Refresh
            if ([bool]::TryParse($args[++$i], [ref]$tempVal)) {
                $argsParam.Refresh = $tempVal
            }
        }
        "-After" {
            $tempDate = $argsParam.After
            $dateText = ShapeDateText $args[++$i]
            if ([System.DateTime]::TryParse($dateText, [ref]$tempDate)) {
                $argsParam.After = $tempDate
            }
        }
        "-Before" {
            $tempDate = $argsParam.Before
            $dateText = ShapeDateText $args[++$i]
            if ([System.DateTime]::TryParse($dateText, [ref]$tempDate)) {
                $argsParam.Before = $tempDate
            }
        }
    }
}
if($argsParam.Before -lt $argsParam.After){
    $tempDate = $argsParam.Before
    $argsParam.Before = $argsParam.After
    $argsParam.After = $tempDate
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

$StartTimeInfo = [pscustomobject]@{
    SearchRange      = "[" + $argsParam.After.ToString("yyyy/MM/dd HH:mm:ss") + "]~[" + $argsParam.Before.ToString("yyyy/MM/dd HH:mm:ss") + "]"
    BootAndLogonTime = @()
    Hiberboot        = "Nothing"
}

# Get hiberboot setting
Using-Object($regKey = [Microsoft.Win32.Registry]::LocalMachine.OpenSubKey("SYSTEM\ControlSet001\Control\Session Manager\Power", $false)) {
    $valNames = $regkey.GetValueNames()
    if ($valNames -contains "HiberbootEnabled") {
        $ret = $regKey.GetValue("HiberbootEnabled") -as [int]
        $StartTimeInfo.Hiberboot = . {
            if ($ret -eq 1) { "Enable" } else { "Disable" }
        }
    }
}

# 6005 : Boot
# 6006 : Shutdown (normal)
# 6008 : Shutdown (abnormal)
# 7001 : Logon
# 7002 : Logoff
$ids = @(6005, 6006, 6008, 7001, 7002)
$logEvents = Get-EventLog `
    -LogName System `
    -After $argsParam.After `
    -Before $argsParam.Before | `
    Where-Object { $ids -contains $_.EventId } | `
    Where-Object { $_.Source -eq "Microsoft-Windows-Winlogon" -or $_.Source -eq "EventLog" }

$list = @()
function GetNameFromSID($sids) {
    $sid = $sids | Where-Object { $_.ToString().StartsWith("S-") } | Select-Object -First 1
    $sidObj = New-Object System.Security.Principal.SecurityIdentifier($sid)
    return $sidObj.Translate([System.Security.Principal.NTAccount]).Value
}
[Array]::Reverse($logEvents)
$logEvents | Foreach-Object {
    $obj = $_
    $logTime = $_.TimeWritten.ToString("yyyy/MM/dd HH:mm:ss")
    switch ($_.EventId) {
        6005 { $list += "[${logTime}] (6005) Boot" }
        6006 { $list += "[${logTime}] (6006) Shutdown, normal" }
        6008 { $list += "[${logTime}] (6006) Shutdown, abnormal" }
        7001 {
            $name = GetNameFromSID $obj.ReplacementStrings
            $list += "[${logTime}] (7001) Logon, user:${name}" 
        }
        7002 { 
            $name = GetNameFromSID $obj.ReplacementStrings
            $list += "[${logTime}] (7002) Logoff, user:${name}"
        }
    }
}
$StartTimeInfo.BootAndLogonTime = $list

$StartTimeInfo | ConvertTo-Json | Out-File -FilePath $cacheFile -Encoding utf8
$StartTimeInfo | ConvertTo-Json
