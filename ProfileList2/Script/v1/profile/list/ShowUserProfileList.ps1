# Show user profile list

$systemAccounts = (Get-CimInstance -ClassName "Win32_SystemAccount" | `
        Where-Object { $_.SID -ne $null }).SID

        
$userProfiles = Get-CimInstance -ClassName "Win32_UserProfile" | `
    Where-Object { !($systemAccounts -contains $_.SID) }
$userAccount = Get-CimInstance -ClassName "Win32_UserAccount" | `
    Where-Object { $_.SID -ne $null }

$list = @()
foreach ($profile in $userProfile) {
    $profileParameter = [pscustomobject]@{
        UserName     = ""
        UserDomain   = ""
        Caption      = ""
        ProfilePath  = ""
        SID          = ""
        IsLogon      = $false
        IsDomainuser = $false
    }
    $user = $userAccount | Where-Object { $_.SID -eq $profile.SID }
    if ($user -ne $null) {
        $profileParameter.UserName = $user.Name
        $profileParameter.UserDomain = $user.Domain
        $profileParameter.Caption = $user.Caption
        $profileParameter.IsDomainuser = !($user.LocalAccount)

    }
    else {
        $profileParameter.UserName = "-"
        $profileParameter.Caption = "Unknown"
    }
    $list += $profileParameter
}

