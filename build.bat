cls 
@echo off 

set cache=%cd%\Dependency Cache
set final=%cd%\Build

if exist "%cache%" (
    rd /s /q "%cache%"
)

if not exist "%cache%" (
    mkdir "%cache% "
)

echo Building Itami...
msbuild.exe "%cd%\Itami\Itami.csproj" /p:Configuration=Release /property:Platform=x64 /p:OutputPath="%cache%" /p:AllowUnsafeBlocks=true

Echo Obfuscating...
set profiles=%cd%\Obfuscation Profiles
"%cd%\Confuser\Confuser.CLI.exe" "%profiles%\confuser_core.crproj"

if exist "%final%" (
    rd /s /q "%final%"
)

if not exist "%final%" (
    mkdir "%final%"
)

copy "%cache%\Confused\Clarity.exe" "%final%"

pause