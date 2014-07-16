#!/bin/bash -ex

XBUILD=${MONO_PREFIX:-/usr}/bin/xbuild
VERSION_SUFFIX=${1:-1234}
BUILD_COUNTER=(${VERSION_SUFFIX//[~+-]/ })

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
		/property:BUILD_NUMBER="0.0.$3.abcd" build.mono.proj)
}

cd ../libpalaso
up
bld SourcePackage DebugMono ${BUILD_COUNTER}
cd $OLDPWD

cd ../chorus
mkdir -p externals
cp -p ../libpalaso/output/DebugMono/libpalaso-2.5.${BUILD_COUNTER}.0.tar.gz externals/
up
bld SourcePackage DebugMono ${BUILD_COUNTER}

cd $OLDPWD
mkdir -p externals
cp -p ../chorus/output/DebugMono/chorus-2.4.${BUILD_COUNTER}.0.tar.gz externals/
up
bld SourcePackage Release ${VERSION_SUFFIX}

rm -rf lib/Debug
rm -rf lib/Release/[Gg]ecko*

bld Installer Release ${VERSION_SUFFIX}
