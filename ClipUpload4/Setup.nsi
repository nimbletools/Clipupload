!include "MUI2.nsh"

Name "Nimble Clipupload"
#OutFile "ClipuploadSetup.exe"

InstallDir "$PROGRAMFILES\Nimble Tools\Clipupload"
InstallDirRegKey HKCU "Software\Clipupload" ""

RequestExecutionLevel admin

!define MUI_ABORTWARNING

!insertmacro MUI_PAGE_LICENSE "readme.txt"
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "English"

Section "Clipupload" SecProgram
	SectionIn RO
	SetOutPath "$INSTDIR"
	AccessControl::GrantOnFile "$INSTDIR" "(S-1-5-32-545)" "FullAccess"
	File AddonHelper.dll
	File ClipUploadShellExtension.dll
	File Clipupload.exe
	File changelog.txt
	File settings.txt.clean
	File srm.exe
	WriteRegStr HKCU "Software\Clipupload" "" $INSTDIR
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Clipupload" "DisplayName" "Clipupload"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Clipupload" "UninstallString" '"$INSTDIR\Uninstall.exe"'
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Clipupload" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Clipupload" "NoRepair" 1
	WriteUninstaller "$INSTDIR\Uninstall.exe"
	CreateShortCut "$SMPROGRAMS\Nimble Clipupload.lnk" "$INSTDIR\Clipupload.exe"
	
	ClearErrors
	ReadRegDWORD $0 HKLM "Software\Microsoft\Net Framework Setup\NDP\v4\Full" "Install"
	IfErrors dotNet40NotFound
	IntCmp $0 1 dotNet40Found
	dotNet40NotFound:
		DetailPrint "Installing .NET Framework 4.0"
		SetOutPath $TEMP
		File /nonfatal "dotnetfx40.exe"
		ExecWait "$TEMP\dotnetfx40.exe /passive /norestart"
		Delete "$TEMP\dotnetfx40.exe"
	dotNet40Found:
	
	SetOutPath "$INSTDIR"
	DetailPrint "Registering shell extension"
	ExecWait "$INSTDIR\srm.exe install $\"$INSTDIR\ClipUploadShellExtension.dll$\" -codebase"
SectionEnd

SectionGroup /e "Cloud hosting" SecCloudHosting
	Section "Dropbox" SecAddonsDropbox
		SetOutPath "$INSTDIR\Addons\Dropbox"
		File Addons\Dropbox\Dropbox.dll
		File Addons\Dropbox\Icon.ico
		File Addons\Dropbox\settings.txt.clean
	SectionEnd
	
	Section "Facebook" SecAddonsFacebook
		SetOutPath "$INSTDIR\Addons\Facebook"
		File Addons\Facebook\Facebook.dll
		File Addons\Facebook\Icon.ico
		File Addons\Facebook\settings.txt.clean
	SectionEnd
	
	Section "Imgur" SecAddonsImgur
		SetOutPath "$INSTDIR\Addons\Imgur"
		File Addons\Imgur\Imgur.dll
		File Addons\Imgur\Icon.ico
		File Addons\Imgur\settings.txt.clean
	SectionEnd
	
	Section "Pastebin" SecAddonsPastebin
		SetOutPath "$INSTDIR\Addons\Pastebin"
		File Addons\Pastebin\Pastebin.dll
		File Addons\Pastebin\Icon.ico
		File Addons\Pastebin\settings.txt.clean
	SectionEnd
	
	Section "Zippyshare" SecAddonsZippyshare
		SetOutPath "$INSTDIR\Addons\Zippyshare"
		File Addons\Zippyshare\Zippyshare.dll
		File Addons\Zippyshare\Icon.ico
		File Addons\Zippyshare\settings.txt.clean
	SectionEnd
SectionGroupEnd

