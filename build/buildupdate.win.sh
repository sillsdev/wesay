#!/bin/bash
# server=build.palaso.org
# project=WeSay1.4-Linux
# build=wesay1.4-win32-continuous
# root_dir=..
# $Id: 729ee8446b8c332adf45f7a1f11f184ebd86d235 $

cd "$(dirname "$0")"

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
# build: wesay1.4-win32-continuous (bt312)
# project: WeSay1.4-Linux
# URL: http://build.palaso.org/viewType.html?buildTypeId=bt312
# VCS: https://github.com/sillsdev/wesay [develop]
# dependencies:
# [0] build: chorus-win32-master-buildupdate (bt347)
#     project: Chorus
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt347
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"release/**"=>"lib/Release", "debug/**"=>"lib/Debug", "MercurialExtensions/**"=>"MercurialExtensions"}
#     VCS: https://github.com/sillsdev/chorus.git [master]
# [1] build: L10NSharp continuous (bt196)
#     project: L10NSharp
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt196
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"L10NSharp.dll"=>"lib/Debug", "L10NSharp.pdb"=>"lib/Debug"}
#     VCS: https://bitbucket.org/hatton/l10nsharp []
# [2] build: L10NSharp continuous (bt196)
#     project: L10NSharp
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt196
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"L10NSharp.dll"=>"lib/Release", "L10NSharp.pdb"=>"lib/Release"}
#     VCS: https://bitbucket.org/hatton/l10nsharp []
# [3] build: palaso-win32-master-buildupdate (bt348)
#     project: libpalaso
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt348
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"release/**"=>"lib/Release", "debug/**"=>"lib/Debug"}
#     VCS: https://github.com/sillsdev/libpalaso.git []
# [4] build: icucil-win32-default Continuous (bt14)
#     project: Libraries
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt14
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"icu*.dll"=>"lib/Release"}
#     VCS: https://github.com/sillsdev/icu-dotnet [master]
# [5] build: icucil-win32-default Continuous (bt14)
#     project: Libraries
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt14
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"icu*.dll"=>"lib/Debug"}
#     VCS: https://github.com/sillsdev/icu-dotnet [master]

# make sure output directories exist
mkdir -p ../lib/Release
mkdir -p ../lib/Debug
mkdir -p ../MercurialExtensions
mkdir -p ../MercurialExtensions/fixutf8

