"C:\Program Files (x86)\WiX Toolset v3.14\bin\heat.exe" dir "..\..\Data\Thumbnails" -dr Thumbnails -cg ThumbnailsGroup -gg -g1 -sf -srd -var "var.ThumbnailsDir" -out ".\GroupThumbnails.wxs"


@echo off
setlocal enabledelayedexpansion
ren GroupThumbnails.wxs in.tmp
set line=0
for /f "tokens=*" %%a in (in.tmp) do (
  set /a line+=1
  if "!line!"=="2" (
	Echo|set /p="<?include Config.wxi?>">> GroupThumbnails.wxs
	Echo. >> GroupThumbnails.wxs
	)
  Echo %%a >> GroupThumbnails.wxs
)
del in.tmp
goto :eof