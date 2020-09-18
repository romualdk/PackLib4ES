del /q "..\..\PicParam\bin\Release\*.pdb"
del /q "..\..\PicParam\bin\Release\*.dll.config"
del /q "..\..\PicParam\bin\Release\*.xml"
del /q "..\..\PicParam\bin\Release\*.resx"

rmdir /s /q "..\..\PicParam\bin\Release\de"
rmdir /s /q "..\..\PicParam\bin\Release\es"
rmdir /s /q "..\..\PicParam\bin\Release\fr"
rmdir /s /q "..\..\PicParam\bin\Release\it"
rmdir /s /q "..\..\PicParam\bin\Release\ja"
rmdir /s /q "..\..\PicParam\bin\Release\pl"
rmdir /s /q "..\..\PicParam\bin\Release\pt"
rmdir /s /q "..\..\PicParam\bin\Release\sv"
rmdir /s /q "..\..\PicParam\bin\Release\zh"

"C:\Program Files (x86)\WiX Toolset v3.14\bin\heat.exe" dir "..\..\PicParam\bin\Release" -dr Bin -cg PicParamGroup -gg -g1 -sf -srd -sreg -var "var.PicParamBinDir" -o ".\GroupPicParam.wxs"

@echo off
setlocal enabledelayedexpansion
ren GroupPicParam.wxs in.tmp
set line=0
for /f "tokens=*" %%a in (in.tmp) do (
  set /a line+=1
  if "!line!"=="2" (
	Echo|set /p="<?include Config.wxi?>">> GroupPicParam.wxs
	Echo. >> GroupPicParam.wxs
	)
  Echo %%a >> GroupPicParam.wxs
)
del in.tmp
goto :eof

