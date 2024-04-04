# Show server machine information

# Arguments
$argsParam = [pscustomobject]@{
    Refresh = $false
    NIC = ""
    Summary = $false
}
for ($i = 0; $i -lt $args.Length; $i++) {
    switch ($args[$i]) {
        "-Refresh" { 
            $tempVal = $false
            if ([bool]::TryParse($args[++$i], [ref]$tempVal)) {
                $argsParam.Refresh = $tempVal
            }
        }
        "-NIC" {
            $argsParam.NIC = $args[++$i]
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
$operatingSystem = Get-CimInstance -Class "Win32_OperatingSystem"
$bios = Get-CimInstance -Class "Win32_BIOS"
$systemAccount = Get-CimInstance -Class "Win32_SystemAccount"
$nwAdapterConf = Get-CimInstance -Class "Win32_NetworkAdapterConfiguration" | `
    Where-Object { $_.IPEnabled }
$nwConf = $nwAdapterConf | Where { $_.DefaultIPGateway -ne $null }
if ($nwConf -eq $null) {
    $nwConf = $nwAdapterConf | Select-Object -First 1
}
$adapter = Get-CimInstance -Class "Win32_NetworkAdapter" | `
    Where-Object { $_.GUID -eq $nwConf.SettingID } | `
    Select-Object -First 1

$machineInfo = [pscustomobject]@{
    ComputerName = $env:COMPUTERNAME
    DomainName = $computerSystem.Domain.ToSTring()
    IsDomainMachine = [bool]$computerSystem.PartOfDomain
    SystemSIDs = $systemAccount.SID -join ", "
    Manufacturer = $computerSystem.Manufacturer
    Model = $computerSystem.Model
    SerialNumber = $bios.SerialNumber
    OSName = $operatingSystem.Caption
    OSVersion = $operatingSystem.Version
    Network = [pscustomObject]@{
        InterfaceName = $adapter.NetConnectionID
        IPAddress = $nwConf.IPAddress -join ", "
        SubnetMask = $nwConf.IPSubnet -join ", "
        DefaultGateway = $nwConf.DefaultIPGateway -join ", "
        DNS = $nwConf.DNSServerSearchOrder -join ", "
        MACAddress = $nwConf.MACAddress
    }
}

$machineInfo | ConvertTo-Json | Out-File -FilePath $cacheFile -Encoding utf8
$machineInfo | ConvertTo-Json
