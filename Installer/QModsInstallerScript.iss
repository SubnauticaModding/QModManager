; Throws an error if the version used to compile this script is not 5.6.1 unicode
; This ensures that the application is build correctly
#if VER < EncodeVer(5, 6, 1)
  #error A newer version of Inno Setup is required to compile this script (5.6.1 unicode)
#endif
#if VER > EncodeVer(5, 6, 1)
  #error An older version of Inno Setup is required to compile this script (5.6.1 unicode)
#endif
#if !Defined(UNICODE)
  #error An unicode version of Inno Setup is required to compile this script (5.6.1 unicode)
#endif

; Defines some variables
#define Name "QModManager"
#define Version "1.4"
#define Author "the QModManager team"
#define URL "https://github.com/Qwiso/QModManager"
#define SupportURL "https://discord.gg/UpWuWwq"
#define UpdatesURL "https://nexusmods.com/subnautica/mods/16"

; Defines special flags that change the way the installer behaves
#define PreRelease true ; If this is true, a window will appear, promting the user to agree to download even if this is a prerelease

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{52CC87AA-645D-40FB-8411-510142191678}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={code:GetDefaultDir}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=.
OutputBaseFilename=QModsSetup
SetupIconFile=QModsIcon.ico
Compression=lzma
SolidCompression=yes
WizardImageFile=WizardImage.bmp
WizardSmallImageFile=WizardSmallImage.bmp
DisableWelcomePage=no
DisableDirPage=no
DirExistsWarning=no
UsePreviousAppDir=no

[Messages]
ExitSetupMessage=Setup is not complete. If you exit now, QMods will not be installed.%n%nExit Setup?
SelectDirLabel3=Please select your Subnautica install folder.
SelectDirBrowseLabel=If this is correct, click Next. If you need to select a different Subnautica install folder, click Browse.
WizardSelectDir=Select Subnautica Install Location

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "QModManager\0Harmony.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: ignoreversion
Source: "QModManager\Mono.Cecil.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: ignoreversion
Source: "QModManager\Newtonsoft.Json.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: ignoreversion
Source: "QModManager\QModInstaller.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: ignoreversion
Source: "QModManager\QModManager.exe"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: ignoreversion

[Run]
Filename: "{app}\Subnautica_Data\Managed\QModManager.exe"; Parameters: "-i"

[UninstallRun]
Filename: "{app}\Subnautica_Data\Managed\QModManager.exe"; Parameters: "-u"

[Code]
function GetDefaultDir(def: string): string;
var
I : Integer;
P : Integer;
steamInstallPath : string;
configFile : string;
fileLines: TArrayOfString;
begin
	steamInstallPath := 'not found';
	if RegQueryStringValue( HKEY_LOCAL_MACHINE, 'SOFTWARE\WOW6432Node\Valve\Steam', 'InstallPath', steamInstallPath ) then
	begin
	end;

	if FileExists(steamInstallPath + '\steamapps\common\Subnautica\Subnautica.exe') then
	begin
		Result := steamInstallPath + '\steamapps\common\Subnautica';
		Exit;
	end
	else
	begin
		configFile := steamInstallPath + '\config\config.vdf';
		if FileExists(configFile) then
		begin
			if LoadStringsFromFile(configFile, FileLines) then
			begin
				for I := 0 to GetArrayLength(FileLines) - 1 do
				begin
					P := Pos('BaseInstallFolder_', FileLines[I]);
					if P > 0 then
					begin
						steamInstallPath := Copy(FileLines[I], P + 23, Length(FileLines[i]) - P - 23);
						if FileExists(steamInstallPath + '\steamapps\common\Subnautica\Subnautica.exe') then
						begin
							Result := steamInstallPath + '\steamapps\common\Subnautica';
							Exit;
						end;
					end;
				end;
			end;
		end;
	end;
	
	Result := 'not found';
end;