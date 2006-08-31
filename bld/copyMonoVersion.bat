mkdir ..\output\debugMono
cd ..\output\debugMono
del /F /S /Q *.*
copy ..\debug\*.* .
del /Q PlatformSpecific.*
ren MonoSpecific.dll PlatformSpecific.dll
ren MonoSpecific.pdb PlatformSpecific.pdb
