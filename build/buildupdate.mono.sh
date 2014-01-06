#!/bin/bash
# server=build.palaso.org
# project=WeSay1.4-Linux
# build=WeSay1.4-Precise64 Continuous

#### Results ####
# build: WeSay1.4-Precise64 Continuous (bt314)
# project: WeSay1.4-Linux
# URL: http://build.palaso.org/viewType.html?buildTypeId=bt314
# VCS:  []
# dependencies:
# [0] build: chorus-precise64-master Continuous (bt323)
#     project: Chorus
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt323
#     VCS:  []
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"Autofac.dll"=>"lib/common"}
# [1] build: chorus-precise64-master Continuous (bt323)
#     project: Chorus
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt323
#     VCS:  []
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"Chorus.exe"=>"lib/Release", "LibChorus.dll"=>"lib/Release", "ChorusMerge.exe"=>"lib/Release", "LibChorus.TestUtilities.dll"=>"lib/Release", "Mercurial-i686.zip"=>"lib/Release"}
# [2] build: L10NSharp Mono continuous (bt271)
#     project: L10NSharp
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt271
#     VCS: https://bitbucket.org/hatton/l10nsharp [default]
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"L10NSharp.dll"=>"lib/common"}
# [3] build: icucil-precise64-Continuous (bt281)
#     project: Libraries
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt281
#     VCS:  []
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"icu*.dll"=>"lib/Release", "icu*.config"=>"lib/Release"}
# [4] build: palaso-precise64-master Continuous (bt322)
#     project: libpalaso
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt322
#     VCS:  []
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"Palaso.BuildTasks.dll"=>"lib/Release", "Palaso.dll"=>"lib/Release", "Palaso.TestUtilities.dll"=>"lib/Release", "Palaso.Tests.dll"=>"lib/Release", "Palaso.DictionaryServices.dll"=>"lib/Release", "Palaso.Media.dll"=>"lib/Release", "PalasoUIWindowsForms.dll"=>"lib/Release", "Palaso.Lift.dll"=>"lib/Release"}

# make sure output directories exist
mkdir -p ./lib/common
mkdir -p ./lib/Release

# download artifact dependencies
curl -L -o ./lib/common/Autofac.dll http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/Autofac.dll
curl -L -o ./lib/Release/Chorus.exe http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/Chorus.exe
curl -L -o ./lib/Release/LibChorus.dll http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/LibChorus.dll
curl -L -o ./lib/Release/ChorusMerge.exe http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/ChorusMerge.exe
curl -L -o ./lib/Release/LibChorus.TestUtilities.dll http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/LibChorus.TestUtilities.dll
curl -L -o ./lib/Release/Mercurial-i686.zip http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/Mercurial-i686.zip
curl -L -o ./lib/common/L10NSharp.dll http://build.palaso.org/guestAuth/repository/download/bt271/latest.lastSuccessful/L10NSharp.dll
curl -L -o ./lib/Release/icu.net.dll http://build.palaso.org/guestAuth/repository/download/bt281/latest.lastSuccessful/icu.net.dll
curl -L -o ./lib/Release/icu.net.dll.config http://build.palaso.org/guestAuth/repository/download/bt281/latest.lastSuccessful/icu.net.dll.config
curl -L -o ./lib/Release/Palaso.BuildTasks.dll http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.BuildTasks.dll
curl -L -o ./lib/Release/Palaso.dll http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.dll
curl -L -o ./lib/Release/Palaso.TestUtilities.dll http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.TestUtilities.dll
curl -L -o ./lib/Release/Palaso.Tests.dll http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.Tests.dll
curl -L -o ./lib/Release/Palaso.DictionaryServices.dll http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.DictionaryServices.dll
curl -L -o ./lib/Release/Palaso.Media.dll http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.Media.dll
curl -L -o ./lib/Release/PalasoUIWindowsForms.dll http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/PalasoUIWindowsForms.dll
curl -L -o ./lib/Release/Palaso.Lift.dll http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.Lift.dll
