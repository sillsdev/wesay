#!/bin/bash -ex

cd "$(dirname "$0")/.."

XBUILD=${MONO_PREFIX:-/usr}/bin/xbuild
VERSION_SUFFIX=${1:-1234}
BUILD_COUNTER=(${VERSION_SUFFIX//[~+-]/ })

# Currently, the WeSay Package won't build if using
# Debug configuration.  For now, continue to just
# do Release
BUILD_PROJECT=build.mono.proj
BUILD_CONFIGURATION=Release
DEPENDENCY_CONFIGURATION=${2:-Release}
PALASO_PROJECT=Palaso.proj
PALASO_CONFIGURATION=${DEPENDENCY_CONFIGURATION}Mono
CHORUS_PROJECT=Chorus.proj
CHORUS_CONFIGURATION=${DEPENDENCY_CONFIGURATION}Mono

up()
{
	(cd build && ./buildupdate.mono.sh)
}

bld()
{
	mkdir -p output/$2
	(cd build && $XBUILD /target:${1?} \
		/property:Configuration=${2?} \
		/property:RootDir=$(cd .. && pwd) \
		/property:BUILD_NUMBER="0.0.$3.abcd" $4)
}

cd ../libpalaso
up
bld SourcePackage ${PALASO_CONFIGURATION} ${BUILD_COUNTER} ${PALASO_PROJECT}
cd $OLDPWD

cd ../chorus
mkdir -p externals
cp -p ../libpalaso/output/${PALASO_CONFIGURATION}/libpalaso-2.6.tar.gz externals/
up
bld SourcePackage ${CHORUS_CONFIGURATION} ${BUILD_COUNTER} ${CHORUS_PROJECT}

cd $OLDPWD
mkdir -p externals
cp -p ../chorus/output/${CHORUS_CONFIGURATION}/chorus-2.4.${BUILD_COUNTER}.0.tar.gz externals/
up
bld SourcePackage ${BUILD_CONFIGURATION} ${VERSION_SUFFIX} ${BUILD_PROJECT}

rm -rf lib/Debug
rm -rf lib/Release/[Gg]ecko*

sed "s/@BUILD_CONFIGURATION@/${BUILD_CONFIGURATION}/g;
	 s/@PALASO_CONFIGURATION@/${PALASO_CONFIGURATION}/g;
	 s/@CHORUS_CONFIGURATION@/${CHORUS_CONFIGURATION}/g" \
	package/lucid/rules.in > package/lucid/rules
bld Installer ${BUILD_CONFIGURATION} ${VERSION_SUFFIX} ${BUILD_PROJECT}
