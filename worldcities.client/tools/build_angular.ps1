$buildHistoryFileName = (Get-Location).ProviderPath + "\logs\BuildHistory" + (Get-Date -format "yyyy-MM-dd") + "-result.txt" 
Start-Transcript -Path $buildHistoryFileName -Append

$buildOption = "--configuration=production"

switch ($args[0])
{
    {$_ -eq "azure"} { $buildOption = "--configuration=azure_bwm"}
    {$_ -eq "burnaby"} { $buildOption = "--configuration=burnaby_hosting"}
    {$_ -eq "production"} { $buildOption = "--configuration=production"}
    {$_ -eq "development"} { $buildOption = "--configuration=development"}
    {$_ -eq "local"} { $buildOption = "--configuration=development"}
}

$logFileName = "'" + (Get-Location).ProviderPath + "\logs\BuildLog" + (Get-Date -format "yyyy-MM-ddTHH-MM-ss.fff") + $buildOption + "-result.txt" +"'"

$buildCommand= "ng build " + $buildOption + ' >' + $logFileName

$buildMessage = "Builing ing Angular front end application " 
Write-Output  ($buildMessage + " started ...") 
Write-Output $buildCommand
Invoke-Expression $buildCommand 
Write-Output  ($buildMessage + " completed ...") 
Stop-Transcript