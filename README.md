# laser_projector_bridge_csharp
C# library for communicating with Ethernet laser projectors (such as MediaLas ILP 622 LAN).

NuGet package available [here](https://www.nuget.org/packages/LaserProjectorBridge).


## Notes

By default the library is built using the "Any CPU" platform and uses the 64 bit [jmlaser.dll](LaserProjectorBridge/jmlaser.dll?raw=true).

If your application is 32 bit, replace the 64 bit [jmlaser.dll](LaserProjectorBridge/jmlaser.dll?raw=true) with the [jmlaser_x86.dll](LaserProjectorBridge/jmlaser_x86.dll?raw=true) in your application build directory (the jmlaser_x86.dll must be renamed to jmlaser.dll in order to DllImport find the dll).
