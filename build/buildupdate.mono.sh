#!/bin/bash
# server=build.palaso.org
# project=WeSay1.4-Linux
# build=wesay1.4-precise64-continuous
# root_dir=..
# $Id: da666a7e5eb1d63b434514279cd14cacd26c730f $

# *** Functions ***
force=

while getopts f opt; do
	case $opt in
	f)
		force=1
		;;

	esac
done

shift $((OPTIND - 1))


copy_auto() {
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
# build: wesay1.4-precise64-continuous (bt314)
# project: WeSay1.4-Linux
# URL: http://build.palaso.org/viewType.html?buildTypeId=bt314
# VCS: https://github.com/sillsdev/wesay [develop]
# dependencies:
# [0] build: chorus-precise64-develop-continuous (bt337)
#     project: Chorus
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt337
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"release/**"=>"lib/Release", "debug/**"=>"lib/Debug", "Mercurial-i686.zip"=>"lib/common", "Mercurial-x86_64.zip"=>"lib/common"}
#     VCS: https://github.com/sillsdev/chorus.git [develop]
# [1] build: L10NSharp Mono continuous (bt271)
#     project: L10NSharp
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt271
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"L10NSharp.dll"=>"lib/Release", "L10NSharp.dll.mdb"=>"lib/Release"}
#     VCS: https://bitbucket.org/hatton/l10nsharp [default]
# [2] build: L10NSharp Mono continuous (bt271)
#     project: L10NSharp
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt271
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"L10NSharp.dll"=>"lib/Debug", "L10NSharp.dll.mdb"=>"lib/Debug"}
#     VCS: https://bitbucket.org/hatton/l10nsharp [default]
# [3] build: palaso-precise64-develop-continuous (bt334)
#     project: libpalaso
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt334
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"release/**"=>"lib/Release", "debug/**"=>"lib/Debug"}
#     VCS: https://github.com/sillsdev/libpalaso.git [develop]
# [4] build: icucil-precise64-Continuous (bt281)
#     project: Libraries
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt281
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"icu.net.*"=>"lib/Release"}
#     VCS: https://github.com/sillsdev/icu-dotnet [master]
# [5] build: icucil-precise64-Continuous (bt281)
#     project: Libraries
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt281
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"icu.net.*"=>"lib/Debug"}
#     VCS: https://github.com/sillsdev/icu-dotnet [master]

# make sure output directories exist
mkdir -p ../lib/Release
mkdir -p ../lib/Debug
mkdir -p ../lib/common

