#!/bin/bash
cd "$(dirname "$0")"/..
. environ
cd  build
xbuild "/target:Clean;Compile" /p:Configuration="${1:-Release}" /p:RootDir=..  /p:BUILD_NUMBER="0.0.1.abcd" build.mono.proj
