nuget restore
nuget install EntityFramework -OutputDirectory packages
msbuild /t:Clean /p:configuration=debug
msbuild /t:Clean /p:configuration=release
msbuild /t:Rebuild /p:configuration=debug
msbuild /t:Rebuild /p:configuration=release
