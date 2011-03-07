#this is tuned to john's machine, checked in case it helps someone else.

#Note, this will update all the assemblyInfo.cs's, which should not be checked in

#Before this will work, install nant, msbuild, msbuild community tasks, & wix.
#I also had to manually copy "Windows Installer XML v3"  from its installed area
#(under progfiles x86) to the normal (64 bit) progfiles, because there was
# a hard-code reference to WIX at that location in one of these build files

# other build targets:
# clean (dies if VisualStudio has wesay open with vshost.exe)
# test

SET BUILD_NUMBER=0-0-0
c:\dev\nant\bin\nant -buildfile:bld/wesay.build build installer /D:configuration=release
