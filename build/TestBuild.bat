@echo off
if "%INCLUDE%%LIB%" == "" (

	if not "%VS120COMNTOOLS%" == "" (
		echo "Setting up Visual Studio Pro 2013 Tools..."
		@call "%VS120COMNTOOLS%vsvars32.bat"
		goto build
	)

	if not "%VS100COMNTOOLS%" == "" (
		echo "Setting up Visual Studio 2010 Pro Tools..."
		@call "%VS100COMNTOOLS%vsvars32.bat"
		goto build
	)

	if not "%VS110COMNTOOLS%" == "" (
		echo "Setting up Visual Studio Express 2010 Tools..."
		@call "%VS110COMNTOOLS%vsvars32.bat"
		goto build
	)
)

:build
if "%~1" == "" (
	SET BUILD=Debug
) else (
	SET BUILD=%~1
)
if "%~2" == "" (
	SET TARGET=Clean;Compile
) else (
	SET TARGET=%~2
)
if "%~3" == "" (
	SET BUILD_STEP=1
) else (
	SET BUILD_STEP=%~3
)

@echo on

@pushd "%~dp0"

msbuild "/target:%TARGET%" /p:Configuration="%BUILD%" /p:Platform=x86 /p:RootDir=..  /p:BUILD_NUMBER="0.0.%BUILD_STEP%.abcd" build.win.proj

@popd
