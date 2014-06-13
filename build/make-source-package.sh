#!/bin/bash -ex

XBUILD=${MONO_PREFIX:-/usr}/bin/xbuild
BUILD_COUNTER=${1:-1234}

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
		/property:BUILD_NUMBER="0.0.$BUILD_COUNTER.abcd" build.mono.proj)
}

cd ../libpalaso
up
bld SourcePackage DebugMono
cd $OLDPWD

cd ../chorus
mkdir -p externals
cp -p ../libpalaso/output/DebugMono/libpalaso-2.5.${BUILD_COUNTER}.0.tar.gz externals/
up
bld SourcePackage DebugMono
cd $OLDPWD

mkdir -p externals
cp -p ../chorus/output/DebugMono/chorus-2.4.${BUILD_COUNTER}.0.tar.gz externals/
up
bld SourcePackage Release

rm -rf lib/Debug
rm -rf lib/Release/[Gg]ecko*

bld Installer Release