# download artifact dependencies
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/Autofac.dll ../lib/Release/Autofac.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/Chorus.exe ../lib/Release/Chorus.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/Chorus.exe.config ../lib/Release/Chorus.exe.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/Chorus.exe.mdb ../lib/Release/Chorus.exe.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/ChorusHub.exe ../lib/Release/ChorusHub.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/ChorusHub.exe.config ../lib/Release/ChorusHub.exe.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/ChorusHub.exe.mdb ../lib/Release/ChorusHub.exe.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/ChorusMerge.exe ../lib/Release/ChorusMerge.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/ChorusMerge.exe.config ../lib/Release/ChorusMerge.exe.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/ChorusMerge.exe.mdb ../lib/Release/ChorusMerge.exe.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/Geckofx-Winforms-14.dll ../lib/Release/Geckofx-Winforms-14.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/LibChorus.TestUtilities.dll ../lib/Release/LibChorus.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/LibChorus.TestUtilities.dll.mdb ../lib/Release/LibChorus.TestUtilities.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/LibChorus.dll ../lib/Release/LibChorus.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/LibChorus.dll.mdb ../lib/Release/LibChorus.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/NDesk.DBus.dll ../lib/Release/NDesk.DBus.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/NDesk.DBus.dll.config ../lib/Release/NDesk.DBus.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/geckofix.so ../lib/Release/geckofix.so
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/geckofx-core-14.dll ../lib/Release/geckofx-core-14.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/release/geckofx-core-14.dll.config ../lib/Release/geckofx-core-14.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/Autofac.dll ../lib/Debug/Autofac.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/Chorus.exe ../lib/Debug/Chorus.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/Chorus.exe.config ../lib/Debug/Chorus.exe.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/Chorus.exe.mdb ../lib/Debug/Chorus.exe.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/ChorusHub.exe ../lib/Debug/ChorusHub.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/ChorusHub.exe.config ../lib/Debug/ChorusHub.exe.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/ChorusHub.exe.mdb ../lib/Debug/ChorusHub.exe.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/ChorusMerge.exe ../lib/Debug/ChorusMerge.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/ChorusMerge.exe.config ../lib/Debug/ChorusMerge.exe.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/ChorusMerge.exe.mdb ../lib/Debug/ChorusMerge.exe.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/Geckofx-Winforms-14.dll ../lib/Debug/Geckofx-Winforms-14.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/LibChorus.TestUtilities.dll ../lib/Debug/LibChorus.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/LibChorus.TestUtilities.dll.mdb ../lib/Debug/LibChorus.TestUtilities.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/LibChorus.dll ../lib/Debug/LibChorus.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/LibChorus.dll.mdb ../lib/Debug/LibChorus.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/NDesk.DBus.dll ../lib/Debug/NDesk.DBus.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/NDesk.DBus.dll.config ../lib/Debug/NDesk.DBus.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/geckofix.so ../lib/Debug/geckofix.so
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/geckofx-core-14.dll ../lib/Debug/geckofx-core-14.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/debug/geckofx-core-14.dll.config ../lib/Debug/geckofx-core-14.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/Mercurial-i686.zip ../lib/common/Mercurial-i686.zip
copy_auto http://build.palaso.org/guestAuth/repository/download/bt337/latest.lastSuccessful/Mercurial-x86_64.zip ../lib/common/Mercurial-x86_64.zip
copy_auto http://build.palaso.org/guestAuth/repository/download/bt271/latest.lastSuccessful/L10NSharp.dll ../lib/Release/L10NSharp.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt271/latest.lastSuccessful/L10NSharp.dll.mdb ../lib/Release/L10NSharp.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt271/latest.lastSuccessful/L10NSharp.dll ../lib/Debug/L10NSharp.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt271/latest.lastSuccessful/L10NSharp.dll.mdb ../lib/Debug/L10NSharp.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/Palaso.BuildTasks.dll ../lib/Release/Palaso.BuildTasks.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/Palaso.DictionaryServices.dll ../lib/Release/Palaso.DictionaryServices.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/Palaso.Lift.dll ../lib/Release/Palaso.Lift.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/Palaso.Lift.dll.mdb ../lib/Release/Palaso.Lift.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/Palaso.Media.dll ../lib/Release/Palaso.Media.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/Palaso.Media.dll.config ../lib/Release/Palaso.Media.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/Palaso.Media.dll.mdb ../lib/Release/Palaso.Media.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/Palaso.TestUtilities.dll ../lib/Release/Palaso.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/Palaso.TestUtilities.dll.mdb ../lib/Release/Palaso.TestUtilities.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/Palaso.Tests.dll ../lib/Release/Palaso.Tests.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/Palaso.Tests.dll.mdb ../lib/Release/Palaso.Tests.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/Palaso.dll ../lib/Release/Palaso.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/Palaso.dll.config ../lib/Release/Palaso.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/Palaso.dll.mdb ../lib/Release/Palaso.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/PalasoUIWindowsForms.dll ../lib/Release/PalasoUIWindowsForms.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/PalasoUIWindowsForms.dll.config ../lib/Release/PalasoUIWindowsForms.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/PalasoUIWindowsForms.dll.mdb ../lib/Release/PalasoUIWindowsForms.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/SIL.Archiving.dll ../lib/Release/SIL.Archiving.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/SIL.Archiving.dll.config ../lib/Release/SIL.Archiving.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/SIL.Archiving.dll.mdb ../lib/Release/SIL.Archiving.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/release/ibusdotnet.dll ../lib/Release/ibusdotnet.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/Palaso.BuildTasks.dll ../lib/Debug/Palaso.BuildTasks.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/Palaso.DictionaryServices.dll ../lib/Debug/Palaso.DictionaryServices.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/Palaso.Lift.dll ../lib/Debug/Palaso.Lift.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/Palaso.Lift.dll.mdb ../lib/Debug/Palaso.Lift.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/Palaso.Media.dll ../lib/Debug/Palaso.Media.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/Palaso.Media.dll.config ../lib/Debug/Palaso.Media.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/Palaso.Media.dll.mdb ../lib/Debug/Palaso.Media.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/Palaso.TestUtilities.dll ../lib/Debug/Palaso.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/Palaso.TestUtilities.dll.mdb ../lib/Debug/Palaso.TestUtilities.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/Palaso.Tests.dll ../lib/Debug/Palaso.Tests.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/Palaso.Tests.dll.mdb ../lib/Debug/Palaso.Tests.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/Palaso.dll ../lib/Debug/Palaso.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/Palaso.dll.config ../lib/Debug/Palaso.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/Palaso.dll.mdb ../lib/Debug/Palaso.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/PalasoUIWindowsForms.dll ../lib/Debug/PalasoUIWindowsForms.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/PalasoUIWindowsForms.dll.mdb ../lib/Debug/PalasoUIWindowsForms.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/SIL.Archiving.dll ../lib/Debug/SIL.Archiving.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/SIL.Archiving.dll.config ../lib/Debug/SIL.Archiving.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/SIL.Archiving.dll.mdb ../lib/Debug/SIL.Archiving.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt334/latest.lastSuccessful/debug/ibusdotnet.dll ../lib/Debug/ibusdotnet.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt281/latest.lastSuccessful/icu.net.dll ../lib/Release/icu.net.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt281/latest.lastSuccessful/icu.net.dll.config ../lib/Release/icu.net.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt281/latest.lastSuccessful/icu.net.dll.mdb ../lib/Release/icu.net.dll.mdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt281/latest.lastSuccessful/icu.net.dll ../lib/Debug/icu.net.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt281/latest.lastSuccessful/icu.net.dll.config ../lib/Debug/icu.net.dll.config
copy_auto http://build.palaso.org/guestAuth/repository/download/bt281/latest.lastSuccessful/icu.net.dll.mdb ../lib/Debug/icu.net.dll.mdb
# End of script
