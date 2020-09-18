"C:\Program Files (x86)\WiX Toolset v3.14\bin\heat.exe" dir "..\..\Data\Database" -dr Database -cg DatabaseGroup -gg -g1 -sf -srd -var "var.DatabaseDir" -out ".\GroupDatabase.wxs"

@echo off
setlocal enabledelayedexpansion
ren GroupDatabase.wxs in.tmp
set line=0
for /f "tokens=*" %%a in (in.tmp) do (
  set /a line+=1
  if "!line!"=="2" (
	Echo|set /p="<?include Config.wxi?>">> GroupDatabase.wxs
	Echo. >> GroupPicParam.wxs
	)
  Echo %%a >> GroupDatabase.wxs
)
del in.tmp
goto :eof