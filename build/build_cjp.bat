@ECHO OFF
set BUILD_NUMBER=0.1.2.abcd
set teamcity_build_checkoutDir=C:\src\sil\wesay
msbuild %*
