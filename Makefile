# Copyright (c) 2021 SIL International. MIT License.
#
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in
# all copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
# THE SOFTWARE.

INSTALLATION_PREFIX ?= /usr
BUILD_CONFIG ?= Release
DESTDIR ?=

all: build-app

build-deps:
	[ -f build/nuget.exe ] || \
	  wget --output-document=build/nuget.exe \
	    https://dist.nuget.org/win-x86-commandline/v6.0.0/nuget.exe
	mono build/nuget.exe restore src/WeSay.sln

build-app:
	msbuild src/WeSay.sln -p:Configuration=$(BUILD_CONFIG)

test-deps:
	msbuild build/WeSay.proj -p:Configuration=$(BUILD_CONFIG) \
	  -t:RestoreBuildTasks

test-run:
	msbuild build/WeSay.proj -t:TestOnly -p:Configuration=$(BUILD_CONFIG) \
	  -p:Platform="x86"

install:
	# install the wrapper scripts
	install -d $(DESTDIR)$(INSTALLATION_PREFIX)/bin/
	install -d $(DESTDIR)$(INSTALLATION_PREFIX)/lib/wesay
	install -d $(DESTDIR)$(INSTALLATION_PREFIX)/share/doc/wesay
	install -m 755 \
		build/mono/wesay \
		build/mono/wesay-config \
		build/mono/chorus \
		$(DESTDIR)$(INSTALLATION_PREFIX)/bin/
	install -m 755 \
		build/mono/chorusmerge \
		$(DESTDIR)$(INSTALLATION_PREFIX)/lib/wesay
	install --mode 644 \
		build/mono/setup-wesay.bash \
		$(DESTDIR)$(INSTALLATION_PREFIX)/lib/wesay
	install --mode 644 doc/Welcome.htm $(DESTDIR)$(INSTALLATION_PREFIX)/share/doc/wesay
	# Install menu items
	install -d $(DESTDIR)$(INSTALLATION_PREFIX)/share/applications
	install -m 644 src/Installer_Linux/sil-wesay.desktop $(DESTDIR)$(INSTALLATION_PREFIX)/share/applications/sil-wesay.desktop
	install -m 644 src/Installer_Linux/sil-wesay-config.desktop $(DESTDIR)$(INSTALLATION_PREFIX)/share/applications/sil-wesay-config.desktop
	install -d $(DESTDIR)$(INSTALLATION_PREFIX)/share/appdata
	install -m 644 src/Installer_Linux/sil-wesay.appdata.xml $(DESTDIR)$(INSTALLATION_PREFIX)/share/appdata/sil-wesay.appdata.xml
	# install the binaries
	RUNMODE=BUILDINGPACKAGE BUILD=$(BUILD_CONFIG) . ./environ && \
	  msbuild /target:Install /p:Configuration=$(BUILD_CONFIG) /p:InstallDir=$(DESTDIR)$(INSTALLATION_PREFIX) /p:ApplicationNameLC=wesay build/build.mono.proj
	# Install mercurial.ini with correct fixutf8 config and line endings.
	install --mode 644 src/Installer_Linux/mercurial.ini $(DESTDIR)$(INSTALLATION_PREFIX)/lib/wesay/Mercurial
	perl -pi -e "s#/usr#$(INSTALLATION_PREFIX)#" $(DESTDIR)$(INSTALLATION_PREFIX)/lib/wesay/Mercurial/mercurial.ini
