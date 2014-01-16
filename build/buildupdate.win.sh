#!/bin/bash
# server=build.palaso.org
# project=WeSay1.4-Linux
# build=WeSay1.4-win32 Continuous
# root_dir=..

# *** Functions ***
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
	if [ -e "$2" ]
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
# build: WeSay1.4-win32 Continuous (bt312)
# project: WeSay1.4-Linux
# URL: http://build.palaso.org/viewType.html?buildTypeId=bt312
# VCS: https://github.com/sillsdev/wesay [develop]
# dependencies:
# [0] build: chorus-win32-master Continuous (bt2)
#     project: Chorus
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt2
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"Chorus.exe"=>"lib/Release", "Chorus.pdb"=>"lib/Release", "LibChorus.dll"=>"lib/Release", "LibChorus.pdb"=>"lib/Release", "ChorusMerge.exe"=>"lib/Release", "ChorusMerge.pdb"=>"lib/Release", "LibChorus.TestUtilities.dll"=>"lib/Release", "LibChorus.TestUtilities.pdb"=>"lib/Release", "Mercurial.zip"=>"lib/Release", "MercurialExtensions/**"=>"MercurialExtensions"}
#     VCS: https://github.com/sillsdev/chorus.git [master]
# [1] build: chorus-win32-master Continuous (bt2)
#     project: Chorus
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt2
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"Autofac.dll"=>"lib/common"}
#     VCS: https://github.com/sillsdev/chorus.git [master]
# [2] build: L10NSharp continuous (bt196)
#     project: L10NSharp
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt196
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"L10NSharp.dll"=>"lib/common", "L10NSharp.pdb"=>"lib/common"}
#     VCS: https://bitbucket.org/hatton/l10nsharp []
# [3] build: palaso-win32-master Continuous (bt223)
#     project: libpalaso
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt223
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"Palaso.BuildTasks.dll"=>"lib/Release", "Palaso.dll"=>"lib/Release", "Palaso.pdb"=>"lib/Release", "Palaso.TestUtilities.dll"=>"lib/Release", "Palaso.TestUtilities.pdb"=>"lib/Release", "PalasoUIWindowsForms.dll"=>"lib/Release", "PalasoUIWindowsForms.pdb"=>"lib/Release", "Palaso.Lift.dll"=>"lib/Release", "Palaso.Lift.pdb"=>"lib/Release", "Palaso.DictionaryServices.dll"=>"lib/Release", "Palaso.DictionaryServices.pdb"=>"lib/Release", "Palaso.Media.dll"=>"lib/Release", "Palaso.Media.pdb"=>"lib/Release", "Palaso.Tests.dll"=>"lib/Release", "Palaso.Tests.pdb"=>"lib/Release"}
#     VCS: https://github.com/sillsdev/libpalaso.git []
# [4] build: icucil-win32-default Continuous (bt14)
#     project: Libraries
#     URL: http://build.palaso.org/viewType.html?buildTypeId=bt14
#     clean: false
#     revision: latest.lastSuccessful
#     paths: {"icu*.dll"=>"lib/Release"}
#     VCS: https://github.com/sillsdev/icu-dotnet [master]

# make sure output directories exist
mkdir -p ../lib/Release
mkdir -p ../MercurialExtensions
mkdir -p ../MercurialExtensions/fixutf8
mkdir -p ../lib/common

# download artifact dependencies
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/Chorus.exe ../lib/Release/Chorus.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/Chorus.pdb ../lib/Release/Chorus.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/LibChorus.dll ../lib/Release/LibChorus.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/LibChorus.pdb ../lib/Release/LibChorus.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/ChorusMerge.exe ../lib/Release/ChorusMerge.exe
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/ChorusMerge.pdb ../lib/Release/ChorusMerge.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/LibChorus.TestUtilities.dll ../lib/Release/LibChorus.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/LibChorus.TestUtilities.pdb ../lib/Release/LibChorus.TestUtilities.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/Mercurial.zip ../lib/Release/Mercurial.zip
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/.guidsForInstaller.xml ../MercurialExtensions/.guidsForInstaller.xml
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/Dummy.txt ../MercurialExtensions/Dummy.txt
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/.gitignore ../MercurialExtensions/fixutf8/.gitignore
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/.guidsForInstaller.xml ../MercurialExtensions/fixutf8/.guidsForInstaller.xml
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/.hg_archival.txt ../MercurialExtensions/fixutf8/.hg_archival.txt
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/.hgignore ../MercurialExtensions/fixutf8/.hgignore
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/README. ../MercurialExtensions/fixutf8/README.
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/buildcpmap.py ../MercurialExtensions/fixutf8/buildcpmap.py
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/cpmap.pyc ../MercurialExtensions/fixutf8/cpmap.pyc
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/fixutf8.py ../MercurialExtensions/fixutf8/fixutf8.py
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/fixutf8.pyc ../MercurialExtensions/fixutf8/fixutf8.pyc
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/fixutf8.pyo ../MercurialExtensions/fixutf8/fixutf8.pyo
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/osutil.py ../MercurialExtensions/fixutf8/osutil.py
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/osutil.pyc ../MercurialExtensions/fixutf8/osutil.pyc
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/osutil.pyo ../MercurialExtensions/fixutf8/osutil.pyo
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/win32helper.py ../MercurialExtensions/fixutf8/win32helper.py
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/win32helper.pyc ../MercurialExtensions/fixutf8/win32helper.pyc
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/MercurialExtensions/fixutf8/win32helper.pyo ../MercurialExtensions/fixutf8/win32helper.pyo
copy_auto http://build.palaso.org/guestAuth/repository/download/bt2/latest.lastSuccessful/Autofac.dll ../lib/common/Autofac.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt196/latest.lastSuccessful/L10NSharp.dll ../lib/common/L10NSharp.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt196/latest.lastSuccessful/L10NSharp.pdb ../lib/common/L10NSharp.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.BuildTasks.dll ../lib/Release/Palaso.BuildTasks.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.dll ../lib/Release/Palaso.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.pdb ../lib/Release/Palaso.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.TestUtilities.dll ../lib/Release/Palaso.TestUtilities.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.TestUtilities.pdb ../lib/Release/Palaso.TestUtilities.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/PalasoUIWindowsForms.dll ../lib/Release/PalasoUIWindowsForms.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/PalasoUIWindowsForms.pdb ../lib/Release/PalasoUIWindowsForms.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.Lift.dll ../lib/Release/Palaso.Lift.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.Lift.pdb ../lib/Release/Palaso.Lift.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.DictionaryServices.dll ../lib/Release/Palaso.DictionaryServices.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.DictionaryServices.pdb ../lib/Release/Palaso.DictionaryServices.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.Media.dll ../lib/Release/Palaso.Media.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.Media.pdb ../lib/Release/Palaso.Media.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.Tests.dll ../lib/Release/Palaso.Tests.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt223/latest.lastSuccessful/Palaso.Tests.pdb ../lib/Release/Palaso.Tests.pdb
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icu.net.dll ../lib/Release/icu.net.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icudt40.dll ../lib/Release/icudt40.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icuin40.dll ../lib/Release/icuin40.dll
copy_auto http://build.palaso.org/guestAuth/repository/download/bt14/latest.lastSuccessful/icuuc40.dll ../lib/Release/icuuc40.dll
# End of script
