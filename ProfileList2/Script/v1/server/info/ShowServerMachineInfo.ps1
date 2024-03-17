# Show server machine information

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
$computerSystem = Get-CimInstance -Class "Win32_ComputerSystem"
$systemAccount = Get-CimInstance -Class "Win32_SystemAccount"
$nwAdapterConf = Get-CimInstance -Class "Win32_NetworkAdapterConfiguration" | Where { $_.IPEnabled }

$computerName = $env:COMPUTERNAME
$domainName = $computerSystem.Domain.ToSTring()
$isDomainMachine = [bool]$computerSystem.PartOfDomain
$systemSIDs = $systemAccount.SID







