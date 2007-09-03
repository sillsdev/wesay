PoMaker.exe .. > ..\common\WeSay.pot

#just for testing
PoMaker.exe .. -en > ..\common\en.po

#This will update all the exiting ones with the new strings
for %%I in (..\common\*.po) do gettextUtils\msgmerge.exe -U "%%~I" ..\common\WeSay.pot
