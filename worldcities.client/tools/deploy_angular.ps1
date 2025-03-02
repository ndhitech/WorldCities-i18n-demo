$tagetRootDir="c:\demo"

Write-Output $env:COMPUTERNAME

switch ($env:COMPUTERNAME)
{
    {$_ -eq "ND-SKWINSVR02"} { $tagetRootDir = "c:\demo"}
    {$_ -eq "ZHANG-ASUS2020"} { $tagetRootDir = "c:\Test_demo"}
}


$currentToolPath = (Get-Location).ProviderPath
$deployHistoryFileName = $currentToolPath + "\logs\DeployHistory" + (Get-Date -format "yyyy-MM-dd") + "-result.txt"
$logFileName = $logFileName = "'" + $currentToolPath + "\logs\DeployLog" + (Get-Date -format "yyyy-MM-ddTHH-MM-ss.fff") + $deployOption + "-result.txt" + "'"
Start-Transcript -Path $deployHistoryFileName -Append
Write-Output $deployHistoryFileName

$sourceDir= "\..\dist\browser\"
$sourceFullDir= (Get-Location).ProviderPath + $sourceDir
$sourceFullDir_azure = $sourceFullDir.Replace("\","/")

$targetAppName = $args[1] 
$targetAppDir = $targetAppName + ".client"
$targetAppFullDir = $tagetRootDir + "\"+ $targetAppDir

$deployOption = "production"

Write-Output  ($deployMessage + " started ...")

switch ($args[0])
{
    {$_ -eq "azure"} { $buildOption = "--configuration=azure_bwm"}
    {$_ -eq "burnaby"} { $buildOption = "--configuration=burnaby_hosting"}
    {$_ -eq "production"} { $buildOption = "--configuration=production"}
    {$_ -eq "development"} { $buildOption = "--configuration=development"}
    {$_ -eq "local"} { $buildOption = "--configuration=development"}
}

$deployMessage = "Deploying Angular front end application to " + $targetDir + " for " +  $deployOption 
Write-Output  ($deployMessage + " started ...")

if ($args[0] -eq "azure")
{
	$deplopyCommand = "swa deploy "`
        + "--app-location ../dist/browser " `
        + "--app-name " + $targetAppName + " " `
        + "--tenant-id 225b1324-d511-49c2-9b6d-6c85beb8da7d "`
        + "--subscription-id 1af23237-6a1e-40e9-98a8-bdd4268ff291 " `
        + "--env production -nu"
        $setCurrentPath = "Set-Location -path " + "'" + $currentToolPath + "'"
        Write-Output $setCurrentPath
        Invoke-Expression $setCurrentPath
        $cmdString = " deploy_azure.cmd " + $targetAppName
        Write-Output $cmdString
        Start-Process deploy_azure.cmd $targetAppName
}

if ($args[0] -eq "burnaby")
{
	$deplopyCommand = "xcopy "`
        +  "'" + $sourceFullDir + "' "`
        +  "'" + $targetAppFullDir +"'"`
        +  " /s /q /y " `
        +  " >" + $logFileName
         Write-Output $deplopyCommand
         powershell.exe $deplopyCommand
}
 
Write-Output  ($deployMessage + " completed ...")
Stop-Transcript