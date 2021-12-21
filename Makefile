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

install:
	# install the wrapper scripts
	install -d $(DESTDIR)/usr/bin/
	install -d $(DESTDIR)$(LIB)
	install -m 755 build/mono/wesay $(DESTDIR)/usr/bin/$(PACKAGENAME)
	install -m 755 build/mono/wesay-config $(DESTDIR)/usr/bin/$(PACKAGENAME)-config
	install -m 755 build/mono/chorus $(DESTDIR)/usr/bin/chorus$(APPEND)
	install -m 755 build/mono/chorusmerge $(DESTDIR)$(LIB)
	# adjust the paths inside the wrapper scripts
	sed -e "s#/usr/lib/wesay#/usr/lib/$(PACKAGENAME)#" -i $(DESTDIR)/usr/bin/$(PACKAGENAME)
	sed -e "s#/usr/share/doc/wesay#/usr/share/doc/$(PACKAGENAME)#" -i $(DESTDIR)/usr/bin/$(PACKAGENAME)
	sed -e "s#wesay-version#$(PACKAGENAME)-version#" -i $(DESTDIR)/usr/bin/$(PACKAGENAME)
	sed -e "s#dpkg -s wesay#dpkg -s $(PACKAGENAME)#" -i $(DESTDIR)/usr/bin/$(PACKAGENAME)
	sed -e "s#/usr/lib/wesay#/usr/lib/$(PACKAGENAME)#" -i $(DESTDIR)/usr/bin/$(PACKAGENAME)-config
	sed -e "s#/usr/share/doc/wesay#/usr/share/doc/$(PACKAGENAME)#" -i $(DESTDIR)/usr/bin/$(PACKAGENAME)-config
	sed -e "s#wesay-version#$(PACKAGENAME)-version#" -i $(DESTDIR)/usr/bin/$(PACKAGENAME)-config
	sed -e "s#dpkg -s wesay#dpkg -s $(PACKAGENAME)#" -i $(DESTDIR)/usr/bin/$(PACKAGENAME)-config
	sed -e "s#/usr/lib/wesay#/usr/lib/$(PACKAGENAME)#" -i $(DESTDIR)/usr/bin/chorus$(APPEND)
	sed -e "s#/usr/lib/wesay#/usr/lib/$(PACKAGENAME)#" -i $(DESTDIR)$(LIB)/chorusmerge
	# Install menu items
	install -d $(DESTDIR)/usr/share/applications
	install -m 644 src/Installer_Linux/sil-wesay.desktop $(DESTDIR)/usr/share/applications/sil-$(PACKAGENAME).desktop
	install -m 644 src/Installer_Linux/sil-wesay-config.desktop $(DESTDIR)/usr/share/applications/sil-$(PACKAGENAME)-config.desktop
	install -d $(DESTDIR)/usr/share/appdata
	install -m 644 src/Installer_Linux/sil-$(PACKAGENAME).appdata.xml $(DESTDIR)/usr/share/appdata/sil-$(PACKAGENAME).appdata.xml
	# Update the name of the Executable and Icons
	sed -e "s/\(Icon=sil-\).*/\1$(PACKAGENAME)/" -e "s/Exec=[^ ]*\(.*\)/Exec=$PACKAGENAME\1/" -i $(DESTDIR)/usr/share/applications/sil-$(PACKAGENAME).desktop
	sed -e "s/\(Icon=sil-\).*/\1$(PACKAGENAME)-config/" -e "s/Exec=[^ ]*\(.*\)/Exec=$(PACKAGENAME)-config\1/" -i $(DESTDIR)/usr/share/applications/sil-$(PACKAGENAME)-config.desktop
	# install the binaries
	RUNMODE=BUILDINGPACKAGE . ./environ && \
	cd build && \
	xbuild /target:Install /p:Configuration=$(CONFIGURATION) /p:InstallDir=../$(DESTDIR)/usr /p:ApplicationNameLC=$(PACKAGENAME) build.mono.proj
	# Apparently the downloaded mercurial.ini doesn't have the right fixutf8 config, and it also has
	# wrong line endings, so we re-create the entire file
	echo "$$MERCURIAL_INI" > $(DESTDIR)$(LIB)/Mercurial/mercurial.ini
