#!/bin/bash
# server=build.palaso.org
# project=WeSay1.5-Linux
# build=wesay1.5-precise64-continuous
# root_dir=..
# $Id: 0b75ca980cea444bf053cfdd852cb3e370225ffe $

cd "$(dirname "$0")"

# *** Functions ***
force=0
clean=0

while getopts fc opt; do
case $opt in
f) force=1 ;;
c) clean=1 ;;
esac
done

shift $((OPTIND - 1))

copy_auto() {
if [ "$clean" == "1" ]
then
echo cleaning $2
rm -f ""$2""
else
where_curl=$(type -P curl)
where_wget=$(type -P wget)
if [ "$where_curl" != "" ]
then
copy_curl $1 $2
elif [ "$where_wget" != "" ]
then
copy_wget $1 $2
else
echo "Missing curl or wget"
exit 1
fi
fi
}

copy_curl() {
echo "curl: $2 <= $1"
if [ -e "$2" ] && [ "$force" != "1" ]
then
curl -# -L -z $2 -o $2 $1
else
curl -# -L -o $2 $1
fi
}

copy_wget() {
echo "wget: $2 <= $1"
f=$(basename $2)
d=$(dirname $2)
cd $d
wget -q -L -N $1
cd -
}


# *** Results ***
# build: wesay1.5-precise64-continuous (bt314)
# project: WeSay1.5-Linux
# URL: http://build.palaso.org/viewType.html?buildTypeId=bt314
# VCS: https://github.com/sillsdev/wesay.git [develop]
# dependencies:
# [0] build: Chorus-Documentation (bt216)
#     project: Chorus
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt216
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"Chorus_Help.chm"=>"common"}
#     VCS: https://github.com/sillsdev/chorushelp.git [master]
# [1] build: chorus-precise64-master Continuous (bt323)
#     project: Chorus
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt323
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"Chorus.exe"=>"lib/Release", "Chorus.exe.mdb"=>"lib/Release", "ChorusHub.exe"=>"lib/Release", "ChorusHub.exe.mdb"=>"lib/Release", "ChorusHubApp.exe"=>"lib/Release", "ChorusHubApp.exe.mdb"=>"lib/Release", "ChorusMerge.exe"=>"lib/Release", "ChorusMerge.exe.mdb"=>"lib/Release", "LibChorus.dll"=>"lib/Release", "LibChorus.dll.mdb"=>"lib/Release", "LibChorus.TestUtilities.dll"=>"lib/Release", "LibChorus.TestUtilities.dll.mdb"=>"lib/Release", "Autofac.dll"=>"lib/Release", "NDesk.DBus.dll"=>"lib/Release", "NDesk.DBus.dll.config"=>"lib/Release", "debug/**"=>"lib/Debug", "Mercurial-i686.zip"=>"lib/common", "Mercurial-x86_64.zip"=>"lib/common"}
#     VCS: https://github.com/sillsdev/chorus.git [master]
# [2] build: L10NSharp Mono continuous (bt271)
#     project: L10NSharp
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt271
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"L10NSharp.dll"=>"lib/Release", "L10NSharp.dll.mdb"=>"lib/Release"}
#     VCS: https://bitbucket.org/sillsdev/l10nsharp []
# [3] build: L10NSharp Mono continuous (bt271)
#     project: L10NSharp
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt271
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"L10NSharp.dll"=>"lib/Debug", "L10NSharp.dll.mdb"=>"lib/Debug"}
#     VCS: https://bitbucket.org/sillsdev/l10nsharp []
# [4] build: palaso-precise64-master Continuous (bt322)
#     project: libpalaso
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt322
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"Enchant.Net.dll"=>"lib/Release", "Enchant.Net.dll.config"=>"lib/Release", "ibusdotnet.dll"=>"lib/Release", "Palaso.dll"=>"lib/Release", "Palaso.dll.mdb"=>"lib/Release", "Palaso.dll.config"=>"lib/Release", "Palaso.DictionaryServices.dll"=>"lib/Release", "Palaso.DictionaryServices.dll.mdb"=>"lib/Release", "Palaso.Lift.dll"=>"lib/Release", "Palaso.Lift.dll.mdb"=>"lib/Release", "Palaso.Media.dll"=>"lib/Release", "Palaso.Media.dll.mdb"=>"lib/Release", "Palaso.Media.dll.config"=>"lib/Release", "Palaso.Tests.dll"=>"lib/Release", "Palaso.Tests.dll.mdb"=>"lib/Release", "Palaso.TestUtilities.dll"=>"lib/Release", "Palaso.TestUtilities.dll.mdb"=>"lib/Release", "PalasoUIWindowsForms.dll"=>"lib/Release", "PalasoUIWindowsForms.dll.mdb"=>"lib/Release", "PalasoUIWindowsForms.dll.config"=>"lib/Release", "PalasoUIWindowsForms.GeckoBrowserAdapter.dll"=>"lib/Release", "PalasoUIWindowsForms.GeckoBrowserAdapter.dll.mdb"=>"lib/Release", "Ionic.Zip.dll"=>"lib/Release", "taglib-sharp.dll"=>"lib/Release", "debug/**"=>"lib/Debug"}
#     VCS: https://github.com/sillsdev/libpalaso.git [master]
# [5] build: icucil-precise64-Continuous (bt281)
#     project: Libraries
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt281
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"icu.net.*"=>"lib/Debug/icu48"}
#     VCS: https://github.com/sillsdev/icu-dotnet [master]
# [6] build: icucil-precise64-Continuous (bt281)
#     project: Libraries
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt281
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"icu.net.*"=>"lib/Release/icu48"}
#     VCS: https://github.com/sillsdev/icu-dotnet [master]
# [7] build: icucil-precise64-icu52 Continuous (bt413)
#     project: Libraries
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt413
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"icu.net.*"=>"lib/Debug/icu52"}
#     VCS: https://github.com/sillsdev/icu-dotnet [master]
# [8] build: icucil-precise64-icu52 Continuous (bt413)
#     project: Libraries
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt413
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"icu.net.*"=>"lib/Release/icu52"}
#     VCS: https://github.com/sillsdev/icu-dotnet [master]
# [9] build: wesay-doc-default (bt184)
#     project: WeSay Windows
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt184
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"WeSay_Helps.chm"=>"common"}
#     VCS: http://hg.palaso.org/wesay-doc []
# [10] build: wesay-localize-dev Update Pot and Po (bt52)
#     project: WeSay Windows
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt52
#     clean: false
#     revision: latest.lastFinished
#     paths: {"*.po"=>"common"}
#     VCS: http://hg.palaso.org/wesay-tx []

