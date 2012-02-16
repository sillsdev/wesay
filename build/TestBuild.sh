#!/bin/bash
xbuild /target:Installer /p:Configuration=DebugMono /p:RootDir=/media/mono/wesay  /p:BUILD_NUMBER="0.0.1.abcd" build.mono.proj
