@echo off
REM ============================
REM Windows version of build script
REM
REM * 1 - downloads latest version of paket using paket.bootstrapper.exe
REM * 2 - uses paket to restore all nuget packages using paket.dependencies and packet.lock in the root directory
REM * 3 - runs FAKE on the build script to build the project and run the tests
REM ============================


cls

.paket\paket.bootstrapper.exe
if errorlevel 1 (
  exit /b %errorlevel%
)

.paket\paket.exe restore
if errorlevel 1 (
  exit /b %errorlevel%
)

packages\FAKE\tools\FAKE.exe build.fsx %*
