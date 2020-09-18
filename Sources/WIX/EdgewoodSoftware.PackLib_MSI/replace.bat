@echo off
set filename=%1
set targetline=%2
set message=%3
set n=0
for /F "tokens=*" %%i in (%filename%) do call :write %%i
exit /b 0

:write
set /a n+=1
if %n%==%targetline% (echo %message%) else echo %*
exit /b 0