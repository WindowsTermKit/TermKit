@echo off

rem Set the environment variables.
call "C:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\vcvarsall.bat" x86
cd "C:\Server Storage\Projects\TermKit\Win32"

rem Build the solution.
msbuild /t:Rebuild /fileLogger /fileLoggerParameters:LogFile=ScheduledBuild.log

rem Pack and upload the results.
cd "C:\Server Storage\Projects\TermKit\Win32\bin\Debug\Win32"
"..\..\..\tools\ScheduledUpload\bin\x86\Debug\Nightly Upload Tool.exe"