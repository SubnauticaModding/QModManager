; Throws an error if the version used to compile this script is not unicode
; This ensures that the application is built correctly
#if !Defined(UNICODE)
  #error A unicode version of Inno Setup is required to compile this script
#endif

#define Name "QModManager" ; The name of the game will be added after it
#define Version "4.1.1"
#define Author "QModManager"
#define URL "https://github.com/QModManager/QModManager"
#define SupportURL "https://discord.gg/UpWuWwq"
#define UpdatesURL "https://nexusmods.com" ; The link to the mod will be added after it

[Setup]
AllowNetworkDrive=no
AllowUNCPath=no
AlwaysShowDirOnReadyPage=yes
AppendDefaultDirName=no
AppId={code:GetGUID}
AppName={#Name}
AppPublisher={#Author}
AppPublisherURL={#URL}
AppSupportURL={#SupportURL}
AppUpdatesURL={code:GetURL}
AppVerName={#Name} {#Version}
AppVersion={#Version}
Compression=lzma
DefaultDirName=.
DirExistsWarning=no
DisableDirPage=no
DisableProgramGroupPage=yes
DisableWelcomePage=no
EnableDirDoesntExistWarning=yes
OutputBaseFilename=QModManager_{#Version}_BelowZero_Setup
OutputDir=.\
PrivilegesRequired=admin
SetupIconFile=..\..\Assets\Icon.ico
SolidCompression=yes
UninstallDisplayIcon={app}\BepInEx\patchers\QModManager\QModManager.exe
UninstallDisplayName={code:GetName}
UsePreviousAppDir=no
UsePreviousLanguage=no
WizardImageFile=..\..\Assets\InstallerImage.bmp
WizardSmallImageFile=..\..\Assets\InstallerSmallImage.bmp
UsePreviousSetupType=False
UsePreviousTasks=False
CloseApplications=False

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
; Files used by the installer but not required by QModManager itself
; Installer theme
Source: "..\..\Dependencies\VclStylesinno.dll"; Flags: DontCopy
Source: "..\..\Dependencies\Carbon.vsf"; Flags: DontCopy
; Installer extensions
Source: "InstallerExtensions.dll"; Flags: DontCopy

; Files required by QModManager itself
; Dependencies
Source: "..\..\packages\AssetsTools.NET.2.0.3\lib\net35\AssetsTools.NET.dll"; DestDir: "{app}\BepInEx\patchers\QModManager"; Flags: ignoreversion;
Source: "..\..\Dependencies\cldb.dat"; DestDir: "{app}\BepInEx\patchers\QModManager"; Flags: ignoreversion;
Source: "..\..\Dependencies\Oculus.Newtonsoft.Json.dll"; DestDir: "{app}\BepInEx\patchers\QModManager"; Flags: ignoreversion;

; QMM
Source: "QModInstaller.dll"; DestDir: "{app}\BepInEx\plugins\QModManager"; Flags: ignoreversion;
Source: "QModInstaller.xml"; DestDir: "{app}\BepInEx\plugins\QModManager"; Flags: ignoreversion;
Source: "QModManager.exe"; DestDir: "{app}\BepInEx\patchers\QModManager"; Flags: ignoreversion;

; BepInEx patchers
Source: "QModManager.OculusNewtonsoftRedirect.dll"; DestDir: "{app}\BepInEx\patchers\QModManager"; Flags: ignoreversion;
Source: "QModManager.QModPluginGenerator.dll"; DestDir: "{app}\BepInEx\patchers\QModManager"; Flags: ignoreversion;
Source: "QModManager.UnityAudioFixer.dll"; DestDir: "{app}\BepInEx\patchers\QModManager"; Flags: ignoreversion;
Source: "QModManager.UnityAudioFixer.xml"; DestDir: "{app}\BepInEx\patchers\QModManager"; Flags: ignoreversion;

; BepInEx
Source: "..\..\Dependencies\BepInEx\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs replacesameversion sharedfile uninsnosharedfileprompt;

[Dirs]
Name: "{app}\QMods"

[Run]
Filename: "{app}\BepInEx\patchers\QModManager\QModManager.exe"; Parameters: "-c"; Tasks: cleanup
 
[UninstallRun]
Filename: "{app}\BepInEx\patchers\QModManager\QModManager.exe"; Parameters: "-u";

[Messages]
; BeveledLabel={#Name} {#Version}
WizardSelectDir=Select install location
SelectDirLabel3=Please select the install folder of the game.
SelectDirBrowseLabel=To continue, click Next. If you would like to select a different folder, click Browse.%nIf you have the game on steam, you can also use the buttons on the bottom left to auto-complete the install path for the chosen game.
ReadyLabel2a=By installing, you agree to allow QModManager to send external web requests, most often to check for updates. You can disable this option at any time in the Mods tab of the Subnautica options menu.
ExitSetupMessage=Setup is not complete. If you exit now, {#Name} will not be installed.%nExit Setup?
WizardSelectComponents=Review Install
SelectComponentsDesc=
SelectComponentsLabel2=

[Types]
; Used to disable the three Full, Compact and Custom types
Name: "select"; Description: "QModManager"; Flags: IsCustom;

[Components]
Name: "qmm"; Description: "QModManager"; Flags: fixed; Types: select;
Name: "qmm\bz"; Description: "Install for Below Zero"; Flags: exclusive fixed;

[Tasks]
Name: "cleanup"; Description: "(Recommended) Clean up after previous Nitrox and QMM installs";

[Code]
// Import stuff from InstallerExtensions.dll
function PathsEqual(pathone, pathtwo: WideString): Boolean; external 'PathsEqual@files:InstallerExtensions.dll stdcall setuponly delayload';

function IsBelowZero(path: String): Boolean;
begin
  if (FileExists(path + '\SubnauticaZero.exe')) and (FileExists(path + '\SubnauticaZero_Data\Managed\Assembly-CSharp.dll')) then
  begin
    Result := true
    Exit
  end
  else
  begin
    Result := false
    Exit
  end
end;
function IsBelowZeroApp(): Boolean;
begin
  Result := IsBelowZero(ExpandConstant('{app}'));
end;

function GetName(def: string): String;
begin
  if (IsBelowZeroApp()) then
  begin
    Result := '{#Name} (Below Zero)'
  end
  else
  begin
    Result := ExpandConstant('{app}')
  end
end;

function GetURL(def: string): String;
begin
  if (IsBelowZeroApp()) then
  begin
    Result := '{#UpdatesURL}/subnauticabelowzero/mods/1'
  end
  else
  begin
    Result := '{#UpdatesURL}'
  end
end;

function CurPageChanged_SelectComponents(CurPageID: Integer): Boolean;
var
  Index: Integer;
  app: String;
begin
  if CurPageID = wpSelectComponents then
  begin
    try
      app := ExpandConstant('{app}')
    except
      app := 'null'
    end;
    if not IsBelowZero(app) then
    begin
      WizardForm.SelectComponentsLabel.Caption := 'No game detected in this folder, cannot install'
      Exit
    end;
    Index := WizardForm.ComponentsList.Items.IndexOf('Install for Below Zero')
    if Index <> -1 then
    begin
      if IsBelowZero(app) then
      begin
        WizardForm.ComponentsList.Checked[Index] := true
        WizardForm.SelectComponentsLabel.Caption := 'Install QModManager for Below Zero'
      end
    end
  end
end;

function GetDir(folder: String; name: String): String;
var
I : Integer;
P : Integer;
steamInstallPath : String;
configFile : String;
fileLines: TArrayOfString;
begin
  steamInstallPath := ''
  RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\WOW6432Node\Valve\Steam', 'InstallPath', steamInstallPath)
  if (FileExists(steamInstallPath + '\steamapps\common\' + folder + '\' + name + '.exe')) and (FileExists(steamInstallPath + '\steamapps\common\' + folder + '\' + name + '_Data\Managed\Assembly-CSharp.dll')) then
  begin
    Result := steamInstallPath + '\steamapps\common\' + folder
    Exit
  end
  else
  begin
    configFile := steamInstallPath + '\config\config.vdf' 
    if FileExists(configFile) then
    begin
      if LoadStringsFromFile(configFile, FileLines) then 
      begin
        for I := 0 to GetArrayLength(FileLines) - 1 do
        begin
          P := Pos('BaseInstallFolder_', FileLines[I])
          if P > 0 then
          begin
            steamInstallPath := Copy(FileLines[I], P + 23, 3) + Copy(FileLines[I], P + 27, Length(FileLines[I]) - P  - 27);
            if (FileExists(steamInstallPath + '\steamapps\common\' + folder + '\' + name + '.exe')) and (FileExists(steamInstallPath + '\steamapps\common\' + folder + '\' + name + '_Data\Managed\Assembly-CSharp.dll')) then // If the folder is correct
            begin
              Result := steamInstallPath + '\steamapps\common\' + folder
              Exit
            end
          end
        end
      end
    end
  end;
  Result := 'none'
  Exit
end;

var ACLabel: TLabel;
var BelowZeroButton: TNewRadioButton;

procedure BelowZeroButtonOnClick(Sender: TObject);
begin
  WizardForm.DirEdit.Text := GetDir('SubnauticaZero', 'SubnauticaZero')
  BelowZeroButton.Checked := true
end;

function InitializeWizard_AddButtons(): Boolean;
begin
  ACLabel := TLabel.Create(WizardForm)
  with ACLabel do
  begin
    Parent := WizardForm
    Caption := 'Get path from Steam for:'
    Left := WizardForm.SelectDirLabel.Left / 3
    Top := WizardForm.BackButton.Top - WizardForm.BackButton.Top / 90
  end;

  BelowZeroButton := TNewRadioButton.Create(WizardForm)
  with BelowZeroButton do
  begin
    Parent := WizardForm
    Caption := 'Below Zero'
    OnClick := @BelowZeroButtonOnClick  
    Left := WizardForm.SelectDirLabel.Left + WizardForm.SelectDirLabel.Left / 30
    Top := WizardForm.BackButton.Top + 10
    Height := WizardForm.BackButton.Height
    Enabled := True
  end;
end;

function CurPageChanged_AddButtons(CurPageID: Integer): Boolean;
begin
  if CurPageID = wpSelectDir then
  begin
    WizardForm.DirEdit.Text := ''
    if GetDir('SubnauticaZero', 'SubnauticaZero') = 'none' then
    begin
      BelowZeroButton.Enabled := false
    end;
    
    if BelowZeroButton.Enabled then
    begin
      WizardForm.DirEdit.Text := GetDir('SubnauticaZero', 'SubnauticaZero')
      BelowZeroButton.Checked := true
    end;
  end;
  BelowZeroButton.Visible := CurPageID = wpSelectDir
  ACLabel.Visible := CurPageID = wpSelectDir
end;

var DirEditOnChangePrev: TNotifyEvent;

procedure DirEditOnChange(Sender: TObject);
var
  S: String;
begin
  if Pos('subnauticazero', LowerCase(WizardForm.DirEdit.Text)) <> 0 then
    begin
      if PathsEqual(WizardForm.DirEdit.Text, GetDir('SubnauticaZero', 'SubnauticaZero')) then
      begin
        BelowZeroButton.Checked := true
      end
      else
      begin
        BelowZeroButton.Checked := false;
      end
    end
  else
    begin
      BelowZeroButton.Checked := false;
    end;
  
  if (Pos('://', WizardForm.DirEdit.Text) <> 0) or (Pos(':\\', WizardForm.DirEdit.Text) <> 0) then
  begin
    S := WizardForm.DirEdit.Text;
    StringChangeEx(S, '://', ':/', true);
    StringChangeEx(S, ':\\', ':\', true);
    WizardForm.DirEdit.Text := S;
  end
end;

function InitializeWizard_DirOnChange(): Boolean;
begin
  DirEditOnChangePrev := WizardForm.DirEdit.OnChange
  WizardForm.DirEdit.OnChange := @DirEditOnChange
end;

var appIsSet: Boolean;

function GetGUID(def: String): String;
begin
  if not appIsSet then // The installer tries to get the GUID at startup to use previous options such as install path or install settings. As QModManager's GUID is defined AFTER the path is selected, it doesn't need to provide a value
  begin
    Result := ''
    Exit
  end;
  if IsBelowZero(ExpandConstant('{app}')) then
  begin
    Result := '{A535470D-3403-46A2-8D44-28AD4B90C9A3}'
    Exit
  end
end;

function IsAppRunning(const FileName : string): Boolean;
var
  FSWbemLocator: Variant;
  FWMIService   : Variant;
  FWbemObjectSet: Variant;
begin
  Result := false;
  FSWbemLocator := CreateOleObject('WBEMScripting.SWBEMLocator');
  FWMIService := FSWbemLocator.ConnectServer('', 'root\CIMV2', '', '');
  FWbemObjectSet :=
    FWMIService.ExecQuery(
      Format('SELECT Name FROM Win32_Process Where Name="%s"', [FileName]));
  Result := (FWbemObjectSet.Count > 0);
  FWbemObjectSet := Unassigned;
  FWMIService := Unassigned;
  FSWbemLocator := Unassigned;
end;

// Imports some stuff from VclStylesInno.dll
procedure LoadVCLStyle(VClStyleFile: String); external 'LoadVCLStyleW@files:VclStylesInno.dll stdcall';
procedure UnLoadVCLStyles; external 'UnLoadVCLStyles@files:VclStylesInno.dll stdcall';

// Check for .NET version -- code from http://www.kynosarges.de/DotNetVersion.html
function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1'          .NET Framework 1.1
//    'v2.0'          .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//    'v4.5'          .NET Framework 4.5
//    'v4.5.1'        .NET Framework 4.5.1
//    'v4.5.2'        .NET Framework 4.5.2
//    'v4.6'          .NET Framework 4.6
//    'v4.6.1'        .NET Framework 4.6.1
//    'v4.6.2'        .NET Framework 4.6.2
//    'v4.7'          .NET Framework 4.7
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key, versionKey: string;
    install, release, serviceCount, versionRelease: cardinal;
    success: boolean;
begin
    versionKey := version;
    versionRelease := 0;

    // .NET 1.1 and 2.0 embed release number in version key
    if version = 'v1.1' then begin
        versionKey := 'v1.1.4322';
    end else if version = 'v2.0' then begin
        versionKey := 'v2.0.50727';
    end

    // .NET 4.5 and newer install as update to .NET 4.0 Full
    else if Pos('v4.', version) = 1 then begin
        versionKey := 'v4\Full';
        case version of
          'v4.5':   versionRelease := 378389;
          'v4.5.1': versionRelease := 378675; // 378758 on Windows 8 and older
          'v4.5.2': versionRelease := 379893;
          'v4.6':   versionRelease := 393295; // 393297 on Windows 8.1 and older
          'v4.6.1': versionRelease := 394254; // 394271 before Win10 November Update
          'v4.6.2': versionRelease := 394802; // 394806 before Win10 Anniversary Update
          'v4.7':   versionRelease := 460798; // 460805 before Win10 Creators Update
        end;
    end;

    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + versionKey;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0 and newer use value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;

    // .NET 4.5 and newer use additional value Release
    if versionRelease > 0 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
        success := success and (release >= versionRelease);
    end;

    result := success and (install = 1) and (serviceCount >= service);
end;

function InitializeSetup(): Boolean;
var
  ErrCode: Integer;
begin
  if not IsDotNetDetected('v4\Full', 0) then 
  begin
    if MsgBox('QModManager requires Microsoft .NET Framework 4.0' + #13#10 + 'Would you like to install it now?', mbCriticalError, MB_YESNO) = IDYES then
    begin
      if not ShellExec('open', 'https://dotnet.microsoft.com/download/dotnet-framework/net40', '', '', SW_SHOW, ewNoWait, ErrCode) then
      begin
        SysErrorMessage(ErrCode);
      end
    end;
    result := false;
    Exit
  end;
  appIsSet := false
  if IsAppRunning('SubnauticaZero.exe') then
  begin
    MsgBox('You need to close Below Zero before installing QModManager.' + #13#10 + 'If the game is not running, please reboot your computer.', mbError, MB_OK);
    Result := false
  end
  else
  begin
    // Load skin
    ExtractTemporaryFile('Carbon.vsf');
    LoadVCLStyle(ExpandConstant('{tmp}\Carbon.vsf'));
    Result := true
  end
end;    

function IsPreviousVersionInstalled: Boolean;
var
  uninstallRegKey: String;
  previousVersion: String;
begin
  uninstallRegKey := 'Software\Microsoft\Windows\CurrentVersion\Uninstall\' + GetGuid('') + '_is1';
  previousVersion := '';
  Result := (RegKeyExists(HKLM, uninstallRegKey) or RegKeyExists(HKCU, uninstallRegKey));
end;

function GetUninstallString: string;
var
  uninstallRegKey: String;
  uninstallString: String;
begin
  Result := '';
  uninstallRegKey := 'Software\Microsoft\Windows\CurrentVersion\Uninstall\' + GetGuid('') + '_is1';
  uninstallString := '';
  if not RegQueryStringValue(HKLM, uninstallRegKey, 'UninstallString', uninstallString) then
    RegQueryStringValue(HKCU, uninstallRegKey, 'UninstallString', uninstallString);
  Result := uninstallString;
end;

function IsUpgrade: Boolean;
begin
  Result := (GetUninstallString() <> '');
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var 
  uninstallString: String;
  resultCode: Integer;
begin
  if CurPageID = wpSelectComponents then
    appIsSet := true;
  
  Result := true;
end;

function PrepareToInstall(var NeedsRestart: boolean): string;
var
  uninstallString: string;
  resultCode: integer;
begin
  NeedsRestart := false;

  if IsPreviousVersionInstalled() then
  begin
    uninstallString := RemoveQuotes(GetUninstallString());
    if FileExists(uninstallString) then
    begin
      Exec(uninstallString, '/SILENT', '', SW_SHOW, ewWaitUntilTerminated, resultCode);
      if IsPreviousVersionInstalled() then
        Result := 'Previous installation must be uninstalled to continue.';
    end;
  end;
end;

var TypesComboOnChangePrev: TNotifyEvent;

procedure ComponentsListCheckChanges;
begin
  WizardForm.NextButton.Enabled := (WizardSelectedComponents(false) <> '')
end;

procedure ComponentsListClickCheck(Sender: TObject);
begin
  ComponentsListCheckChanges
end;

procedure TypesComboOnChange(Sender: TObject);
begin
  TypesComboOnChangePrev(Sender)
  ComponentsListCheckChanges
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  CurPageChanged_SelectComponents(CurPageID)
  CurPageChanged_AddButtons(CurPageID)
  if CurPageID = wpSelectComponents then
  begin
    ComponentsListCheckChanges;
  end
end;

procedure InitializeWizard();
begin
  WizardForm.ComponentsList.OnClickCheck := @ComponentsListClickCheck
  TypesComboOnChangePrev := WizardForm.TypesCombo.OnChange
  WizardForm.TypesCombo.OnChange := @TypesComboOnChange
  InitializeWizard_AddButtons
  InitializeWizard_DirOnChange
end;

procedure UnloadInstallerExtensions();
  var
  FilePath: string;
  BatchPath: string;
  S: TArrayOfString;
  ResultCode: Integer;
begin
  FilePath := ExpandConstant('{tmp}\InstallerExtensions.dll');
  if not FileExists(FilePath) then
  begin
    Log(Format('File %s does not exist', [FilePath]));
  end
    else
  begin
    BatchPath :=
      ExpandConstant('{%TEMP}\') +
      'delete_' + ExtractFileName(ExpandConstant('{tmp}')) + '.bat';
    SetArrayLength(S, 7);
    S[0] := ':loop';
    S[1] := 'del "' + FilePath + '"';
    S[2] := 'if not exist "' + FilePath + '" goto end';
    S[3] := 'goto loop';
    S[4] := ':end';
    S[5] := 'rd "' + ExpandConstant('{tmp}') + '"';
    S[6] := 'del "' + BatchPath + '"';
    if not SaveStringsToFile(BatchPath, S, False) then
    begin
      Log(Format('Error creating batch file %s to delete %s', [BatchPath, FilePath]));
    end
      else
    if not Exec(BatchPath, '', '', SW_HIDE, ewNoWait, ResultCode) then
    begin
      Log(Format('Error executing batch file %s to delete %s', [BatchPath, FilePath]));
    end
      else
    begin
      Log(Format('Executed batch file %s to delete %s', [BatchPath, FilePath]));
    end;
  end;
end;

procedure DeinitializeSetup();
begin
  // Unload skin
  UnLoadVCLStyles;
  UnloadInstallerExtensions;
end;
