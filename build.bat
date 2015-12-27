@echo off
set "picklesVersion=2.0.4"

cls

"packages\nuget\NuGet.exe" "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"
"packages\nuget\NuGet.exe" "Restore" "src\Pickles\Pickles.sln"

"packages\FAKE\tools\Fake.exe" build.fsx --envvar version %picklesVersion%
"packages\FAKE\tools\Fake.exe" nuget.fsx --envvar version %picklesVersion%
"packages\FAKE\tools\Fake.exe" chocolatey.fsx --envvar version %picklesVersion%

call unzip.cmd %picklesVersion%

FOR %%A IN (testRunnerCmd testRunnerMsBuild testRunnerPowerShell) DO (
call %%A.cmd %picklesVersion%
if errorlevel 1 goto handleerror1orhigher
)


@ECHO all fine
goto end

:handleerror1orhigher

@ECHO Something went wrong!
goto end

:end
