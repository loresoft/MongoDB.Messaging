@echo off
NuGet.exe update "Source\MongoDB.Messaging.sln" -r "Source\packages"
msbuild master.proj /t:refresh