# Readme

Installers can be found on the [product website](http://software.sil.org/wesay/).

## Readme for Developers

This file contains information that may be useful to anyone wanting to work on
WeSay development.  Much of this information is Linux specific, but some of it
applies to both Linux and Windows development.

In this document, `$wesay` refers to the directory of the wesay repository
(e.g. `~/src/wesay`).

### UBUNTU DISTRIBUTION PACKAGES NEEDED

You will need a few system packages installed.

	sudo apt-get install libicu-dev curl

Install mono v6, such as by installing package `mono-complete` from the repo
specified at https://www.mono-project.com/download/stable/ .

### OBTAINING WESAY SOURCES

The basic WeSay repository is at github, and can be cloned as follows:

	git clone git://github.com/sillsdev/wesay.git

### NEED A PATCHED LINUX MONO

Linux WeSay 1.6 used mono4-sil. WeSay on branch develop can build with an
unmodified mono 6.12.0.122 and msbuild 16.6.0, but it may be that a custom mono
(such as mono5-sil) may provide benefit.

Historically, the stock Mono had a number of bugs that we have found and fixed
over the years.  Many of these fixes appear in newer versions of Mono. (Indeed,
some of the fixes are cherry-picks from later versions of the standard Mono.)
But enough fixes are specific to our work that we've been using a custom
version of Mono in FieldWorks development for years.  In order to benefit from
this work, the same version of Mono will be used for WeSay as well in the
future.

There are two ways to obtain this patched version of Mono: build it or install
the packages for it.  (Using the first method allows
for the possibility of adding your very own bugfixes to the mix!)

Five separate repositories contain the necessary code for building the custom
version of Mono.  These should all be installed side-by-side.

	git clone git://github.com/sillsdev/mono.git
	git clone git://github.com/sillsdev/mono-basic.git
	git clone git://github.com/sillsdev/gtk-sharp.git
	git clone git://github.com/sillsdev/libgdiplus.git
	git clone git://github.com/sillsdev/mono-calgary.git

The first four repositories contain the actual source code cloned from the Mono
project.  The fifth repository contains the build scripts and packaging
information developed by SIL. After
cloning these repositories, then the patched mono can be built and installed to
`/opt/mono5-sil` by the following command (assuming these five repositories are
all cloned with their default names and the current directory is still the
common parent):

	mono-calgary/BuildOptMono.sh

This will prompt you for your password to enable some sudo commands in the
script.  Depending on how fast your computer is, it may take 15 minutes to an
hour to finish compiling and installing everything, and it may prompt you more
than once for your password.

If you don't want to build Mono for yourself, then you can install
the following packages from PSO (see https://packages.sil.org/ ):

	mono5-sil
	gtk-sharp5-sil
	libgdiplus5-sil

These packages install everything into the same place, `/opt/mono5-sil`.

### BUILDING WESAY

#### Install dependencies

Linux:

```bash
wget --output-document=build/nuget.exe https://dist.nuget.org/win-x86-commandline/v6.0.0/nuget.exe
mono build/nuget.exe restore src/WeSay.sln
```

Windows: `nuget restore src/WeSay.sln`

#### Build

```bash
msbuild src/WeSay.sln
```

#### Test

```bash
msbuild build/WeSay.proj -t:RestoreBuildTasks
msbuild build/WeSay.proj -t:TestOnly -p:Configuration=Debug -p:Platform="x86"
```

#### Test installation

```bash
rm -rf test_install &&
  mkdir test_install &&
  BUILD_CONFIG=Release \
    INSTALLATION_PREFIX="$(pwd)/test_install" \
    MONO_PREFIX=/usr \
    make build-deps build-app install &&
  MONO_PREFIX=/usr WESAY_PREFIX="$(pwd)/test_install" test_install/bin/wesay
```

### USING MONODEVELOP ON LINUX

In Windows, WeSay can be debugged using Visual Studio Community 2015.
For Linux, debugging requires using MonoDevelop, the standard Mono IDE.
You have a choice of using the standard MonoDevelop that comes with the
system, which for Ubuntu/Precise was version 2.8, or using a newer version
installed into `/opt`.  The newer version can be installed as follows:

	sudo add-apt-repository ppa:ermshiperete/monodevelop
	sudo apt-get update
	sudo apt-get install monodevelop-current

In either version of MonoDevelop, you can choose which Mono runtime to use for
a project, and you can add new Mono runtimes to the known list.  The Edit menu
has a Preferences command near the bottom.  Click on that and choose the ".NET
Runtimes" panel under Projects.  Click on the Add button and use the file
chooser to find `/opt/mono5-sil`.  Once you choose that, you should see
"Mono (/opt/mono5-sil)" as a possible choice.  Click on that
item to select it, then click on "Set as Default".  (of course, only if you do
want it as the default...)  After loading the WeSay solution (or creating your
own solution to run WeSay if you like to be tricky), double check the runtime
with the "Active Runtime" command in the Project menu.

One more step is essential to actually get WeSay to run properly with the Mono
runtime in `/opt/mono5-sil`.  Under the Project menu, select the "_ProjectName_
Options" command.  This brings up the Project Options dialog. Choose the
General pane under Run and add the environment variable MONO_ENVIRON with the
proper value for your installation.  This would be the full path to your wesay
repository with "/environ" appended to it.  For example, the setting might be
something like this:

	MONO_ENVIRON	   /home/user/sillsdev/wesay/environ

Be sure to click elsewhere in the dialog before clicking on the OK button.  One
of MonoDevelop's aggravating "features" is that clicking OK does not by itself
save the most recent edit.  But losing focus to elsewhere in the same dialog
does save the edit.  But enough editorializing...

### RUNNING UNIT TESTS

People have had varying success in running unit tests from inside MonoDevelop.
Someone who has had success will have to describe how they did it and the rest
of us will have to verify whether it's a general solution.

The project files in the $wesay/build folder have targets that will run all of
the unit tests.  Once everything has been built with the Build or Compile
target (which is what the TestBuild files described above do), then unit tests
can be run by using the TestOnly target.  For Windows, the command line would
look something like this:

	cd $wesay\build
	msbuild.exe /t:TestOnly /p:RootDir=.. /p:Configuration=Release build.win.proj

For Linux, it would be similar (adjust the setting for MONO_ENVIRON):

	cd $wesay
	export MONO_ENVIRON=$PWD/environ
	cd build
	/opt/mono-sil/bin/xbuild /t:TestOnly /p:RootDir=.. /p:Configuration=Release build.mono.proj

### OTHER LINUX PACKAGES NEEDED

For developers, there are a few dependencies that may need to be installed
manually.  (These should be installed automatically for users by the Linux
package system.)  Here are the packages that we are aware of (in command line
format):

	sudo apt-get install chmsee

### TESTING IN VAGRANT VIRTUAL MACHINE

Vagrant is a tool to "Create and configure lightweight, reproducible, and portable
development environments."  There are pre-packaged base images for Precise64 and Wasta64
distributions using VirtualBox.  These work on Linux, Windows, and Mac.  To install on
Linux from the command-line:

	sudo apt-get install vagrant

To install on Windows or Mac, download the [installer](www.vagrantup.com/downloads.html).
After installation, install the required plugins from the command-line on all platforms:

	vagrant plugin install vagrant-cachier
	vagrant plugin install vagrant-vbguest

The pre-packaged base images do not have dependencies installed.  The provisioning script
(`bootstrap.sh`) is executed once when the VM is started and will install all the dependencies
needed to run a local build of WeSay executables.  The first time the VM is started, it
will have to download the required packages and then they are cached for subsequent runs.
Run these commands from the command-line in the host (use the appropriate directory separator):

	cd $wesay/vagrant
	vagrant up precise64 (or wasta)

The status output of the provisioning (`bootstrap.sh`) will display in the terminal console that
executed the vagrant up command. When it is completed, it will have mounted `$wesay` to `/wesay`
inside the VM.  If the host and VM with have the same line endings (e.g. Ubuntu Saucy host
and Ubuntu Precise VM), the `/wesay` directory can be used directly within the VM.
Run these commands from the command-line in the VM:

	cd /wesay
	. environ
	mono output/Release/WeSay.App.exe

When testing is complete, clean up the virtual machine by running these commands from the
command-line of the host machine:

	cd $wesay/vagrant
	vagrant destroy precise64 (or wasta)

Run `vagrant -h` from the command-line for other commands to halt, suspend, resume, etc or
refer to the [documentation](http://vagrantup.com).

#### WINDOWS HOST WITH LINUX VAGRANT VM

If the host and VM have different line endings (e.g. Windows host and Ubuntu Precise VM),
then the mapped drive within the VM will have files with non-native line endings.  To work
around this, setup a development directory inside the VM.  There is a script in the `$wesay/vagrant`
directory to setup a clone in the `~/src/wesay` directory referencing the git repo in the host.
Run these commands from the command-line inside the VM:

	cd /wesay/vagrant
	./setup-vm-wesay-dev.sh
	cd ~/src/wesay
	build/buildupdate.mono.sh
	build/TestBuild.sh
	. environ
	mono output/Release/WeSay.App.exe

#### TESTING TEAMCITY CONFIGURATIONS

To test changes in TeamCity configuration and `build/buildupdate.*.sh`, it is best to use a clean
environment.  There is a script in the `$wesay/vagrant` directory to install `buildupdate` to enable
updating the buildupdate scripts.  A clean development directory should be setup first.
Run these commands from the command-line inside the VM:

	cd /wesay/vagrant
	./setup-vm-wesay-dev.sh
	sudo ./setup-buildupdate.sh
	cd ~/src/wesay
	../Buildupdate/buildupdate.rb -f build/buildupdate.mono.sh
	build/buildupdate.mono.sh
	build/TestBuild.sh
	. environ
	mono output/Release/WeSay.App.exe

### BUILDING FOR WINDOWS INSTALLER

To set up a windows box for developing WeSay you need to install the following tools:

- VS 2010, (Express works OK)
- WIX 3.5  (Works fine)
- Git-1.9.x (See a helpful configuration below for as you install it) =>
  you can grab this from: [msysgit.github.io](https://git-for-windows.github.io/)
	- Select Components Dialog
		- Additional Icons (if you want them)
		- Windows Explorer integration (simple, check all)
		- Otherwise stick with the defaults
	- Adjusting your PATH environment Dialog
		- Radio button recommendation:
			- Run Git and included Unix tools from the Windows Command Prompt
				(If you don't want that option, then select: "Run Git from the Windows Command Prompt")
	- Configuring the line ending conversions Dialog
		- Radio button recommendation:
			- Checkout as-is, commit Unix-style line endings

Configure the following environment variables:

|||
|-----------|-------------------|
|http_proxy |(if you need one)|
|path       |make sure that you include these (ymmv): `C:\Program Files\Windows Installer XML v3.5\bin` (if 64bit, use: `C:\Program Files (x86)\...`) as well as `C:\Windows\Microsoft.NET\Framework\v4.0.30319`

For a nice alternative graphical git tool,

- [Sourcetree](www.sourcetreeap.com) - unfortunately not available for Linux yet :(
- [giteye](www.collab.net/giteyeapp)
- [Smartgit](www.syntevo.com/smartgithg/)

Some other helpful developer editing, or troubleshooting tools:

- [Notepad++](notepad-plus-plus.org) - for source code editing
- [JetBrains dotPeek](www.jetbrains.com/decompiler) - for decompiling .NET assemblies
- [Process Explorer](technet.microsoft.com/en-ca/sysinternals/bb896653.aspx) -
  for looking at which dlls are loaded, and for looking at which files are opened or loaded
- [depends](dependencywalker.com) - for troubleshooting system errors in loading and executing modules

To build for installer, just type in the windows command prompt:

	buildupdate.win.sh -f
	TestBuild.bat Release Installer

`buildupdate.win.sh` does a "full" update the libraries needed to
 build WeSay. `TestBuild.bat` builds a Release version of the Installer.

**NOTE:** If you have set up your bash commands not to work within
										the windows command prompt as above, you will need to
execute `buildupdate.win.sh` command in a bash window.
