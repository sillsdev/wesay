#!/bin/bash

if [ "$1" == "" ]
then
	export BUILD=Release
else
	export BUILD="$1"
fi

xbuild "/target:Clean;Compile" /p:Configuration="${BUILD}" /p:RootDir=..  /p:BUILD_NUMBER="0.0.1.abcd" build.mono.proj
