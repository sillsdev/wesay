This file contains information that may be useful to anyone wanting to work on
WeSay development.  Much of this information is Linux specific, but some of it
applies to both Linux and Windows development.

In this document, `$wesay` refers to the directory of the wesay repository 
(e.g. `~/src/wesay`).


0. UBUNTU DISTRIBUTION PACKAGES NEEDED

You will need a few system packages installed.

	sudo apt-get install libicu-dev curl


1. OBTAINING WESAY SOURCES

The basic WeSay repository is at github, and can be cloned as follows:

	git clone git://github.com/sillsdev/wesay.git


2. SIL LINUX PACKAGES NEEDED

SIL has Linux (Ubuntu/Debian) package repositories that are used to distribute
software that are either original with us or specialized for our use.  There
are two basic repositories, often referred to as PSO or PSO-experimental.  Two
packages need to be installed from these repositories: geckofx29 and
xulrunner-geckofx29 (which is a dependency of geckofx29).

The following commands will add the repositories to your apt setup and then
download and install the packages.  (You need to add the second repository only
if the first repository doesn't work.)

	sudo add-apt-repository 'deb http://packages.sil.org/ubuntu precise main'
	sudo add-apt-repository 'deb http://packages.sil.org/ubuntu precise-experimental main'
	wget -O - http://packages.sil.org/sil.gpg | sudo apt-key add -
	sudo apt-get update
	sudo apt-get install geckofx29

If you are not running precise on your machine, change precise and
precise-experimental to match your actual distribution.  If you are running
debian instead of ubuntu, then substitute accordingly.  (In that case, I don't
think there's a distinction of -experimental.)


3. NEED A PATCHED LINUX MONO

Unfortunately the stock Mono has a number of bugs that we have found and fixed
over the years.  Many of these fixes appear in newer versions of Mono. (Indeed,
some of the fixes are cherry-picks from later versions of the standard Mono.)
But enough fixes are specific to our work that we've been using a custom
version of Mono in FieldWorks development for years.  In order to benefit from
this work, the same version of Mono will be used for WeSay as well in the
future.

There are two ways to obtain this patched version of Mono: build it or install
the packages for it.  (Real programmers use the first method, since that allows
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
information developed by SIL, largely by the Linux team based in Calgary.  After
cloning these repositories, then the patched mono can be built and installed to
/opt/mono-sil by the following command (assuming these five repositories are all
cloned with their default names and the current directory is still the common
parent):

	mono-calgary/BuildOptMono.sh

This will prompt you for your password to enable some sudo commands in the
script.  Depending on how fast your computer is, it may take 15 minutes to an
hour to finish compiling and installing everything, and it may prompt you more
than once for your password.

If you don't want to build Mono for yourself (coward!!), then you can install
the following packages from PSO (or PSO-experimental as the case may be):

	mono-sil
	mono-basic-sil
	gtk-sharp2-sil
	libgdiplus-sil

These packages install everything into the same place, /opt/mono-sil.  I
haven't actually tested whether installing works as well as building.


4. FIX LOCAL REPO FOR GERRIT CODE REVIEW

Here are some instructions to get us all on the same page about pushing and
reviews.  This applies to both Windows and Linux.  (SOME OF THE DETAILS MAY
CHANGE SOON.)  The various commands and scripts should be executed in a git
bash window for Windows.  On Linux, any normal terminal window will do.

Make sure you have a Gerrit account. For details on getting a Gerrit account,
see

	https://github.com/sillsdev/FwDocumentation/wiki/Getting-Started-for-Core-Developers

Even though the explanation involves FieldWorks, steps 1-3 (Register, Request,
Upload) apply to WeSay developers as well. Make a note of your gerrit username,
which is referred to as $username below. You can find it in the Profile tab of
your settings on the gerrit site (gerrit.lsdev.sil.org).

Clone the repository with helpful scripts for setting up gerrit code reviews:

	git clone git://github.com/sillsdev/gerrit-support.git

Move to the wesay repository and execute a script in the gerrit-support repo
something like this (use a git bash window in Windows).  Note that $wesay here
and below is shorthand for wherever the wesay repository is located.

	cd $wesay
	../gerrit-support/configure-for-gerrit ssh://$username@gerrit.lsdev.sil.org:59418/wesay.git

(This assumes that the wesay and gerrit-support repositories are installed
side-by-side with their default names.)  After this has executed, when you've
made one or more commits on the develop branch, you can just push them:

	git push

These will show up as a review item in the gerrit web UI. We all need to be
checking gerrit frequently so we can review other people's changes. However,
for faster service you can use the UI to add a specific person as reviewer to
your changes, and they will get an email requesting a review. Once approved,
changes can be merged onto develop by pressing the "Submit" button in the web
UI.

If you're working on something major, you can use a feature branch to keep your
work separate from develop. This allows you to switch back temporarily to a
"vanilla" develop branch if you need to test or fix something, and then resume
work on your feature afterwards. You'll likely end up with a sequence of
several commits that implement the feature. It would be nice to mark these
commits as being related, and this is done with extended "magic branch" syntax:

	git push origin feature/whizbang:refs/for/develop/feature/whizbang

The part before the colon is the local branch name, and the part after the
colon is the remote branch it will be pushed to. If there are several new
commits on the local branch, they will all show up in gerrit, as separate
review items all sharing the same "topic".


5. BUILDING WESAY

Before you can build wesay, a number of dependencies must be downloaded from
the internet.

For Windows, execute (in a git bash window) the script:

	build/buildupdate.win.sh

For Linux, execute the script:

	build/buildupdate.mono.sh

After all the dependencies have been downloaded, the simplest way to compile
wesay is to use a batch file that calls msbuild.exe (on Windows) or a shell
script that calls xbuild (on Linux).  For example, in a Windows "DOS box"
window,

	cd $wesay\build
	TestBuild.bat

On linux, the operation is similar:

	cd $wesay/build
	./TestBuild.sh

Without any arguments, these scripts perform a Release build, placing the
result in $wesay/output/Release.  If you prefer a Debug build, then add Debug
as a command line argument to TestBuild.bat or TestBuild.sh.  The result would
then appear in $wesay/output/Debug.


6. USING MONODEVELOP ON LINUX

In Windows, WeSay can be debugged using Visual Studio 2010, and probably Visual
Studio Express 2010.  For Linux, debugging requires using MonoDevelop, the
standard Mono IDE.  You have a choice of using the standard MonoDevelop that
comes with the system, which for Ubuntu/Precise was version 2.8, or using a
newer version installed into /opt.  The newer version can be installed as
follows:

	sudo add-apt-repository ppa:ermshiperete/monodevelop
	sudo apt-get update
	sudo apt-get install monodevelop-current

In either version of MonoDevelop, you can choose which Mono runtime to use for
a project, and you can add new Mono runtimes to the known list.  The Edit menu
has a Preferences command near the bottom.  Click on that and choose the ".NET
Runtimes" panel under Projects.  Click on the Add button and use the file
chooser to find /opt/mono-sil.  Once you choose that, you should see "Mono
2.10.9.1 (develop/xxxxxxx)(/opt/mono-sil)" as a possible choice.  Click on that
item to select it, then click on "Set as Default".  (of course, only if you do
want it as the default...)  After loading the WeSay solution (or creating your
own solution to run WeSay if you like to be tricky), double check the runtime
with the "Active Runtime" command in the Project menu.

One more step is essential to actually get WeSay to run properly with the Mono
runtime in /opt/mono-sil.  Under the Project menu, select the "<ProjectName>
Options" command.  This brings up the Project Options dialog. Choose the
General pane under Run and add the environment variable MONO_ENVIRON with the
proper value for your installation.  This would be the full path to your wesay
repository with "/environ" appended to it.  For example, the setting might be
something like this:

	MONO_ENVIRON	   /home/steve/sillsdev/wesay/environ

Be sure to click elsewhere in the dialog before clicking on the OK button.  One
of MonoDevelop's aggravating "features" is that clicking OK does not by itself
save the most recent edit.  But losing focus to elsewhere in the same dialog
does save the edit.  But enough editorializing...


7. RUNNING UNIT TESTS

People have had varying success in running unit tests from inside MonoDevelop.
Someone who has had success will have to describe how they did it and the rest
of us will have to verify whether it's a general solution.

The project files in the $wesay/build folder have targets that will run all of
the unit tests.  Once everything has been built with the Build or Compile
target (which is what the TestBuild files described above do), then unit tests
can be run by using the LocalTest target.  For Windows, the command line would
look something like this:

	cd $wesay\build
	msbuild.exe /t:LocalTest /p:RootDir=.. /p:Configuration=Release build.win.proj

For Linux, it would be similar (adjust the setting for MONO_ENVIRON):

	cd $wesay
	export MONO_ENVIRON=$PWD/environ
	cd build
	/opt/mono-sil/bin/xbuild /t:LocalTest /p:RootDir=.. /p:Configuration=Release build.mono.proj


8. OTHER LINUX PACKAGES NEEDED

For developers, there are a few dependencies that may need to be installed
manually.  (These should be installed automatically for users by the Linux
package system.)  Here are the packages that we are aware of (in command line
format):

	sudo apt-get install chmsee

9. TESTING IN VAGRANT VIRTUAL MACHINE

Vagrant is a tool to "Create and configure lightweight, reproducible, and portable
development environments."  There are pre-packaged base images for Precise64 and Wasta64
distributions using VirtualBox.  These work on Linux, Windows, and Mac.  To install on
Linux from the command-line:

	sudo apt-get install vagrant

To install on Windows or Mac, download the installer from www.vagrantup.com/downloads.html.
After installation, install the required plugins from the command-line on all platforms:

	vagrant plugin install vagrant-cachier
	vagrant plugin install vagrant-vbguest

The pre-packaged base images do not have dependencies installed.  The provisioning script
(bootstrap.sh) is executed once when the VM is started and will install all the dependencies
needed to run a local build of WeSay executables.  The first time the VM is started, it
will have to download the required packages and then they are cached for subsequent runs.
Run these commands from the command-line in the host (use the appropriate directory separator):

	cd $wesay/vagrant
	vagrant up precise64 (or wasta)

The status output of the provisioning (bootstrap.sh) will display in the terminal console that
executed the vagrant up command. When it is completed, it will have mounted $wesay to /wesay
inside the VM.  If the host and VM with have the same line endings (e.g. Ubuntu Saucy host
and Ubuntu Precise VM), the /wesay directory can be used directly within the VM.
Run these commands from the command-line in the VM:

	cd /wesay
	. environ
	mono output/Release/WeSay.App.exe

When testing is complete, clean up the virtual machine by running these commands from the
command-line of the host machine:

	cd $wesay/vagrant
	vagrant destroy precise64 (or wasta)

Run 'vagrant -h' from the command-line for other commands to halt, suspend, resume, etc or
refer to the documentation at http://vagrantup.com

9.1 WINDOWS HOST WITH LINUX VAGRANT VM

If the host and VM have different line endings (e.g. Windows host and Ubuntu Precise VM),
then the mapped drive within the VM will have files with non-native line endings.  To work
around this, setup a development directory inside the VM.  There is a script in the $wesay/vagrant
directory to setup a clone in the ~/src/wesay directory referencing the git repo in the host.
Run these commands from the command-line inside the VM:

	cd /wesay/vagrant
	./setup-vm-wesay-dev.sh
	cd ~/src/wesay
	build/buildupdate.mono.sh
	build/TestBuild.sh
	. environ
	mono output/Release/WeSay.App.exe

9.2 TESTING TEAMCITY CONFIGURATIONS

To test changes in TeamCity configuration and build/buildupdate.*.sh, it is best to use a clean
environment.  There is a script in the $wesay/vagrant directory to install buildupdate to enable
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

10.0 BUILDING FOR WINDOWS INSTALLER

To set up a windows box for developing WeSay you need to install the following tools:

	VS 2010, (Express works OK)
	WIX 3.5  (Works fine)
	Git-1.9.x (See a helpful configuration below for as you install it)
		=> you can grab this from: http://msysgit.github.io
		- Select Components Dialog =>
			Additional Icons (if you want them)
			Windows Explorer integration (simple, check all)
			Otherwise stick with the defaults
		- Adjusting your PATH environment Dialog =>
			Radio button recommendation:
				Run Git and included Unix tools from the Windows Command Prompt
				(If you don't want that option, then select: "Run Git from the Windows Command Prompt")
		- Configuring the line ending conversions Dialog =>
			Radio button recommendation:
				Checkout as-is, commit Unix-style line endings

Configure the following environment variables:

	http_proxy	(if you need one)
	path		make sure that you include these (ymmv):
				C:\Program Files\Windows Installer XML v3.5\bin
					(If 64bit, use: C:\Program Files (x86)\...)
				C:\Windows\Microsoft.NET\Framework\v4.0.30319

For a nice alternative graphical git tool,
	Sourcetree at: www.sourcetreeap.com, unfortunately not available for Linux yet :(
	giteye at: www.collab.net/giteyeapp
	Smartgit at: www.syntevo.com/smartgithg/

Some other helpful developer editing, or troubleshooting tools:

	Notepad++			(for source code editing, see: notepad-plus-plus.org)
	JetBrains dotPeek   (for decompiling .NET assemblies, see: www.jetbrains.com/decompiler)
	Process Explorer	(for looking at which dlls are loaded, and for looking at which files are opened or loaded,
						see: technet.microsoft.com/en-ca/sysinternals/bb896653.aspx, or just DuckDuckGo for it.)
	depends				(for troubleshooting system errors in loading and executing modules, see: dependencywalker.com)

To build for installer, just type in the windows command prompt:
	`buildupdate.win.sh -f`				(to do a "full" update the libraries needed to build WeSay)
										- If you have set up your bash commands not to work within
										the windows command prompt as above, you will need to
										execute this command in a bash window.)
	`TestBuild.bat Release Installer`	(to build an Release version of Installer)
