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
#define PreRelease false ; If this is true, a window will appear, promting the user to agree to download even if this is a prerelease

[Setup]
AllowNetworkDrive=no
AllowUNCPath=no
; Makes the install path appear on the Ready to Install page
AlwaysShowDirOnReadyPage=yes
; Fixes an issue with the previous version where 'not found' would appear at the end of the path
AppendDefaultDirName=no
; The GUID of the app
AppId={{52CC87AA-645D-40FB-8411-510142191678}
; The app name
AppName={#Name}
; The authors of the app
AppPublisher={#Author}
; URLs that will appear on the information page of the app in the Add or Remove Programs page
AppPublisherURL={#URL}
AppSupportURL={#SupportURL}
AppUpdatesURL={#UpdatesURL}
; Display name of the app in the Add or Remove Programs page
AppVerName={#Name} {#Version}
; Sets the version of the app
AppVersion={#Version}
; How the installer compresses the required files
Compression=lzma
; The default directory name (this is not used, but it needs to have a value)
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
; The output file name
OutputBaseFilename=QModManager_Setup
; The output directory
OutputDir=.
; The application might administrator access
PrivilegesRequired=admin
; Restarts closed applications after install
RestartApplications=yes
; Icon file
SetupIconFile=..\Assets\QModsIcon.ico
; Changes compression, smaller size
SolidCompression=yes
; Uninstall icon file
UninstallDisplayIcon=..\Assets\icon.ico
; Uninstall app name
; TODO: Append game name at the end
UninstallDisplayName={#Name}
; Use previous app directory if possible
UsePreviousAppDir=yes
; Images that appear in the installer
WizardImageFile=..\Assets\WizardImage.bmp
WizardSmallImageFile=..\Assets\WizardSmallImage.bmp

; Uses default messages when not overriden
[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

; Required files
[Files]
Source: "0Harmony.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion
Source: "Mono.Cecil.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion
Source: "QModInstaller.dll"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion
Source: "QModManager.exe"; DestDir: "{app}\Subnautica_Data\Managed"; Flags: IgnoreVersion

; On install and uninstall, run executable based on condition
[Run]
Filename: "{app}\Subnautica_Data\Managed\QModManager.exe"; Parameters: "-i"

[UninstallRun]
Filename: "{app}\Subnautica_Data\Managed\QModManager.exe"; Parameters: "-u"

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
SelectDirBrowseLabel=If you cannot click yes, it is because Subnautica is not installed in that folder.
; The installer doesn't use components, but the feature is used for displaying the install type
WizardSelectComponents=Review Install
SelectComponentsDesc=
SelectComponentsLabel2=
; The message that appears when the user tries to cancel the install
ExitSetupMessage=Setup is not complete. If you exit now, {#Name} will not be installed.%nExit Setup?

[Code]

function IsSubnautica(path: String): Boolean; // Checks if Subnautica is installed in the current folder
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


var DirEditOnChangePrev: TNotifyEvent;

procedure DirEditOnChange(Sender: TObject);
begin
  if not IsSubnautica(WizardForm.DirEdit.Text) then // If subnautica isnt installed in that path, disable the buttons
  begin
    WizardForm.NextButton.Enabled := false
  end
  else // else enable them
  begin
    WizardForm.NextButton.Enabled := true
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
  DirEditOnChangePrev := WizardForm.DirEdit.OnChange
  WizardForm.DirEdit.OnChange := @DirEditOnChange
  #if PreRelease == true
    InitializeWizard_();
  #endif
end;