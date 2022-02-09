; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Upsilon.Tools.ReleaseManagementTool.GUI"
#define MyAppVersion "1.2.1"
#define MyAppPublisher "UpsilonEcosystem"
#define MyAppURL "https://www.linkedin.com/in/yassin-lokhat"
#define MyAppExeName MyAppName + ".exe"
#define MyAppExeDirectory ".\Dotfuscated"
#define Updater "Upsilon.Tools.Updater"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{762BD67C-5B11-4AC5-82E7-9A047EBBB62F}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
; Remove the following line to run in administrative install mode (install for all users.)
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputDir={#MyAppExeDirectory}
OutputBaseFilename={#MyAppName}_setup_v{#MyAppVersion}
SetupIconFile=..\UpsilonLogo.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#MyAppExeDirectory}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppExeDirectory}\{#MyAppName}.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppExeDirectory}\{#MyAppName}.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppExeDirectory}\Upsilon.Common.Forms.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppExeDirectory}\Upsilon.Common.Library.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppExeDirectory}\Upsilon.Common.MetaHelper.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppExeDirectory}\Upsilon.Database.Library.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppExeDirectory}\Upsilon.Tools.ReleaseManagementTool.Core.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppExeDirectory}\data\*"; DestDir: "{app}\data"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#MyAppExeDirectory}\servers"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#Updater}\{#Updater}.exe"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#Updater}\{#Updater}.dll"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#Updater}\{#Updater}.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

