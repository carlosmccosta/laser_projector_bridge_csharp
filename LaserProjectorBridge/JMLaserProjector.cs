using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserProjectorBridge
{
    public class JMLaserProjector : IDisposable
    {
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   <fields>   <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        #region fields
        public static int NumberOfProjectors { get; set; } = NativeMethods.JMLaser.JMLASER_ERROR_NOT_ENUMERATED;
        public uint ProjectorListEntryIndex { get; set; }
        public int ProjectorHandle { get; set; }
        public int ProjectorMaximumNumberOfVectorsPerFrame { get; set; }
        public int ProjectorMinimumSpeed { get; set; }
        public int ProjectorMaximumSpeed { get; set; }
        public int ProjectorSpeedStep { get; set; }
        public string ProjectorNetworkAddress { get; set; }
        public string ProjectorName { get; set; }
        public string ProjectorNameFromHandle { get; set; }
        public string ProjectorFriendlyName { get; set; }
        public string ProjectorFamilyName { get; set; }
        public bool ProjectorOutputStarted { get; set; }

        private static readonly Lazy<JMLaserProjector> SingletonLazyJMLaserProjector = new Lazy<JMLaserProjector>(() => new JMLaserProjector());
        #endregion
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   </fields>   <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   <constructors-destructor>   <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        #region constructors-destructor
        public JMLaserProjector()
        {
            ProjectorListEntryIndex = 0;
            ProjectorHandle = -1;
            ProjectorMaximumNumberOfVectorsPerFrame = -1;
            ProjectorMinimumSpeed = -1;
            ProjectorMaximumSpeed = -1;
            ProjectorSpeedStep = -1;
            ProjectorOutputStarted = false;
            jmLaserBridgeEnumerateDevices();
        }

        public static JMLaserProjector Instance => SingletonLazyJMLaserProjector.Value;

        private void ReleaseUnmanagedResources()
        {
            ResetProjector();
        }

        ~JMLaserProjector()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
        }


        public void ResetProjector()
        {
            if (ProjectorOutputStarted)
            {
                StopOutput();
                ProjectorOutputStarted = false;
            }

            if (ProjectorHandle >= 0)
            {
                CloseProjector();
                ProjectorHandle = -1;
            }

            ProjectorListEntryIndex = 0;
            ProjectorMaximumNumberOfVectorsPerFrame = -1;
            ProjectorMinimumSpeed = -1;
            ProjectorMaximumSpeed = -1;
            ProjectorSpeedStep = -1;
            ProjectorNetworkAddress = "";
            ProjectorName = "";
            ProjectorNameFromHandle = "";
            ProjectorFriendlyName = "";
            ProjectorFamilyName = "";
        }
        #endregion
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   </constructors-destructor>  <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   <static functions>   <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        #region static functions
        public static int jmLaserBridgeOpenDll()
        {
            return NativeMethods.JMLaser.jmLaserOpenDll();
        }


        public static int jmLaserBridgeCloseDll()
        {
            return NativeMethods.JMLaser.jmLaserCloseDll();
        }

        public static int jmLaserBridgeEnumerateDevices()
        {
            NumberOfProjectors = NumberOfProjectors <= 0 ? NativeMethods.JMLaser.jmLaserEnumerateDevices() : jmLaserBridgeGetDeviceListLength();
            return NumberOfProjectors;
        }

        public static string jmLaserBridgeGetDeviceListEntry(uint list_index)
        {
            if (jmLaserBridgeEnumerateDevices() <= 0) { return ""; }
            int deviceNameLength = NativeMethods.JMLaser.jmLaserGetDeviceListEntryLength(list_index);
            if (deviceNameLength == NativeMethods.JMLaser.JMLASER_ERROR_NOT_ENUMERATED)
            {
                if (jmLaserBridgeEnumerateDevices() <= 0) { return ""; }
                deviceNameLength = NativeMethods.JMLaser.jmLaserGetDeviceListEntryLength(list_index);
            }
            if (deviceNameLength > 0)
            {
                StringBuilder deviceName = new StringBuilder(deviceNameLength);
                if (NativeMethods.JMLaser.jmLaserGetDeviceListEntry(list_index, deviceName, (uint) deviceNameLength) == 0) {
                    return deviceName.ToString();
                }
            }
            return "";
        }

        public static string jmLaserBridgeGetDeviceName(int projectorHandle)
        {
            if (projectorHandle < 0) { return ""; }
            int deviceNameFromHandleLength = NativeMethods.JMLaser.jmLaserGetDeviceNameLength(projectorHandle);
            if (deviceNameFromHandleLength > 0)
            {
                StringBuilder deviceNameFromHandle = new StringBuilder(deviceNameFromHandleLength);
                if (NativeMethods.JMLaser.jmLaserGetDeviceName(projectorHandle, deviceNameFromHandle, (uint) deviceNameFromHandleLength) == 0) {
                    return deviceNameFromHandle.ToString();
                }
            }
            return "";
        }

        public static string jmLaserBridgeGetDeviceFamilyName(string projectorName)
        {
            if (projectorName.Length == 0) { return ""; }
            if (jmLaserBridgeEnumerateDevices() <= 0) { return ""; }
            int deviceFamilyNameLength = NativeMethods.JMLaser.jmLaserGetDeviceFamilyNameLength(projectorName);
            if (deviceFamilyNameLength > 0)
            {
                StringBuilder deviceFamilyName = new StringBuilder(deviceFamilyNameLength);
                if (NativeMethods.JMLaser.jmLaserGetDeviceFamilyName(projectorName, deviceFamilyName, (uint) deviceFamilyNameLength) == 0) {
                    return deviceFamilyName.ToString();
                }
            }
            return "";
        }

        public static string jmLaserBridgeGetFriendlyName(string projectorName)
        {
            if (projectorName.Length == 0) { return ""; }
            if (jmLaserBridgeEnumerateDevices() <= 0) { return ""; }
            int deviceFriendlyNameLength = NativeMethods.JMLaser.jmLaserGetFriendlyNameLength(projectorName);
            if (deviceFriendlyNameLength > 0)
            {
                StringBuilder deviceFriendlyName = new StringBuilder(deviceFriendlyNameLength);
                if (NativeMethods.JMLaser.jmLaserGetFriendlyName(projectorName, deviceFriendlyName, (uint) deviceFriendlyNameLength) == 0) {
                    return deviceFriendlyName.ToString();
                }
            }
            return "";
        }

        public static bool jmLaserBridgeSetFriendlyName(int projectorHandle, string projectorFriendlyName)
        {
            if (projectorHandle < 0 || projectorFriendlyName.Length == 0) { return false; }
            if (jmLaserBridgeEnumerateDevices() <= 0) { return false; }
            return (NativeMethods.JMLaser.jmLaserSetFriendlyName(projectorHandle, projectorFriendlyName) == 0);
        }

        public static int jmLaserBridgeOpenDevice(string projectorName)
        {
            if (projectorName.Length == 0) { return 0; }
            if (jmLaserBridgeEnumerateDevices() <= 0) { return 0; }
            int openStatus = NativeMethods.JMLaser.jmLaserOpenDevice(projectorName);
            if (openStatus == NativeMethods.JMLaser.JMLASER_ERROR_NOT_ENUMERATED)
            {
                if (NativeMethods.JMLaser.jmLaserEnumerateDevices() <= 0) { return 0; }
                openStatus = NativeMethods.JMLaser.jmLaserOpenDevice(projectorName);
            }
            return openStatus;
        }

        public static int jmLaserBridgeGetMaxFrameSize(int projectorHandle)
        {
            return NativeMethods.JMLaser.jmLaserGetMaxFrameSize(projectorHandle);
        }

        public static int jmLaserBridgeGetDeviceListLength()
        {
            if (NumberOfProjectors <= 0)
            {
                NumberOfProjectors = NativeMethods.JMLaser.jmLaserEnumerateDevices();
            }
            return NativeMethods.JMLaser.jmLaserGetDeviceListLength();
        }

        public static bool jmLaserBridgeGetIsNetworkDevice(string projectorName)
        {
            if (projectorName.Length == 0) { return false; }
            if (jmLaserBridgeEnumerateDevices() <= 0) { return false; }

            return (NativeMethods.JMLaser.jmLaserGetIsNetworkDevice(projectorName) == 1);
        }

        public static string jmLaserBridgeGetNetworkAddress(string projectorName)
        {
            if (projectorName.Length == 0) { return ""; }
            if (jmLaserBridgeEnumerateDevices() <= 0) { return ""; }
            int networkAddressLength = NativeMethods.JMLaser.jmLaserGetNetworkAddressLength(projectorName);
            if (networkAddressLength > 0)
            {
                StringBuilder networkAddress = new StringBuilder(networkAddressLength);
                if (NativeMethods.JMLaser.jmLaserGetNetworkAddress(projectorName, networkAddress, (uint) networkAddressLength) == 0) {
                    return networkAddress.ToString();
                }
            }
            return "";
        }

        public static int jmLaserBridgeGetMinSpeed(int projectorHandle)
        {
            return NativeMethods.JMLaser.jmLaserGetMinSpeed(projectorHandle);
        }

        public static int jmLaserBridgeGetMaxSpeed(int projectorHandle)
        {
            return NativeMethods.JMLaser.jmLaserGetMaxSpeed(projectorHandle);
        }

        public static int jmLaserBridgeGetSpeedStep(int projectorHandle)
        {
            return NativeMethods.JMLaser.jmLaserGetSpeedStep(projectorHandle);
        }

        public static NativeMethods.JMLaser.JMVectorStruct CreateSingleColorLaserPoint(int xVal, int yVal, ushort intensityVal)
        {
            return new NativeMethods.JMLaser.JMVectorStruct() { x=xVal, y=yVal, r=0, g=0, b=0, i=intensityVal, deepblue=0, yellow=0, cyan=0, user4=0};
        }
        #endregion
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   <static functions/>  <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   <methods>  <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        #region methods
        public bool SetupProjector()
        {
            NumberOfProjectors = jmLaserBridgeGetDeviceListLength();
            if (NumberOfProjectors > 0)
            {
                return SetupProjectorUsingIndex(0);
            }
            return false;
        }

        public bool SetupProjectorUsingName(string projectorName)
        {
            if (projectorName.Length == 0) { return false; }
            NumberOfProjectors = jmLaserBridgeGetDeviceListLength();
            if (NumberOfProjectors > 0)
            {
                for (int i = 0; i < NumberOfProjectors; ++i)
                {
                    if (jmLaserBridgeGetDeviceListEntry((uint)i) == projectorName)
                    {
                        return SetupProjectorUsingIndex((uint)i);
                    }
                }
            }
            return false;
        }

        public bool SetupProjectorUsingFriendlyName(string projectorFriendlyName)
        {
            if (projectorFriendlyName.Length == 0) { return false; }
            NumberOfProjectors = jmLaserBridgeGetDeviceListLength();
            if (NumberOfProjectors > 0)
            {
                for (int i = 0; i < NumberOfProjectors; ++i)
                {
                    string projectorName = jmLaserBridgeGetDeviceListEntry((uint)i);
                    if (jmLaserBridgeGetFriendlyName(projectorName) == projectorFriendlyName)
                    {
                        return SetupProjectorUsingIndex((uint)i);
                    }
                }
            }
            return false;
        }

        public bool SetupProjectorUsingIndex(uint projectorIndex)
        {
            ResetProjector();
            ProjectorListEntryIndex = projectorIndex;
            ProjectorName = jmLaserBridgeGetDeviceListEntry(projectorIndex);
            if (ProjectorName.Length > 0)
            {
                ProjectorFriendlyName = jmLaserBridgeGetFriendlyName(ProjectorName);
                ProjectorFamilyName = jmLaserBridgeGetDeviceFamilyName(ProjectorName);
                ProjectorHandle = jmLaserBridgeOpenDevice(ProjectorName);

                if (ProjectorHandle >= 0)
                {
                    ProjectorNameFromHandle = jmLaserBridgeGetDeviceName(ProjectorHandle);
                    ProjectorMaximumNumberOfVectorsPerFrame = jmLaserBridgeGetMaxFrameSize(ProjectorHandle);
                    if (jmLaserBridgeGetIsNetworkDevice(ProjectorName))
                    {
                        ProjectorNetworkAddress = jmLaserBridgeGetNetworkAddress(ProjectorName);
                    }
                    ProjectorMinimumSpeed = jmLaserBridgeGetMinSpeed(ProjectorHandle);
                    ProjectorMaximumSpeed = jmLaserBridgeGetMaxSpeed(ProjectorHandle);
                    ProjectorSpeedStep = jmLaserBridgeGetSpeedStep(ProjectorHandle);
                    if (ProjectorMaximumNumberOfVectorsPerFrame > 0 && ProjectorMinimumSpeed >= 0 && ProjectorMaximumSpeed > 0 && ProjectorSpeedStep > 0) { return true; }
                }
            }
            ResetProjector();
            return false;
        }

        public bool CloseProjector()
        {
            if (ProjectorHandle >= 0)
            {
                int status = NativeMethods.JMLaser.jmLaserCloseDevice(ProjectorHandle);
                if (status == 0)
                {
                    ProjectorHandle = -1;
                    return true;
                }
            }
            return false;
        }


        public bool SetProjectorFriendlyName(string projectorFriendlyName)
        {
            if (projectorFriendlyName.Length > 0 && ProjectorHandle >= 0 && jmLaserBridgeSetFriendlyName(ProjectorHandle, projectorFriendlyName))
            {
                ProjectorFriendlyName = jmLaserBridgeGetFriendlyName(ProjectorName);
                return ProjectorFriendlyName.Length > 0;
            }
            return false;
        }


        public bool StartOutput()
        {
            if (ProjectorHandle >= 0 && NativeMethods.JMLaser.jmLaserStartOutput(ProjectorHandle) == 0)
            {
                ProjectorOutputStarted = true;
                return true;
            }
            return false;
        }

        public bool SendVectorImageToProjector(ref List<NativeMethods.JMLaser.JMVectorStruct> points, uint speed = 42000, uint repetitions = 0, bool addReverseImage = false)
        {
            if (speed < ProjectorMinimumSpeed) { speed = (uint)ProjectorMinimumSpeed; }
            if (speed > ProjectorMaximumSpeed) { speed = (uint)ProjectorMaximumSpeed; }

            if (ProjectorHandle >= 0 && points.Count > 0 && ProjectorMaximumNumberOfVectorsPerFrame > 0)
            {
                if (points.Count > ProjectorMaximumNumberOfVectorsPerFrame)
                {
                    points.RemoveRange(ProjectorMaximumNumberOfVectorsPerFrame - 1, points.Count - ProjectorMaximumNumberOfVectorsPerFrame + 1);
                    NativeMethods.JMLaser.JMVectorStruct lastPointOff = points.Last();
                    lastPointOff.i = 0;
                    points.Add(lastPointOff);
                }

                if (addReverseImage && points.Count * 2 <= ProjectorMaximumNumberOfVectorsPerFrame)
                {
                    for (int i = points.Count - 1; i >= 0; --i)
                    {
                        points.Add(points[i]);
                    }
                }

                int waitStatus = NativeMethods.JMLaser.jmLaserWaitForDeviceReady(ProjectorHandle);
                if (waitStatus == NativeMethods.JMLaser.JMLASER_ERROR_OUTPUT_NOT_STARTED)
                {
                    if (StartOutput())
                    {
                        waitStatus = NativeMethods.JMLaser.jmLaserWaitForDeviceReady(ProjectorHandle);
                    }
                    else {
                        return false;
                    }
                }

                if (waitStatus == NativeMethods.JMLaser.JMLASER_DEVICE_READY)
                {
                    int writeStatus = NativeMethods.JMLaser.jmLaserWriteFrame(ProjectorHandle, points.ToArray(), (uint)points.Count, speed, repetitions);
                    return (writeStatus == 0);
                }
            }
            return false;
        }

        public bool StopOutput()
        {
            if (ProjectorHandle >= 0 && ProjectorOutputStarted)
            {
                var vectorImage = new List<NativeMethods.JMLaser.JMVectorStruct>();
                vectorImage.Add(new NativeMethods.JMLaser.JMVectorStruct());
                SendVectorImageToProjector(ref vectorImage, (uint)ProjectorMinimumSpeed);
                if (NativeMethods.JMLaser.jmLaserStopOutput(ProjectorHandle) == 0)
                {
                    ProjectorOutputStarted = false;
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("############################## JMLaserProjector ###############################\n");
            str.Append("# NumberOfProjectors:\t\t\t\t" + NumberOfProjectors + "\n");
            str.Append("# ProjectorListEntryIndex:\t\t\t" + ProjectorListEntryIndex + "\n");
            str.Append("# ProjectorHandle:\t\t\t\t" + ProjectorHandle + "\n");
            str.Append("# ProjectorMaximumNumberOfVectorsPerFrame:\t" + ProjectorMaximumNumberOfVectorsPerFrame + "\n");
            str.Append("# ProjectorMinimumSpeed:\t\t\t" + ProjectorMinimumSpeed + "\n");
            str.Append("# ProjectorMaximumSpeed:\t\t\t" + ProjectorMaximumSpeed + "\n");
            str.Append("# ProjectorSpeedStep:\t\t\t\t" + ProjectorSpeedStep + "\n");
            str.Append("# ProjectorNetworkAddress:\t\t\t" + ProjectorNetworkAddress + "\n");
            str.Append("# ProjectorName:\t\t\t\t" + ProjectorName + "\n");
            str.Append("# ProjectorNameFromHandle:\t\t\t" + ProjectorNameFromHandle + "\n");
            str.Append("# ProjectorFriendlyName:\t\t\t" + ProjectorFriendlyName + "\n");
            str.Append("# ProjectorFamilyName:\t\t\t\t" + ProjectorFamilyName + "\n");
            str.Append("# ProjectorOutputStarted:\t\t\t" + ProjectorOutputStarted + "\n");
            str.Append("###############################################################################\n");
            return str.ToString();
        }
        #endregion
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   <methods/>  <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    }
}
