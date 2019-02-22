; Throws an error if the version used to compile this script is not unicode
; This ensures that the application is built correctly
#if !Defined(UNICODE)
  #error An unicode version of Inno Setup is required to compile this script
#endif

#define Name "QModManager" ; The name of the game will be added after it
#define Version "2.0.1"
#define Author "the QModManager dev team"
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
InfoBeforeFile=Info.txt
OutputBaseFilename=QModManager_Setup
OutputDir=..\Build
PrivilegesRequired=admin
SetupIconFile=QModsIcon.ico
SolidCompression=yes
UninstallDisplayIcon={app}\{code:GetUninstallIcon}
UninstallDisplayName={code:GetName}
UsePreviousAppDir=no
UsePreviousLanguage=no
WizardImageFile=WizardImage.bmp
WizardSmallImageFile=WizardSmallImageTransparent.bmp

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
// Files used by the installer but not required by QModManager itself
; Installer theme
Source: "VclStylesinno.dll"; Flags: DontCopy
Source: "Carbon.vsf"; Flags: DontCopy
; Installer extensions
Source: "InstallerExtensions.dll"; Flags: DontCopy
// Files required by QModManager itself
; Subnautica
Source: "..\Dependencies\0Harmony.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion; Check: IsSubnauticaApp
Source: "..\Dependencies\0Harmony-1.2.0.1.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion; Check: IsSubnauticaApp
Source: "..\Dependencies\AssetsTools.NET.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion; Check: IsSubnauticaApp
Source: "..\Dependencies\cldb.dat"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion; Check: IsSubnauticaApp
Source: "..\Dependencies\Mono.Cecil.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion; Check: IsSubnauticaApp
Source: "..\Build\QModInstaller.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion; Check: IsSubnauticaApp
Source: "..\Build\QModManager.exe"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion; Check: IsSubnauticaApp
Source: "..\Build\QModManagerAssets.unity3d"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion; Check: IsSubnauticaApp
; Below Zero
Source: "..\Dependencies\0Harmony.dll"; DestDir: "{app}\SubnauticaZero_Data\Managed"; Flags: IgnoreVersion; Check: IsBelowZeroApp
Source: "..\Dependencies\0Harmony-1.2.0.1.dll"; DestDir: "{app}\SubnauticaZero_Data\Managed"; Flags: IgnoreVersion; Check: IsBelowZeroApp
Source: "..\Dependencies\AssetsTools.NET.dll"; DestDir: "{app}\SubnauticaZero_Data\Managed"; Flags: IgnoreVersion; Check: IsBelowZeroApp
Source: "..\Dependencies\cldb.dat"; DestDir: "{app}\SubnauticaZero_Data\Managed"; Flags: IgnoreVersion; Check: IsBelowZeroApp
Source: "..\Dependencies\Mono.Cecil.dll"; DestDir: "{app}\SubnauticaZero_Data\Managed"; Flags: IgnoreVersion; Check: IsBelowZeroApp
Source: "..\Build\QModInstaller.dll"; DestDir: "{app}\SubnauticaZero_Data\Managed"; Flags: IgnoreVersion; Check: IsBelowZeroApp
Source: "..\Build\QModManager.exe"; DestDir: "{app}\SubnauticaZero_Data\Managed"; Flags: IgnoreVersion; Check: IsBelowZeroApp
Source: "..\Build\QModManagerAssets.unity3d"; DestDir: "{app}\SubnauticaZero_Data\Managed"; Flags: IgnoreVersion; Check: IsBelowZeroApp

[Run]
; Subnautica
Filename: "{app}\Subnautica_Data\Managed\QModManager.exe"; Parameters: "-i"; Check: IsSubnauticaApp
; Below Zero
Filename: "{app}\SubnauticaZero_Data\Managed\QModManager.exe"; Parameters: "-i"; Check: IsBelowZeroApp

