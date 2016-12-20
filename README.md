# laser_projector_bridge_csharp
C# library for communicating with Ethernet laser projectors (such as MediaLas ILP 622 LAN using the jmlaser.dll).

## Notes

By default the library is built using the "Any CPU" platform and uses the 32 bit jmlaser.dll.

If your application is 64 bit, replace the 32 bit jmlaser.dll with the jmlaser_x64.dll in your application build directory (the jmlaser_x64.dll must be renamed to jmlaser.dll in order to DllImport find the dll).
