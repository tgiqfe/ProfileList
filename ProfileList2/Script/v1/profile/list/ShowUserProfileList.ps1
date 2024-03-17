# Show user profile list

# Arguments
$argsParam = [pscustomobject]@{
    Refresh = $false
}
for ($i = 0; $i -lt $args.Length; $i++) {
    switch ($args[$i]) {
        "-Refresh" { 
            $tempVal = $false
            if([bool]::TryParse($args[++$i], [ref]$tempVal)){
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

$systemAccounts = (Get-CimInstance -ClassName "Win32_SystemAccount" | `
        Where-Object { $_.SID -ne $null }).SID
        
$userProfiles = Get-CimInstance -ClassName "Win32_UserProfile" | `
    Where-Object { !($systemAccounts -contains $_.SID) }
$userAccount = Get-CimInstance -ClassName "Win32_UserAccount" | `
    Where-Object { $_.SID -ne $null }

$list = @()
foreach ($profile in $userProfiles) {
    $profileParameter = [pscustomobject]@{
        UserName     = ""
        UserDomain   = ""
        Caption      = ""
        ProfilePath  = ""
        SID          = ""
        IsLogon      = $false
        IsDomainuser = $false
    }
    $account = $userAccount | `
        Where-Object { $_.SID -eq $profile.SID } | `
        Select-Object -First 1
    if ($null -ne $account) {
        $profileParameter.UserName = $account.Name
        $profileParameter.UserDomain = $account.Domain
        $profileParameter.Caption = $account.Caption
        $profileParameter.ProfilePath = $profile.LocalPath
        $profileParameter.SID = $profile.SID
        $profileParameter.IsLogon = $userLogonSessions | `
            Where-Object { $_.UserName -eq $account.Name -and $_.UserDomain -eq $account.Domain } | `
            Select-Object -First 1 | `
            ForEach-Object { $_.IsActive() }
        $profileParameter.IsDomainuser = !($account.LocalAccount)
    }
    else {
        $profileParameter.UserName = "-"
        $profileParameter.Caption = "Unknown"
    }
    $list += $profileParameter
}

, $list | ConvertTo-Json | Out-File -FilePath $cacheFile -Encoding utf8
, $list | ConvertTo-Json
