#!/bin/bash
xbuild "/target:Clean;Compile" /p:Configuration=Release /p:RootDir=..  /p:BUILD_NUMBER="0.0.1.abcd" build.mono.proj
