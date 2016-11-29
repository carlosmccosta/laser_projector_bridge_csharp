# laser_projector_bridge_csharp
C# library for communicating with Ethernet laser projectors (such as MediaLas ILP 622 LAN using the jmlaser.dll).

To run the library tests from the command line:
- cd to LaserProjectorBridgeTests folder
- dnx run

Note:
- In order to DllImport be able to load the jmlaser.dll it is necessary to copy it to a folder that is in the PATH (such as C:\Windows\SysWOW64) or add the LaserProjectorBridge/lib/ folder to the PATH
- The dnx executable folder should also be in the PATH (default is C:\Users\\[USER_NAME]\\.dnx\runtimes\dnx-clr-win-x86.1.0.0-rc1-update1\bin)