[UninstallRun]
; Subnautica
Filename: "{app}\Subnautica_Data\Managed\QModManager.exe"; Parameters: "-u"; Check: IsSubnauticaApp
; Below Zero
Filename: "{app}\SubnauticaZero_Data\Managed\QModManager.exe"; Parameters: "-u"; Check: IsBelowZeroApp

[Messages]
WizardSelectDir=Select install location
SelectDirLabel3=Please select the install folder of the game.
SelectDirBrowseLabel=To continue, click Next. If you would like to select a different folder, click Browse.%nYou can also use the buttons on the bottom left to auto-complete the install path for the chosen game. (Only works for Steam)
ReadyLabel2a=By installing, you agree to allow QModManager to send external web requests, most often to check for updates. You can disable this option at any time in the Mods tab of the Subnautica options menu.
ReadyLabel2b=By installing, you agree to allow QModManager to send external web requests, most often to check for updates. You can disable this option at any time in the Mods tab of the Subnautica options menu.
ExitSetupMessage=Setup is not complete. If you exit now, {#Name} will not be installed.%nExit Setup?
WizardSelectComponents=Review Install
SelectComponentsDesc=
SelectComponentsLabel2=

[Types]
; Used to disable the three Full, Compact and Custom types
Name: "select"; Description: "QModManager"; Flags: IsCustom

[Components]
Name: "qmm"; Description: "QModManager"; Flags: fixed; Types: select
Name: "qmm\sn"; Description: "Install for Subnautica"; Flags: exclusive fixed
Name: "qmm\bz"; Description: "Install for Below Zero"; Flags: exclusive fixed

[Code]
// Import stuff from InstallerExtensions.dll
function PathsEqual(pathone, pathtwo: WideString): Boolean; external 'PathsEqual@files:InstallerExtensions.dll stdcall';

function IsSubnautica(path: String): Boolean;
begin
  if (FileExists(path + '\Subnautica.exe')) and (FileExists(path + '\Subnautica_Data\Managed\Assembly-CSharp.dll')) then
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
function IsSubnauticaApp(): Boolean;
begin
  Result := IsSubnautica(ExpandConstant('{app}'));
end;

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
  if (IsSubnauticaApp()) then
  begin
    Result := '{#Name} (Subnautica)'
  end
  else if (IsBelowZeroApp()) then
  begin
    Result := '{#Name} (Below zero)'
  end
  else
  begin
    Result := ExpandConstant('{app}')
  end
end;

function GetUninstallIcon(def: string): String;
begin
  if (IsSubnauticaApp()) then
  begin
    Result := 'Subnautica_Data\Managed\QModManager.exe'
  end
  else if (IsBelowZeroApp()) then
  begin
    Result := 'SubnauticaZero_Data\Managed\QModManager.exe'
  end
  else
  begin
    Result := ''
  end
end;

function GetURL(def: string): String;
begin
  if (IsSubnauticaApp()) then
  begin
    Result := '{#UpdatesURL}/subnautica/mods/16'
  end
  else if (IsBelowZeroApp()) then
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
    if IsSubnautica(app) and IsBelowZero(app) then
    begin
      WizardForm.SelectComponentsLabel.Caption := 'Multiple games detected in the same folder, cannot install'
      Exit
    end;
    if not IsSubnautica(app) and not IsBelowZero(app) then
    begin
      WizardForm.SelectComponentsLabel.Caption := 'No game detected in this folder, cannot install'
      Exit
    end;
    Index := WizardForm.ComponentsList.Items.IndexOf('Install for Subnautica')
    if Index <> -1 then
    begin
      if IsSubnautica(app) then
      begin
        WizardForm.ComponentsList.Checked[Index] := true
        WizardForm.SelectComponentsLabel.Caption := 'Install QModManager for Subnautica'
      end
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
temp: Integer;
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
            steamInstallPath := Copy(FileLines[I], P + 23, Length(FileLines[i]) - P - 23)
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
var SubnauticaButton: TNewRadioButton;
var BelowZeroButton: TNewRadioButton;

procedure SubnauticaButtonOnClick(Sender: TObject);
begin
  WizardForm.DirEdit.Text := GetDir('Subnautica', 'Subnautica')
  SubnauticaButton.Checked := true
end;

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
    Caption := 'Auto-complete path for: (Steam)'
    Left := WizardForm.SelectDirLabel.Left / 3
    Top := WizardForm.BackButton.Top - WizardForm.BackButton.Top / 90
  end;

  SubnauticaButton := TNewRadioButton.Create(WizardForm)
  with SubnauticaButton do
  begin
    Parent := WizardForm
    Caption := 'Subnautica'
    OnClick := @SubnauticaButtonOnClick
    Left := WizardForm.SelectDirLabel.Left + WizardForm.SelectDirLabel.Left / 30
    Top := WizardForm.BackButton.Top + 10
    Height := WizardForm.BackButton.Height
  end;
  
  BelowZeroButton := TNewRadioButton.Create(WizardForm)
  with BelowZeroButton do
  begin
    Parent := WizardForm
    Caption := 'Below Zero'
    OnClick := @BelowZeroButtonOnClick
    Left := SubnauticaButton.Left * 3
    Top := WizardForm.BackButton.Top + 10
    Height := WizardForm.BackButton.Height
  end;
end;

function CurPageChanged_AddButtons(CurPageID: Integer): Boolean;
begin
  if CurPageID = wpSelectDir then
  begin
    WizardForm.DirEdit.Text := ''
    if GetDir('Subnautica', 'Subnautica') = 'none' then
    begin
      SubnauticaButton.Enabled := false
    end;
    if GetDir('SubnauticaZero', 'SubnauticaZero') = 'none' then
    begin
      BelowZeroButton.Enabled := false
    end;
    
    if SubnauticaButton.Enabled and not BelowZeroButton.Enabled then
    begin
      WizardForm.DirEdit.Text := GetDir('Subnautica', 'Subnautica')
      SubnauticaButton.Checked := true
    end
    else if BelowZeroButton.Enabled and not SubnauticaButton.Enabled then
    begin
      WizardForm.DirEdit.Text := GetDir('SubnauticaZero', 'SubnauticaZero')
      BelowZeroButton.Checked := true
    end;
  end;
  SubnauticaButton.Visible := CurPageID = wpSelectDir
  BelowZeroButton.Visible := CurPageID = wpSelectDir
  ACLabel.Visible := CurPageID = wpSelectDir
end;

var DirEditOnChangePrev: TNotifyEvent;

procedure DirEditOnChange(Sender: TObject);
var
  S: String;
begin
  if Pos('subnautica', LowerCase(WizardForm.DirEdit.Text)) <> 0 then
  begin
    if PathsEqual(WizardForm.DirEdit.Text, GetDir('Subnautica', 'Subnautica')) then
    begin
      SubnauticaButton.Checked := true
    end
    else
    begin
      SubnauticaButton.Checked := false;
    end
  end
  else
  begin
    SubnauticaButton.Checked := false;
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
    end
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
  if IsSubnautica(ExpandConstant('{app}')) then
  begin
    Result := '{52CC87AA-645D-40FB-8411-510142191678}'
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

function InitializeSetup(): Boolean;
begin
  appIsSet := false
  if IsAppRunning('Subnautica.exe') or IsAppRunning('SubnauticaZero.exe') then
  begin
    MsgBox('You need to close Subnautica and Subnautica: Below Zero before installing QModManager.' + #13#10 + 'If none of these games are running, please reboot your computer.', mbError, MB_OK);
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

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  if CurPageID = wpSelectComponents then
  begin
    appIsSet := true
  end;
  Result := true
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
    ComponentsListCheckChanges
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

procedure DeinitializeSetup();
begin
  // Unload skin
  UnLoadVCLStyles;
end;