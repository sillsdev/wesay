pushd .

cd ..\chorus
call GetAndBuildThis.bat

copy /Y output\debug\*.exe ..\wesayDev\lib\net2.0
copy /Y output\debug\*.dll ..\wesayDev\lib\net2.0
copy /Y output\debug\*.exe ..\wesayDev\output\debug
copy /Y output\debug\*.dll ..\wesayDev\output\debug
copy /Y output\debug\*.pdb ..\wesayDev\output\debug

pause
popd
pushd .


cd ..\palaso
REM no need, since Chorus already called this on palaso:  call GetAndBuildThis.bat
call "copyto dev wesay.bat"

popd
