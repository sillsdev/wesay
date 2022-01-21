#!/bin/bash
cd "$(dirname "$0")/.."
root=$PWD
. environ
cd  build
msbuild "/target:${2:-Clean;Compile}" /p:Configuration="${1:-Debug}" /p:RootDir=$root  /p:BUILD_NUMBER="${3:-0.0.1.abcd}" build.mono.proj
