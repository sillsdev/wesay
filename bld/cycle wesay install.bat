cd ..\output\installer\
msiexec /x Previous.msi
msiexec /i SetupWeSay.msi
copy /Y SetupWeSay.msi Previous.msi
pause