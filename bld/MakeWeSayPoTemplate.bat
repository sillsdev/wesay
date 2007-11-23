PoMaker.exe .. > ..\common\WeSay.pot

REM just for testing
PoMaker.exe .. -en > ..\common\en.po

REM This will update all the exiting ones with the new strings
REM I (John) am not clear if this is useful anymore... LaunchPad will do the same
REM thing when we upload a new pot
for %%I in (..\common\*.po) do gettextUtils\msgmerge.exe -U "%%~I" ..\common\WeSay.pot

pause
