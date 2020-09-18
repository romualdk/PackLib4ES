"C:\Program Files (x86)\WiX Toolset v3.14\bin\heat.exe" dir "..\..\Data\Documents" -dr Documents -cg DocumentsGroup -gg -g1 -sf -srd -var "var.DocumentsDir" -scom -sreg -sw5151 -out ".\GroupDocuments.wxs"

@echo off
setlocal enabledelayedexpansion
ren GroupDocuments.wxs in.tmp
set line=0
for /f "tokens=*" %%a in (in.tmp) do (
  set /a line+=1
  if "!line!"=="2" (
	Echo|set /p="<?include Config.wxi?>">> GroupDocuments.wxs
	Echo. >> GroupDocuments.wxs
	)
  Echo %%a >> GroupDocuments.wxs
)
del in.tmp
goto :eof