# make sure output directories exist
mkdir -p ../common
mkdir -p ../lib/Debug
mkdir -p ../lib/Debug/icu48
mkdir -p ../lib/Debug/icu52
mkdir -p ../lib/Release
mkdir -p ../lib/Release/icu48
mkdir -p ../lib/Release/icu52
mkdir -p ../lib/common

# download artifact dependencies
copy_auto http://build.palaso.org/guestAuth/repository/download/bt216/latest.lastSuccessful/Chorus_Help.chm ../common/Chorus_Help.chm
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/Chorus.exe ../lib/Release/Chorus.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/Chorus.exe.mdb ../lib/Release/Chorus.exe.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/ChorusHub.exe ../lib/Release/ChorusHub.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/ChorusHub.exe.mdb ../lib/Release/ChorusHub.exe.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/ChorusHubApp.exe ../lib/Release/ChorusHubApp.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/ChorusHubApp.exe.mdb ../lib/Release/ChorusHubApp.exe.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/ChorusMerge.exe ../lib/Release/ChorusMerge.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/ChorusMerge.exe.mdb ../lib/Release/ChorusMerge.exe.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/LibChorus.dll ../lib/Release/LibChorus.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/LibChorus.dll.mdb ../lib/Release/LibChorus.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/LibChorus.TestUtilities.dll ../lib/Release/LibChorus.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/LibChorus.TestUtilities.dll.mdb ../lib/Release/LibChorus.TestUtilities.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/Autofac.dll ../lib/Release/Autofac.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/NDesk.DBus.dll ../lib/Release/NDesk.DBus.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/NDesk.DBus.dll.config ../lib/Release/NDesk.DBus.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/debug/Autofac.dll ../lib/Debug/Autofac.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/debug/Chorus.exe ../lib/Debug/Chorus.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/debug/Chorus.exe.mdb ../lib/Debug/Chorus.exe.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/debug/ChorusHub.exe ../lib/Debug/ChorusHub.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/debug/ChorusHub.exe.mdb ../lib/Debug/ChorusHub.exe.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/debug/ChorusMerge.exe ../lib/Debug/ChorusMerge.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/debug/ChorusMerge.exe.mdb ../lib/Debug/ChorusMerge.exe.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/debug/LibChorus.TestUtilities.dll ../lib/Debug/LibChorus.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/debug/LibChorus.TestUtilities.dll.mdb ../lib/Debug/LibChorus.TestUtilities.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/debug/LibChorus.dll ../lib/Debug/LibChorus.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/debug/LibChorus.dll.mdb ../lib/Debug/LibChorus.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/debug/NDesk.DBus.dll ../lib/Debug/NDesk.DBus.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/debug/NDesk.DBus.dll.config ../lib/Debug/NDesk.DBus.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/Mercurial-i686.zip ../lib/common/Mercurial-i686.zip
copy_auto http://build.palaso.org/guestAuth/repository/download/bt323/latest.lastSuccessful/Mercurial-x86_64.zip ../lib/common/Mercurial-x86_64.zip
copy_auto http://build.palaso.org/guestAuth/repository/download/bt271/latest.lastSuccessful/L10NSharp.dll ../lib/Release/L10NSharp.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt271/latest.lastSuccessful/L10NSharp.dll.mdb ../lib/Release/L10NSharp.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt271/latest.lastSuccessful/L10NSharp.dll ../lib/Debug/L10NSharp.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt271/latest.lastSuccessful/L10NSharp.dll.mdb ../lib/Debug/L10NSharp.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Enchant.Net.dll ../lib/Release/Enchant.Net.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Enchant.Net.dll.config ../lib/Release/Enchant.Net.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/ibusdotnet.dll ../lib/Release/ibusdotnet.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.dll ../lib/Release/Palaso.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.dll.mdb ../lib/Release/Palaso.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.dll.config ../lib/Release/Palaso.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.DictionaryServices.dll ../lib/Release/Palaso.DictionaryServices.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.DictionaryServices.dll.mdb ../lib/Release/Palaso.DictionaryServices.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.Lift.dll ../lib/Release/Palaso.Lift.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.Lift.dll.mdb ../lib/Release/Palaso.Lift.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.Media.dll ../lib/Release/Palaso.Media.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.Media.dll.mdb ../lib/Release/Palaso.Media.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.Media.dll.config ../lib/Release/Palaso.Media.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.Tests.dll ../lib/Release/Palaso.Tests.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.Tests.dll.mdb ../lib/Release/Palaso.Tests.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.TestUtilities.dll ../lib/Release/Palaso.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Palaso.TestUtilities.dll.mdb ../lib/Release/Palaso.TestUtilities.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/PalasoUIWindowsForms.dll ../lib/Release/PalasoUIWindowsForms.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/PalasoUIWindowsForms.dll.mdb ../lib/Release/PalasoUIWindowsForms.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/PalasoUIWindowsForms.dll.config ../lib/Release/PalasoUIWindowsForms.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/PalasoUIWindowsForms.GeckoBrowserAdapter.dll ../lib/Release/PalasoUIWindowsForms.GeckoBrowserAdapter.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/PalasoUIWindowsForms.GeckoBrowserAdapter.dll.mdb ../lib/Release/PalasoUIWindowsForms.GeckoBrowserAdapter.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/Ionic.Zip.dll ../lib/Release/Ionic.Zip.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/taglib-sharp.dll ../lib/Release/taglib-sharp.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Enchant.Net.dll ../lib/Debug/Enchant.Net.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Enchant.Net.dll.config ../lib/Debug/Enchant.Net.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Ionic.Zip.dll ../lib/Debug/Ionic.Zip.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/L10NSharp.dll ../lib/Debug/L10NSharp.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/NDesk.DBus.dll ../lib/Debug/NDesk.DBus.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/NDesk.DBus.dll.config ../lib/Debug/NDesk.DBus.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Palaso.BuildTasks.dll ../lib/Debug/Palaso.BuildTasks.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Palaso.DictionaryServices.dll ../lib/Debug/Palaso.DictionaryServices.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Palaso.DictionaryServices.dll.mdb ../lib/Debug/Palaso.DictionaryServices.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Palaso.Lift.dll ../lib/Debug/Palaso.Lift.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Palaso.Lift.dll.mdb ../lib/Debug/Palaso.Lift.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Palaso.Media.dll ../lib/Debug/Palaso.Media.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Palaso.Media.dll.config ../lib/Debug/Palaso.Media.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Palaso.Media.dll.mdb ../lib/Debug/Palaso.Media.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Palaso.TestUtilities.dll ../lib/Debug/Palaso.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Palaso.TestUtilities.dll.mdb ../lib/Debug/Palaso.TestUtilities.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Palaso.Tests.dll ../lib/Debug/Palaso.Tests.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Palaso.Tests.dll.mdb ../lib/Debug/Palaso.Tests.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Palaso.dll ../lib/Debug/Palaso.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Palaso.dll.config ../lib/Debug/Palaso.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/Palaso.dll.mdb ../lib/Debug/Palaso.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/PalasoUIWindowsForms.GeckoBrowserAdapter.dll ../lib/Debug/PalasoUIWindowsForms.GeckoBrowserAdapter.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/PalasoUIWindowsForms.GeckoBrowserAdapter.dll.mdb ../lib/Debug/PalasoUIWindowsForms.GeckoBrowserAdapter.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/PalasoUIWindowsForms.dll ../lib/Debug/PalasoUIWindowsForms.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/PalasoUIWindowsForms.dll.config ../lib/Debug/PalasoUIWindowsForms.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/PalasoUIWindowsForms.dll.mdb ../lib/Debug/PalasoUIWindowsForms.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/ibusdotnet.dll ../lib/Debug/ibusdotnet.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt322/latest.lastSuccessful/debug/taglib-sharp.dll ../lib/Debug/taglib-sharp.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt281/latest.lastSuccessful/icu.net.dll ../lib/Debug/icu48/icu.net.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt281/latest.lastSuccessful/icu.net.dll.config ../lib/Debug/icu48/icu.net.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt281/latest.lastSuccessful/icu.net.dll.mdb ../lib/Debug/icu48/icu.net.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt281/latest.lastSuccessful/icu.net.dll ../lib/Release/icu48/icu.net.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt281/latest.lastSuccessful/icu.net.dll.config ../lib/Release/icu48/icu.net.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt281/latest.lastSuccessful/icu.net.dll.mdb ../lib/Release/icu48/icu.net.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt413/latest.lastSuccessful/icu.net.dll ../lib/Debug/icu52/icu.net.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt413/latest.lastSuccessful/icu.net.dll.config ../lib/Debug/icu52/icu.net.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt413/latest.lastSuccessful/icu.net.dll.mdb ../lib/Debug/icu52/icu.net.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt413/latest.lastSuccessful/icu.net.dll ../lib/Release/icu52/icu.net.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt413/latest.lastSuccessful/icu.net.dll.config ../lib/Release/icu52/icu.net.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt413/latest.lastSuccessful/icu.net.dll.mdb ../lib/Release/icu52/icu.net.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt184/latest.lastSuccessful/WeSay_Helps.chm ../common/WeSay_Helps.chm
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.az.po ../common/wesay.az.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.da.po ../common/wesay.da.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.es.po ../common/wesay.es.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.fr.po ../common/wesay.fr.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.he.po ../common/wesay.he.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.hi.po ../common/wesay.hi.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.id.po ../common/wesay.id.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.is.po ../common/wesay.is.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.km.po ../common/wesay.km.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.lo.po ../common/wesay.lo.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.my.po ../common/wesay.my.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.nl.po ../common/wesay.nl.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.oc.po ../common/wesay.oc.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.pl.po ../common/wesay.pl.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.pt.po ../common/wesay.pt.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.ru.po ../common/wesay.ru.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.rw.po ../common/wesay.rw.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.sv.po ../common/wesay.sv.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.th.po ../common/wesay.th.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.tpi.po ../common/wesay.tpi.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.vi.po ../common/wesay.vi.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.zh_CN.po ../common/wesay.zh_CN.po
copy_auto http://build.palaso.org/guestAuth/repository/download/bt52/latest.lastFinished/wesay.zh_TW.po ../common/wesay.zh_TW.po
# End of script
