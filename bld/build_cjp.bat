@ECHO OFF
set COMPLUS_Version=v2.0.50727
msbuild /target:test /property:NUnitPath="c:\program files\nunit 2.4\bin" bld\WeSay.proj
