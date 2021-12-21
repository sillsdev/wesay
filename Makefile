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

install:
	# install the wrapper scripts
	install -d $(DESTDIR)$(INSTALLATION_PREFIX)/bin/
	install -d $(DESTDIR)$(INSTALLATION_PREFIX)/lib/wesay
	install -m 755 build/mono/wesay $(DESTDIR)$(INSTALLATION_PREFIX)/bin/wesay
	install -m 755 build/mono/wesay-config $(DESTDIR)$(INSTALLATION_PREFIX)/bin/wesay-config
	install -m 755 build/mono/chorus $(DESTDIR)$(INSTALLATION_PREFIX)/bin/chorus
	install -m 755 build/mono/chorusmerge $(DESTDIR)$(INSTALLATION_PREFIX)/lib/wesay
	# adjust the paths inside the wrapper scripts
	# Install menu items
	install -d $(DESTDIR)$(INSTALLATION_PREFIX)/share/applications
	install -m 644 src/Installer_Linux/sil-wesay.desktop $(DESTDIR)$(INSTALLATION_PREFIX)/share/applications/sil-wesay.desktop
	install -m 644 src/Installer_Linux/sil-wesay-config.desktop $(DESTDIR)$(INSTALLATION_PREFIX)/share/applications/sil-wesay-config.desktop
	install -d $(DESTDIR)$(INSTALLATION_PREFIX)/share/appdata
	install -m 644 src/Installer_Linux/sil-wesay.appdata.xml $(DESTDIR)$(INSTALLATION_PREFIX)/share/appdata/sil-wesay.appdata.xml
	# install the binaries
	RUNMODE=BUILDINGPACKAGE . ./environ && \
	cd build && \
	xbuild /target:Install /p:Configuration=$(CONFIGURATION) /p:InstallDir=../$(DESTDIR)$(INSTALLATION_PREFIX) /p:ApplicationNameLC=wesay build.mono.proj
	# Apparently the downloaded mercurial.ini doesn't have the right fixutf8 config, and it also has
	# wrong line endings, so we re-create the entire file
	echo "$$MERCURIAL_INI" > $(DESTDIR)$(INSTALLATION_PREFIX)/lib/wesay/Mercurial/mercurial.ini
