; Throws an error if the version used to compile this script is not unicode
; This ensures that the application is built correctly
#if !Defined(UNICODE)
  #error An unicode version of Inno Setup is required to compile this script
#endif

; Defines some variables
#define Name "QModManager" ; The name of the game will be added after it
#define Version "2.0"
#define Author "the QModManager dev team"
#define URL "https://github.com/QModManager/QModManager"
#define SupportURL "https://discord.gg/UpWuWwq"
#define UpdatesURL "https://nexusmods.com" ; The link to the mod will be added after it

; Defines special flags that change the way the installer behaves
#define PreRelease false ; If this is true, a window will appear, promting the user to agree to install the program in this unstable pre-release state
[Setup]
AllowNetworkDrive=no
AllowUNCPath=no
; Makes the install path appear on the Ready to Install page
AlwaysShowDirOnReadyPage=yes
; Fixes an issue with the previous version where 'not found' would appear at the end of the path
AppendDefaultDirName=no
; The GUID of the app
AppId={code:GetGUID}
; The app name
AppName={#Name}
; The authors of the app
AppPublisher={#Author}
; URLs that will appear on the information page of the app in the Add or Remove Programs page
AppPublisherURL={#URL}
AppSupportURL={#SupportURL}
AppUpdatesURL={code:GetURL}
; Display name of the app in the Add or Remove Programs page
AppVerName={#Name} {#Version}
; Sets the version of the app
AppVersion={#Version}
; How the installer compresses the required files
Compression=lzma
; The default directory name
DefaultDirName=.
; Disables directory exists warnings
DirExistsWarning=no
; Forces the choose install path page to appear
DisableDirPage=no
; Disables the start menu group page
DisableProgramGroupPage=yes
; Enables the welcome page
DisableWelcomePage=no
; Enables directory doesn't exist warnings
EnableDirDoesntExistWarning=yes
; Shows information before installing
InfoBeforeFile=Info.txt
; The output file name
OutputBaseFilename=QModManager_Setup
; The output directory
OutputDir=.
; The application might require administrator access
PrivilegesRequired=admin
; Restarts closed applications after install
RestartApplications=yes
; Icon file
SetupIconFile=..\Assets\QModsIcon.ico
; Changes compression, smaller size
SolidCompression=yes
; Uninstall icon file
UninstallDisplayIcon=..\Assets\QModsIcon.ico
; Uninstall app name
UninstallDisplayName={code:GetName}
; Disables the usage of previous settings (when updating) because the GUID is generated too late for them to work
UsePreviousAppDir=no
UsePreviousLanguage=no
; Images that appear in the installer
WizardImageFile=..\Assets\WizardImage.bmp
WizardSmallImageFile=..\Assets\WizardSmallImage.bmp

; Uses default messages when not overriden
[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

; Required files
[Files]
; Subnautica
Source: "0Harmony.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion; Check: IsSubnauticaApp
Source: "0Harmony-1.2.0.1.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion; Check: IsSubnauticaApp
Source: "Mono.Cecil.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion; Check: IsSubnauticaApp
Source: "QModInstaller.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion; Check: IsSubnauticaApp
Source: "QModManager.exe"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion; Check: IsSubnauticaApp
Source: "QModManagerAssets.unity3d"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion; Check: IsSubnauticaApp
; Below Zero
Source: "0Harmony.dll"; DestDir: "{app}\SubnauticaZero_Data\Managed"; Flags: IgnoreVersion; Check: IsBelowZeroApp
Source: "0Harmony-1.2.0.1.dll"; DestDir: "{app}\SubnauticaZero_Data\Managed"; Flags: IgnoreVersion; Check: IsBelowZeroApp
Source: "Mono.Cecil.dll"; DestDir: "{app}\SubnauticaZero_Data\Managed"; Flags: IgnoreVersion; Check: IsBelowZeroApp
Source: "QModInstaller.dll"; DestDir: "{app}\SubnauticaZero_Data\Managed"; Flags: IgnoreVersion; Check: IsBelowZeroApp
Source: "QModManager.exe"; DestDir: "{app}\SubnauticaZero_Data\Managed"; Flags: IgnoreVersion; Check: IsBelowZeroApp
Source: "QModManagerAssets.unity3d"; DestDir: "{app}\SubnauticaZero_Data\Managed"; Flags: IgnoreVersion; Check: IsBelowZeroApp

; On install and uninstall, run executable based on condition
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
; The text that appears in the bottom-left, on the line of the box
BeveledLabel={#Name} {#Version}
; The installer isn't password-protected, but the feature is used for the pre-release warning if the condition is set to true
WizardPassword=Warning
PasswordLabel1=Please read the following important information before continuing.
PasswordLabel3=You are trying to install a pre-release version of QModManager.%nPre-releases are unstable and might contain bugs.%nWe are not responsible for any crashes or world corruptions that might occur.%n%nPlease type 'YES' (without quotes) to continue with the installation.
PasswordEditLabel=Consent:
; The text that appears on the Select install location page
WizardSelectDir=Select install location
SelectDirLabel3=Please select the install folder of the game.
SelectDirBrowseLabel=To continue, click Next. If you would like to select a different folder, click Browse. You can also use the buttons on the bottom left to auto-complete the install path for the chosen game. (Only works for Steam)
; Update checks are enabled by default
ReadyLabel2a=By installing, you agree to allow QModManager to periodically check for updates. You can disable this option at any time in the Mods tab of the Subnautica options menu.
ReadyLabel2b=By installing, you agree to allow QModManager to periodically check for updates. You can disable this option at any time in the Mods tab of the Subnautica options menu.
; The message that appears when the user tries to cancel the install
ExitSetupMessage=Setup is not complete. If you exit now, {#Name} will not be installed.%nExit Setup?
; The installer doesn't use components, but the feature is used for letting the user know what game he is about to install QModManager for, or if the folder doesn't contain any valid games.
WizardSelectComponents=Review Install
SelectComponentsDesc=
SelectComponentsLabel2=

[Types]
; Used to disable the three Full, Compact and Custom types
Name: "select"; Description: "QModManager"; Flags: IsCustom

[Components]
; Adds read-only components that are only used for displaying
Name: "qmm"; Description: "QModManager"; Flags: fixed; Types: select
Name: "qmm\sn"; Description: "Install for Subnautica"; Flags: exclusive fixed
Name: "qmm\bz"; Description: "Install for Below Zero"; Flags: exclusive fixed

[Code]
function IsSubnautica(path: String): Boolean; // Checks if Subnautica is installed in the given folder
begin
  if (FileExists(path + '\Subnautica.exe')) and (FileExists(path + '\Subnautica_Data\Managed\Assembly-CSharp.dll')) then // If Subnautica-specific files exist
  begin
    Result := true // Returns true
    Exit
  end
  else
  begin
    Result := false // Returns false
    Exit
  end
end;
function IsSubnauticaApp(): Boolean;
begin
  Result := IsSubnautica(ExpandConstant('{app}'));
end;

function IsBelowZero(path: String): Boolean; // Checks if Below Zero is installed in the given folder
begin
  if (FileExists(path + '\SubnauticaZero.exe')) and (FileExists(path + '\SubnauticaZero_Data\Managed\Assembly-CSharp.dll')) then // If Subnautica-specific files exist
  begin
    Result := true // Returns true
    Exit
  end
  else
  begin
    Result := false // Returns false
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

function CurPageChanged_SelectComponents(CurPageID: Integer): Boolean; // Executes whenever the page is changed
var
  Index: Integer;
  app: String;
begin
  if CurPageID = wpSelectComponents then // If the page is Select components (aka Review install)
  begin
    try
      app := ExpandConstant('{app}')
    except
      app := 'null'
    end;
    if IsSubnautica(app) and IsBelowZero(app) then // If multiple games detected (This should never happen in theory)
    begin
      WizardForm.SelectComponentsLabel.Caption := 'Multiple games detected in the same folder, cannot install'
      Exit
    end;
    if not IsSubnautica(app) and not IsBelowZero(app) then // If no games are detected
    begin
      WizardForm.SelectComponentsLabel.Caption := 'No game detected in this folder, cannot install'
      Exit
    end;
    Index := WizardForm.ComponentsList.Items.IndexOf('Install for Subnautica') // Gets the index of the component
    if Index <> -1 then // If the component exists (it should)
    begin
      if IsSubnautica(app) then
      begin
        WizardForm.ComponentsList.Checked[Index] := true // Checks it
        WizardForm.SelectComponentsLabel.Caption := 'Install QModManager for Subnautica' // Changes the description
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
  steamInstallPath := 'Steam install location not found in registry' // Sets a dummy value
  RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\WOW6432Node\Valve\Steam', 'InstallPath', steamInstallPath) // Gets the install path of steam from the registry
  if (FileExists(steamInstallPath + '\steamapps\common\' + folder + '\' + name + '.exe')) and (FileExists(steamInstallPath + '\steamapps\common\' + folder + '\' + name + '_Data\Managed\Assembly-CSharp.dll')) then // If game files exist
  begin
    Result := steamInstallPath + '\steamapps\common\' + folder
    Exit
  end
  else // If the game files DON'T exist
  begin
    configFile := steamInstallPath + '\config\config.vdf' // Gets the path to the steam config file
    if FileExists(configFile) then // If the config file exists
    begin
      // Does some very complicated stuff to get other install folders
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
  Result := 'none' // Returns dummy value (before it was an empty string, but that would conflict with other stuff, so I changed it)
  Exit
end;

var ACLabel: TLabel; // "Auto-complete path for:" label
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

function InitializeWizard_AddButtons(): Boolean; // Is called when the wizard gets initialized
begin
  ACLabel := TLabel.Create(WizardForm) // Create
  with ACLabel do // Set properties
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

function CurPageChanged_AddButtons(CurPageID: Integer): Boolean; // Is called whenever the page is changed
begin
  if CurPageID = wpSelectDir then // If the page is select install path
  begin
    WizardForm.DirEdit.Text := '' // Sets the install path to an empty string
    if (GetDir('Subnautica', 'Subnautica') = 'none') and (SubnauticaButton.Enabled = true) then // If Subnautica isn't found
    begin
      SubnauticaButton.Enabled := false
    end;
    if (GetDir('SubnauticaZero', 'SubnauticaZero') = 'none') and (BelowZeroButton.Enabled = true) then // If Below Zero isn't found
    begin
      BelowZeroButton.Enabled := false
    end;
    
    if SubnauticaButton.Enabled and not BelowZeroButton.Enabled then // If only Subnautica is found
    begin
      WizardForm.DirEdit.Text := GetDir('Subnautica', 'Subnautica') // Sets path to Subnautica install location
      SubnauticaButton.Checked := true
    end
    else if BelowZeroButton.Enabled and not SubnauticaButton.Enabled then
    begin
      WizardForm.DirEdit.Text := GetDir('SubnauticaZero', 'SubnauticaZero') // Sets path to Below Zero install location
      BelowZeroButton.Checked := true
    end;
  end;
  SubnauticaButton.Visible := CurPageID = wpSelectDir // Enables or disables the buttons
  BelowZeroButton.Visible := CurPageID = wpSelectDir
  ACLabel.Visible := CurPageID = wpSelectDir
end;

var DirEditOnChangePrev: TNotifyEvent;

procedure DirEditOnChange(Sender: TObject);
begin
  if not Pos('Subnautica', WizardForm.DirEdit.Text) = 0 then
  begin
    if LowerCase(WizardForm.DirEdit.Text) = LowerCase(GetDir('Subnautica', 'Subnautica')) then // If the Subnautica path is typed manually
    begin
      if not SubnauticaButton.Checked then
      begin
        SubnauticaButton.Checked := true // Check the button
      end;
    end
  end;
  if not Pos('SubnauticaZero', WizardForm.DirEdit.Text) = 0 then
  begin
    if LowerCase(WizardForm.DirEdit.Text) = LowerCase(GetDir('SubnauticaZero', 'SubnauticaZero')) then // If the Below Zero path is typed manually
    begin
      if not BelowZeroButton.Checked then
      begin
        BelowZeroButton.Checked := true // Check the button
      end;
    end
  end
  else // If the path doesn't match any of the known ones, disable the buttons
  begin
    if SubnauticaButton.Checked then
    begin
      SubnauticaButton.Checked := false;
    end;
    if BelowZeroButton.Checked then
    begin
      BelowZeroButton.Checked := false;
    end
  end
end;

function InitializeWizard_DirOnChange(): Boolean; // Overrides the DirEdit.OnChange event
begin
  DirEditOnChangePrev := WizardForm.DirEdit.OnChange
  WizardForm.DirEdit.OnChange := @DirEditOnChange
end;

var appIsSet: Boolean; // True if {app} has a value, false otherwise

function GetGUID(def: String): String;
begin
  if not appIsSet then // The installer tries to get the GUID at startup to use previous options such as install path or install settings. As QModManager's GUID is defined AFTER the path is selected, it doesn't need to provide a value
  begin
    Result := ''
    Exit
  end; // The rest is self-explanatory. A different GUID is provided based on selected install location
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

// Detects wheter an app is running or not based on an .exe name
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

// Called when the app launches. If returns false, cancel install
// Same as InitializeWizard
// TODO: Move and split event functions
function InitializeSetup(): Boolean;
begin
  appIsSet := false // Sets a default value
  if IsAppRunning('Subnautica.exe') or IsAppRunning('SubnauticaZero.exe') then
  begin
    MsgBox('You need to close Subnautica and Subnautica: Below Zero before installing QModManager.' + #13#10 + 'If none of these games are running, please reboot your computer.', mbError, MB_OK);
    Result := false
  end
  else
  begin
    Result := true
  end
end;

// Called whenever the Next button is clicked. If returns false, cancel click
// Same as CurPageChanged ONLY IF THE PAGE IS CHANGED BY CLICKING THE BUTTON. If the page is changed thru script, this doesn't get called
function NextButtonClick(CurPageID: Integer): Boolean;
begin
  if CurPageID = wpSelectComponents then // If the path has been selected, it means that the {app} variable is defined
  begin
    appIsSet := true // Set it to true
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
  #if PreRelease == true
    CurPageChanged__(CurPageID)
  #endif
  CurPageChanged_SelectComponents(CurPageID)
  CurPageChanged_AddButtons(CurPageID)
  if CurPageID = wpSelectComponents then
  begin
    ComponentsListCheckChanges
  end
end;

#if PreRelease == true
  var PasswordEditOnChangePrev: TNotifyEvent;
  var LastValue_PreRelease: Boolean;

  procedure CurPageChanged(CurPageID: Integer);
  begin
    if CurPageID = wpPassword then
    begin
      WizardForm.PasswordEdit.Password := false;
      WizardForm.NextButton.Enabled := false;
      LastValue_PreRelease := false;
    end
    else if CurPageID = wpSelectDir then
    begin
      DirEditOnChange(1)
    end
  end;

  procedure PasswordEditOnChange(Sender: TObject);
  begin
    if (LowerCase(WizardForm.PasswordEdit.Text) = 'yes') then
    begin
      WizardForm.NextButton.Enabled := true
      LastValue_PreRelease := true
    end
    else if (LastValue_PreRelease = true) and not (WizardForm.PasswordEdit.Text = '') then
    begin
      WizardForm.NextButton.Enabled := false
      LastValue_PreRelease := false
    end
  end;

  function InitializeWizard_: Boolean;
  begin
    PasswordEditOnChangePrev := WizardForm.PasswordEdit.OnChange
    WizardForm.PasswordEdit.OnChange := @PasswordEditOnChange
  end;

  function CheckPassword(Password: String): Boolean;
  begin
    if LowerCase(Password) = 'yes' then
    begin
      Result := true
    end
  end;
#endif

procedure InitializeWizard();
begin
  WizardForm.ComponentsList.OnClickCheck := @ComponentsListClickCheck
  TypesComboOnChangePrev := WizardForm.TypesCombo.OnChange
  WizardForm.TypesCombo.OnChange := @TypesComboOnChange
  #if PreRelease == true
    InitializeWizard_()
  #endif
  InitializeWizard_AddButtons
  InitializeWizard_DirOnChange
  DirEditOnChangePrev := WizardForm.DirEdit.OnChange
  WizardForm.DirEdit.OnChange := @DirEditOnChange
  #if PreRelease == true
    InitializeWizard_();
  #endif
end;