# download artifact dependencies
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/release/Autofac.dll ../lib/Release/Autofac.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/release/Chorus.exe ../lib/Release/Chorus.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/release/Chorus.pdb ../lib/Release/Chorus.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/release/ChorusHub.exe ../lib/Release/ChorusHub.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/release/ChorusHub.pdb ../lib/Release/ChorusHub.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/release/ChorusMerge.exe ../lib/Release/ChorusMerge.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/release/ChorusMerge.pdb ../lib/Release/ChorusMerge.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/release/LibChorus.TestUtilities.dll ../lib/Release/LibChorus.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/release/LibChorus.TestUtilities.pdb ../lib/Release/LibChorus.TestUtilities.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/release/LibChorus.dll ../lib/Release/LibChorus.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/release/LibChorus.pdb ../lib/Release/LibChorus.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/release/Mercurial.zip ../lib/Release/Mercurial.zip
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/release/Vulcan.Uczniowie.HelpProvider.dll ../lib/Release/Vulcan.Uczniowie.HelpProvider.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/debug/Autofac.dll ../lib/Debug/Autofac.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/debug/Chorus.exe ../lib/Debug/Chorus.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/debug/Chorus.pdb ../lib/Debug/Chorus.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/debug/ChorusHub.exe ../lib/Debug/ChorusHub.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/debug/ChorusHub.pdb ../lib/Debug/ChorusHub.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/debug/ChorusMerge.exe ../lib/Debug/ChorusMerge.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/debug/ChorusMerge.pdb ../lib/Debug/ChorusMerge.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/debug/LibChorus.TestUtilities.dll ../lib/Debug/LibChorus.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/debug/LibChorus.TestUtilities.pdb ../lib/Debug/LibChorus.TestUtilities.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/debug/LibChorus.dll ../lib/Debug/LibChorus.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/debug/LibChorus.pdb ../lib/Debug/LibChorus.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/debug/Mercurial.zip ../lib/Debug/Mercurial.zip
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/debug/Vulcan.Uczniowie.HelpProvider.dll ../lib/Debug/Vulcan.Uczniowie.HelpProvider.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/.guidsForInstaller.xml ../MercurialExtensions/.guidsForInstaller.xml
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/Dummy.txt ../MercurialExtensions/Dummy.txt
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/.gitignore ../MercurialExtensions/fixutf8/.gitignore
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/.guidsForInstaller.xml ../MercurialExtensions/fixutf8/.guidsForInstaller.xml
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/.hg_archival.txt ../MercurialExtensions/fixutf8/.hg_archival.txt
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/.hgignore ../MercurialExtensions/fixutf8/.hgignore
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/README. ../MercurialExtensions/fixutf8/README.
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/buildcpmap.py ../MercurialExtensions/fixutf8/buildcpmap.py
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/cpmap.pyc ../MercurialExtensions/fixutf8/cpmap.pyc
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/fixutf8.py ../MercurialExtensions/fixutf8/fixutf8.py
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/fixutf8.pyc ../MercurialExtensions/fixutf8/fixutf8.pyc
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/fixutf8.pyo ../MercurialExtensions/fixutf8/fixutf8.pyo
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/osutil.py ../MercurialExtensions/fixutf8/osutil.py
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/osutil.pyc ../MercurialExtensions/fixutf8/osutil.pyc
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/osutil.pyo ../MercurialExtensions/fixutf8/osutil.pyo
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/win32helper.py ../MercurialExtensions/fixutf8/win32helper.py
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/win32helper.pyc ../MercurialExtensions/fixutf8/win32helper.pyc
copy_auto http://build.palaso.org/guestAuth/repository/download/bt347/latest.lastSuccessful/MercurialExtensions/fixutf8/win32helper.pyo ../MercurialExtensions/fixutf8/win32helper.pyo
copy_auto http://build.palaso.org/guestAuth/repository/download/bt196/latest.lastSuccessful/L10NSharp.dll ../lib/Debug/L10NSharp.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt196/latest.lastSuccessful/L10NSharp.pdb ../lib/Debug/L10NSharp.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt196/latest.lastSuccessful/L10NSharp.dll ../lib/Release/L10NSharp.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt196/latest.lastSuccessful/L10NSharp.pdb ../lib/Release/L10NSharp.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/Interop.WIA.dll ../lib/Release/Interop.WIA.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/Palaso.BuildTasks.dll ../lib/Release/Palaso.BuildTasks.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/Palaso.BuildTasks.pdb ../lib/Release/Palaso.BuildTasks.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/Palaso.DictionaryServices.dll ../lib/Release/Palaso.DictionaryServices.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/Palaso.DictionaryServices.pdb ../lib/Release/Palaso.DictionaryServices.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/Palaso.Lift.dll ../lib/Release/Palaso.Lift.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/Palaso.Lift.pdb ../lib/Release/Palaso.Lift.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/Palaso.Media.dll ../lib/Release/Palaso.Media.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/Palaso.Media.pdb ../lib/Release/Palaso.Media.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/Palaso.TestUtilities.dll ../lib/Release/Palaso.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/Palaso.TestUtilities.pdb ../lib/Release/Palaso.TestUtilities.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/Palaso.Tests.dll ../lib/Release/Palaso.Tests.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/Palaso.Tests.pdb ../lib/Release/Palaso.Tests.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/Palaso.dll ../lib/Release/Palaso.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/Palaso.pdb ../lib/Release/Palaso.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/PalasoUIWindowsForms.dll ../lib/Release/PalasoUIWindowsForms.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/PalasoUIWindowsForms.pdb ../lib/Release/PalasoUIWindowsForms.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/SIL.Archiving.dll ../lib/Release/SIL.Archiving.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/release/SIL.Archiving.pdb ../lib/Release/SIL.Archiving.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/Interop.WIA.dll ../lib/Debug/Interop.WIA.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/Palaso.BuildTasks.dll ../lib/Debug/Palaso.BuildTasks.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/Palaso.BuildTasks.pdb ../lib/Debug/Palaso.BuildTasks.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/Palaso.DictionaryServices.dll ../lib/Debug/Palaso.DictionaryServices.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/Palaso.DictionaryServices.pdb ../lib/Debug/Palaso.DictionaryServices.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/Palaso.Lift.dll ../lib/Debug/Palaso.Lift.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/Palaso.Lift.pdb ../lib/Debug/Palaso.Lift.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/Palaso.Media.dll ../lib/Debug/Palaso.Media.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/Palaso.Media.pdb ../lib/Debug/Palaso.Media.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/Palaso.TestUtilities.dll ../lib/Debug/Palaso.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/Palaso.TestUtilities.pdb ../lib/Debug/Palaso.TestUtilities.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/Palaso.Tests.dll ../lib/Debug/Palaso.Tests.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/Palaso.Tests.pdb ../lib/Debug/Palaso.Tests.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/Palaso.dll ../lib/Debug/Palaso.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/Palaso.pdb ../lib/Debug/Palaso.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/PalasoUIWindowsForms.dll ../lib/Debug/PalasoUIWindowsForms.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/PalasoUIWindowsForms.pdb ../lib/Debug/PalasoUIWindowsForms.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/SIL.Archiving.dll ../lib/Debug/SIL.Archiving.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt348/latest.lastSuccessful/debug/SIL.Archiving.pdb ../lib/Debug/SIL.Archiving.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icu.net.dll ../lib/Release/icu.net.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icudt40.dll ../lib/Release/icudt40.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icuin40.dll ../lib/Release/icuin40.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icuuc40.dll ../lib/Release/icuuc40.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icu.net.dll ../lib/Debug/icu.net.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icudt40.dll ../lib/Debug/icudt40.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icuin40.dll ../lib/Debug/icuin40.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icuuc40.dll ../lib/Debug/icuuc40.dll
# End of script
