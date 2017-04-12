ECHO OFF
ECHO Building nuget package...
nuget pack LaserProjectorBridge.csproj -Prop Configuration=Release -Symbols -IncludeReferencedProjects
ECHO Finished building nuget package.
PAUSE
