ECHO OFF
ECHO Building nuget package...
nuget pack LaserProjectorBridge.csproj -Prop Configuration=Debug -IncludeReferencedProjects
ECHO Finished building nuget package.
PAUSE
