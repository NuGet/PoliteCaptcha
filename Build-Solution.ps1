$path = Split-Path $MyInvocation.MyCommand.Path
$slnFile = join-path $path PoliteCaptcha.msbuild
 
& "$(get-content env:windir)\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" $slnFile /t:PackageSolution