SectionGroup /e "Private hosting" SecPrivateHosting
	Section "FTP" SecAddonsFTP
		SetOutPath "$INSTDIR\Addons\FTP"
		File Addons\FTP\FTP.dll
		File Addons\FTP\Icon.ico
		File Addons\FTP\settings.txt.clean
	SectionEnd
	
	Section /o "Post over http" SecAddonsPostHttp
		SetOutPath "$INSTDIR\Addons\PostHttp"
		File Addons\PostHttp\PostHttp.dll
		File Addons\PostHttp\Icon.ico
		File Addons\PostHttp\settings.txt.clean
	SectionEnd
	
	Section /o "Self hoster (webserver)" SecAddonsSelfHoster
		SetOutPath "$INSTDIR\Addons\SelfHoster"
		File Addons\SelfHoster\SelfHoster.dll
		File Addons\SelfHoster\Icon.ico
		File Addons\SelfHoster\settings.txt.clean
	SectionEnd
	
	Section /o "SFTP (file transfer over SSH)" SecAddonsSFTP
		SetOutPath "$INSTDIR\Addons\SFTP"
		File Addons\SFTP\SFTP.dll
		File Addons\SFTP\Renci.SshNet.dll
		File Addons\SFTP\Icon.ico
		File Addons\SFTP\settings.txt.clean
	SectionEnd
SectionGroupEnd

SectionGroup /e "Local" SecLocal
	Section "Desktop" SecAddonsDesktop
		SetOutPath "$INSTDIR\Addons\Desktop"
		File Addons\Desktop\Desktop.dll
		File Addons\Desktop\Icon.ico
		File Addons\Desktop\settings.txt.clean
	SectionEnd
SectionGroupEnd

SectionGroup /e "Third-party" SecThirdParty
	Section /o "ClipBoard" SecAddonClipBoard
		SetOutPath "$INSTDIR\Addons\ClipBoard"
		File Addons\ClipBoard\ClipBoard.dll
		File Addons\ClipBoard\Icon.ico
		File Addons\ClipBoard\settings.txt.clean
	SectionEnd
SectionGroupEnd

Section "Uninstall"
	ExecWait "$INSTDIR\srm.exe uninstall $\"$INSTDIR\ClipUploadShellExtension.dll$\""
	Delete "$INSTDIR\Uninstall.exe"
	RMDir /r "$INSTDIR"
	Delete "$SMPROGRAMS\Nimble Clipupload.lnk"
	DeleteRegKey /ifempty HKCU "Software\Clipupload"
SectionEnd

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${SecProgram} "The Clipupload program."
	
	!insertmacro MUI_DESCRIPTION_TEXT ${SecCloudHosting} "Addons for uploading to cloud-based public services."
	!insertmacro MUI_DESCRIPTION_TEXT ${SecAddonsDropbox} "Upload to your public Dropbox folder."
	!insertmacro MUI_DESCRIPTION_TEXT ${SecAddonsFacebook} "Upload screenshots and images to your Facebook profile."
	!insertmacro MUI_DESCRIPTION_TEXT ${SecAddonsImgur} "Upload screenshots and images to Imgur, either anonymously or authenticated to your account."
	!insertmacro MUI_DESCRIPTION_TEXT ${SecAddonsPastebin} "Upload text to Pastebin, either anonymously or authenticated to your account."
	!insertmacro MUI_DESCRIPTION_TEXT ${SecAddonsZippyshare} "Upload files to Zippyshare."
	
	!insertmacro MUI_DESCRIPTION_TEXT ${SecPrivateHosting} "Addons for uploading to private locations."
	!insertmacro MUI_DESCRIPTION_TEXT ${SecAddonsFTP} "Upload to an FTP server."
	!insertmacro MUI_DESCRIPTION_TEXT ${SecAddonsPostHttp} "Upload to any endpoint on any HTTP server via POST requests."
	!insertmacro MUI_DESCRIPTION_TEXT ${SecAddonsSelfHoster} "Host all your files yourself and have others connect to your the local webserver that this addon hosts."
	!insertmacro MUI_DESCRIPTION_TEXT ${SecAddonsSFTP} "Upload via SFTP to an SSH server."
	
	!insertmacro MUI_DESCRIPTION_TEXT ${SecLocal} "Addons for the local filesystem"
	!insertmacro MUI_DESCRIPTION_TEXT ${SecAddonsDesktop} "Save as files to the desktop."
	
	!insertmacro MUI_DESCRIPTION_TEXT ${SecThirdParty} "Addons not made by the official developer."
	!insertmacro MUI_DESCRIPTION_TEXT ${SecAddonClipBoard} "Take screenshots and put them directly on the clipboard. Created by Jed."
!insertmacro MUI_FUNCTION_DESCRIPTION_END
