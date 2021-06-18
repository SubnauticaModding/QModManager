set solutionDir=%~f1
set targetDir=%~f2
set configName=%3

rmdir "%solutionDir%VortexBuild\%configName%" /q /s
xcopy "%solutionDir%Dependencies\BepInEx" "%solutionDir%VortexBuild\%configName%"  /E /H /I /Q /Y
xcopy "%solutionDir%Dependencies\%configName%\BepInEx.cfg" "%solutionDir%VortexBuild\%configName%\BepInEx\config\"  /I /Q /Y
mkdir "%solutionDir%VortexBuild\%configName%\QMods"
xcopy "%solutionDir%Dependencies\cldb.dat" "%solutionDir%VortexBuild\%configName%\BepInEx\patchers\QModManager\" /I /Q /Y
xcopy "%solutionDir%packages\AssetsTools.NET.2.0.9\lib\net40\AssetsTools.NET.dll" "%solutionDir%VortexBuild\%configName%\BepInEx\patchers\QModManager\" /I /Q /Y
xcopy "%targetDir%QModManager.exe" "%solutionDir%VortexBuild\%configName%\BepInEx\patchers\QModManager\"   /I /Q /Y
xcopy "%targetDir%QModManager.QModPluginGenerator.dll" "%solutionDir%VortexBuild\%configName%\BepInEx\patchers\QModManager\"  /I /Q /Y
xcopy "%targetDir%QModManager.UnityAudioFixer.dll" "%solutionDir%VortexBuild\%configName%\BepInEx\patchers\QModManager\"  /I /Q /Y
xcopy "%targetDir%QModManager.UnityAudioFixer.xml" "%solutionDir%VortexBuild\%configName%\BepInEx\patchers\QModManager\" /I /Q /Y

if NOT "%configName%"  =="SN.STABLE" (
    xcopy "%solutionDir%Dependencies\Oculus.Newtonsoft.Json.dll" "%solutionDir%VortexBuild\%configName%\BepInEx\patchers\QModManager\" /I /Q /Y
    xcopy "%targetDir%QModManager.OculusNewtonsoftRedirect.dll" "%solutionDir%VortexBuild\%configName%\BepInEx\patchers\QModManager\"  /I /Q /Y
)

xcopy "%targetDir%QModInstaller.dll" "%solutionDir%VortexBuild\%configName%\BepInEx\plugins\QModManager\"  /I /Q /Y
xcopy "%targetDir%QModInstaller.xml" "%solutionDir%VortexBuild\%configName%\BepInEx\plugins\QModManager\"   /I /Q /Y

%solutionDir%packages\7-Zip.CommandLine.18.1.0\tools\7za.exe a "%solutionDir%VortexBuild\QModManager_%configName%.zip" "%solutionDir%VortexBuild\%configName%\*"

echo F|xcopy /S /Q /Y /F  "%solutionDir%Installer\%configName%.iss" "%targetDir%\QModsInstallerScript.iss"
"%solutionDir%Dependencies\Inno\ISCC.exe" "%targetDir%QModsInstallerScript.iss"