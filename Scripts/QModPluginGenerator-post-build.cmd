set solutionDir=%~f1
set targetDir=%~f2
set configName=%3


if %configName% == "SN.STABLE" (
	xcopy "%targetDir%QModManager.QModPluginGenerator.dll" "%solutionDir%BepinexPackages\Subnautica_Packages\QModManager\patchers\QModManager\"  /I /Q /Y
	xcopy "%targetDir%QModInstaller.dll" "%solutionDir%BepinexPackages\Subnautica_Packages\QModManager\plugins\QModManager\"  /I /Q /Y
	xcopy "%targetDir%QModInstaller.xml" "%solutionDir%BepinexPackages\Subnautica_Packages\QModManager\plugins\QModManager\"   /I /Q /Y
	IF EXIST "%solutionDir%BepinexPackages\Subnautica_Packages\QModManager_Subnautica.zip" DEL /F "%solutionDir%BepinexPackages\Subnautica_Packages\QModManager_Subnautica.zip"
%solutionDir%packages\7-Zip.CommandLine.18.1.0\tools\7za.exe a "%solutionDir%BepinexPackages\Subnautica_Packages\QModManager_Subnautica.zip" "%solutionDir%BepinexPackages\Subnautica_Packages\QModManager\*"
)

if %configName% == "SN.EXP" (
	xcopy "%targetDir%QModManager.QModPluginGenerator.dll" "%solutionDir%BepinexPackages\Subnautica_Packages\QModManager_Exp\patchers\QModManager\"  /I /Q /Y
	xcopy "%targetDir%QModInstaller.dll" "%solutionDir%BepinexPackages\Subnautica_Packages\QModManager_Exp\plugins\QModManager\"  /I /Q /Y
	xcopy "%targetDir%QModInstaller.xml" "%solutionDir%BepinexPackages\Subnautica_Packages\QModManager_Exp\plugins\QModManager\"   /I /Q /Y
	IF EXIST "%solutionDir%BepinexPackages\Subnautica_Packages\QModManager_Subnautica_Experimental.zip" DEL /F "%solutionDir%BepinexPackages\Subnautica_Packages\QModManager_Subnautica_Experimental.zip"
%solutionDir%packages\7-Zip.CommandLine.18.1.0\tools\7za.exe a "%solutionDir%BepinexPackages\Subnautica_Packages\QModManager_Subnautica_Experimental.zip" "%solutionDir%BepinexPackages\Subnautica_Packages\QModManager_Exp\*"
)

if %configName% == "BZ.STABLE" (
	xcopy "%targetDir%QModManager.QModPluginGenerator.dll" "%solutionDir%BepinexPackages\BelowZero_Packages\QModManager\patchers\QModManager\"  /I /Q /Y
	xcopy "%targetDir%QModInstaller.dll" "%solutionDir%BepinexPackages\BelowZero_Packages\QModManager\plugins\QModManager\"  /I /Q /Y
	xcopy "%targetDir%QModInstaller.xml" "%solutionDir%BepinexPackages\BelowZero_Packages\QModManager\plugins\QModManager\"   /I /Q /Y
	IF EXIST "%solutionDir%BepinexPackages\BelowZero_Packages\QModManager_BelowZero.zip" DEL /F "%solutionDir%BepinexPackages\BelowZero_Packages\QModManager_BelowZero.zip"
%solutionDir%packages\7-Zip.CommandLine.18.1.0\tools\7za.exe a "%solutionDir%BepinexPackages\BelowZero_Packages\QModManager_BelowZero.zip" "%solutionDir%BepinexPackages\BelowZero_Packages\QModManager\*"
)

if %configName% == "BZ.EXP" (
	xcopy "%targetDir%QModManager.QModPluginGenerator.dll" "%solutionDir%BepinexPackages\BelowZero_Packages\QModManager_Exp\patchers\QModManager\"  /I /Q /Y
	xcopy "%targetDir%QModInstaller.dll" "%solutionDir%BepinexPackages\BelowZero_Packages\QModManager_Exp\plugins\QModManager\"  /I /Q /Y
	xcopy "%targetDir%QModInstaller.xml" "%solutionDir%BepinexPackages\BelowZero_Packages\QModManager_Exp\plugins\QModManager\"   /I /Q /Y
	IF EXIST "%solutionDir%BepinexPackages\BelowZero_Packages\QModManager_BelowZero_Experimental.zip" DEL /F "%solutionDir%BepinexPackages\BelowZero_Packages\QModManager_BelowZero_Experimental.zip"
%solutionDir%packages\7-Zip.CommandLine.18.1.0\tools\7za.exe a "%solutionDir%BepinexPackages\BelowZero_Packages\QModManager_BelowZero_Experimental.zip" "%solutionDir%BepinexPackages\BelowZero_Packages\QModManager_Exp\*"
)

