#!/bin/bash
# server=build.palaso.org
# project=WeSay1.4-Linux
# build=WeSay1.4-win32 Continuous
# root_dir=..

#### Results ####
# build: WeSay1.4-win32 Continuous (bt312)
# project: WeSay1.4-Linux
# URL: http://build.palaso.org/viewType.html?buildTypeId=bt312
# VCS:  []
# dependencies:
# [0] build: chorus-win32-master Continuous (bt2)
#     project: Chorus
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt2
#     VCS:  []
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"Chorus.exe"=>"lib/Release", "Chorus.pdb"=>"lib/Release", "LibChorus.dll"=>"lib/Release", "LibChorus.pdb"=>"lib/Release", "ChorusMerge.exe"=>"lib/Release", "ChorusMerge.pdb"=>"lib/Release", "LibChorus.TestUtilities.dll"=>"lib/Release", "LibChorus.TestUtilities.pdb"=>"lib/Release", "Mercurial.zip"=>"lib/Release"}
# [1] build: L10NSharp continuous (bt196)
#     project: L10NSharp
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt196
#     VCS: https://bitbucket.org/hatton/l10nsharp []
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"L10NSharp.dll"=>"lib/common", "L10NSharp.pdb"=>"lib/common"}
# [2] build: palaso-win32-master Continuous (bt223)
#     project: libpalaso
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt223
#     VCS:  []
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"Palaso.BuildTasks.dll"=>"lib/Release", "Palaso.dll"=>"lib/Release", "Palaso.pdb"=>"lib/Release", "Palaso.TestUtilities.dll"=>"lib/Release", "Palaso.TestUtilities.pdb"=>"lib/Release", "PalasoUIWindowsForms.dll"=>"lib/Release", "PalasoUIWindowsForms.pdb"=>"lib/Release", "Palaso.Lift.dll"=>"lib/Release", "Palaso.Lift.pdb"=>"lib/Release"}
# [3] build: icucil-win32-default Continuous (bt14)
#     project: Libraries
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt14
#     VCS:  []
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"icu*.dll"=>"lib/Release", "icu*.config"=>"lib/Release"}
# [4] build: chorus-win32-master Continuous (bt2)
#     project: Chorus
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt2
#     VCS:  []
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"Autofac.dll"=>"lib/common"}

# make sure output directories exist
mkdir -p ../lib/Release
mkdir -p ../lib/common

# download artifact dependencies
curl -L -o ../lib/Release/Chorus.exe http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/Chorus.exe
curl -L -o ../lib/Release/Chorus.pdb http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/Chorus.pdb
curl -L -o ../lib/Release/LibChorus.dll http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/LibChorus.dll
curl -L -o ../lib/Release/LibChorus.pdb http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/LibChorus.pdb
curl -L -o ../lib/Release/ChorusMerge.exe http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/ChorusMerge.exe
curl -L -o ../lib/Release/ChorusMerge.pdb http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/ChorusMerge.pdb
curl -L -o ../lib/Release/LibChorus.TestUtilities.dll http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/LibChorus.TestUtilities.dll
curl -L -o ../lib/Release/LibChorus.TestUtilities.pdb http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/LibChorus.TestUtilities.pdb
curl -L -o ../lib/Release/Mercurial.zip http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/Mercurial.zip
curl -L -o ../lib/common/L10NSharp.dll http://build.palaso.org/guestAuth/repository/download/bt196/latest.lastSuccessful/L10NSharp.dll
curl -L -o ../lib/common/L10NSharp.pdb http://build.palaso.org/guestAuth/repository/download/bt196/latest.lastSuccessful/L10NSharp.pdb
curl -L -o ../lib/Release/Palaso.BuildTasks.dll http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.BuildTasks.dll
curl -L -o ../lib/Release/Palaso.dll http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.dll
curl -L -o ../lib/Release/Palaso.pdb http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.pdb
curl -L -o ../lib/Release/Palaso.TestUtilities.dll http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.TestUtilities.dll
curl -L -o ../lib/Release/Palaso.TestUtilities.pdb http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.TestUtilities.pdb
curl -L -o ../lib/Release/PalasoUIWindowsForms.dll http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/PalasoUIWindowsForms.dll
curl -L -o ../lib/Release/PalasoUIWindowsForms.pdb http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/PalasoUIWindowsForms.pdb
curl -L -o ../lib/Release/Palaso.Lift.dll http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.Lift.dll
curl -L -o ../lib/Release/Palaso.Lift.pdb http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.Lift.pdb
curl -L -o ../lib/Release/icu.net.dll http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icu.net.dll
curl -L -o ../lib/Release/icudt40.dll http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icudt40.dll
curl -L -o ../lib/Release/icuin40.dll http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icuin40.dll
curl -L -o ../lib/Release/icuuc40.dll http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icuuc40.dll
curl -L -o ../lib/Release/icu.net.dll.config http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icu.net.dll.config
curl -L -o ../lib/common/Autofac.dll http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/Autofac.dll
