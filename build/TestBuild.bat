@echo off
@if "%INCLUDE%" == "" (
	echo "Setting up VS Tools..."
	@call "%VS110COMNTOOLS%vsvars32.bat"
)
@echo on

msbuild "/target:Clean;Compile" /p:Configuration=Release /p:RootDir=..  /p:BUILD_NUMBER="0.0.1.abcd" build.win.proj
