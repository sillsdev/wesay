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
#
# Prepare to run WeSay. This should be sourced, not executed.

WESAY_PREFIX="$(cd "${script_dir}/.." && pwd)"
cd "${WESAY_PREFIX}/lib/wesay"
RUNMODE=INSTALLED
. ./environ
cd "$OLDPWD"

DEFAULT_WEB=`xdg-settings get default-web-browser`
DEFAULT_FILEWEB=`xdg-settings get default-url-scheme-handler file`

if [ "$DEFAULT_WEB" != "$DEFAULT_WEB" ]
then
	xdg-settings set default-url-scheme-handler file "$DEFAULT_WEB"
fi

PACKAGE_VERSION="1.9.0"
echo "Package version: [$PACKAGE_VERSION]"

STANDARD_XDG_DATA_HOME="${HOME}/.local/share"
STANDARD_XDG_CONFIG_HOME="${HOME}/.config"
XDG_DATA_HOME="${XDG_DATA_HOME:-${STANDARD_XDG_DATA_HOME}}"
XDG_CONFIG_HOME="${XDG_CONFIG_HOME:-${STANDARD_XDG_CONFIG_HOME}}"
WESAY_CONFIG_DIR="${XDG_CONFIG_HOME}/WeSay"

mkdir --verbose --parents "${WESAY_CONFIG_DIR}"

if [[ -f "${WESAY_CONFIG_DIR}/wesay-version" ]]; then
	# check if version in there is same as current
	echo "wesay-config: Checking last version"
	LAST_VERSION=$(cat "${WESAY_CONFIG_DIR}/wesay-version")
	echo "wesay-config: Last version: [$LAST_VERSION]"
	if [ "$PACKAGE_VERSION" != "$LAST_VERSION" ]
	then
		echo "wesay-config: Last version different from current"
		# open the welcome file
		xdg-open "${WESAY_PREFIX}/share/doc/wesay/Welcome.htm"
		#write out the current version
		echo "$PACKAGE_VERSION" > "${WESAY_CONFIG_DIR}/wesay-version"
	else
		echo "wesay-config: Last version same as current"
	fi
else
	echo "wesay-config: No existing version file"
	#open the welcome file
	xdg-open "${WESAY_PREFIX}/share/doc/wesay/Welcome.htm"
	#write out the current version
	echo "$PACKAGE_VERSION" > "${WESAY_CONFIG_DIR}/wesay-version"
fi

