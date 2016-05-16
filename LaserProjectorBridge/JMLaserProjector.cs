using System.Collections.Generic;
using System.Text;

namespace LaserProjectorBridge
{
    public class JMLaserProjector
    {
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   <fields>   <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        static int s_number_of_projectors_ = NativeMethods.JMLaser.JMLASER_ERROR_NOT_ENUMERATED;
        uint projector_list_entry_index_;
        int projector_handle_;
        int projector_maximum_number_of_vectors_per_frame_;
        int projector_minimum_speed_;
        int projector_maximum_speed_;
        int projector_speed_step_;
        string projector_network_address_;
        string projector_name_;
        string projector_name_from_handle_;
        string projector_friendly_name_;
        string projector_family_name_;
        bool projector_output_started_;
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   </fields>   <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   <constructors-destructor>   <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        public JMLaserProjector()
        {
            projector_list_entry_index_ = 0;
            projector_handle_ = -1;
            projector_maximum_number_of_vectors_per_frame_ = -1;
            projector_minimum_speed_ = -1;
            projector_maximum_speed_ = -1;
            projector_speed_step_ = -1;
            projector_output_started_ = false;
            jmLaserBridgeEnumerateDevices();
        }

        ~JMLaserProjector()
        {
            resetProjector();
        }


        public void resetProjector()
        {
            if (projector_output_started_)
            {
                stopOutput();
                projector_output_started_ = false;
            }

            if (projector_handle_ >= 0)
            {
                closeProjector();
                projector_handle_ = -1;
            }

            projector_list_entry_index_ = 0;
            projector_maximum_number_of_vectors_per_frame_ = -1;
            projector_minimum_speed_ = -1;
            projector_maximum_speed_ = -1;
            projector_speed_step_ = -1;
            projector_network_address_ = "";
            projector_name_ = "";
            projector_name_from_handle_ = "";
            projector_friendly_name_ = "";
            projector_family_name_ = "";
        }
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   </constructors-destructor>  <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   <static functions>   <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
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
            if (s_number_of_projectors_ <= 0)
            {
                s_number_of_projectors_ = NativeMethods.JMLaser.jmLaserEnumerateDevices();
            }
            else {
                s_number_of_projectors_ = jmLaserBridgeGetDeviceListLength();
            }
            return s_number_of_projectors_;
        }

        public static string jmLaserBridgeGetDeviceListEntry(uint list_index)
        {
            if (jmLaserBridgeEnumerateDevices() <= 0) { return ""; }
            int device_name_length = NativeMethods.JMLaser.jmLaserGetDeviceListEntryLength(list_index);
            if (device_name_length == NativeMethods.JMLaser.JMLASER_ERROR_NOT_ENUMERATED)
            {
                if (jmLaserBridgeEnumerateDevices() <= 0) { return ""; }
                device_name_length = NativeMethods.JMLaser.jmLaserGetDeviceListEntryLength(list_index);
            }
            if (device_name_length > 0)
            {
                StringBuilder device_name = new StringBuilder(device_name_length);
                if (NativeMethods.JMLaser.jmLaserGetDeviceListEntry(list_index, device_name, (uint) device_name_length) == 0) {
                    return device_name.ToString();
                }
            }
            return "";
        }

        public static string jmLaserBridgeGetDeviceName(int projector_handle)
        {
            if (projector_handle < 0) { return ""; }
            int device_name_from_handle_length = NativeMethods.JMLaser.jmLaserGetDeviceNameLength(projector_handle);
            if (device_name_from_handle_length > 0)
            {
                StringBuilder device_name_from_handle = new StringBuilder(device_name_from_handle_length);
                if (NativeMethods.JMLaser.jmLaserGetDeviceName(projector_handle, device_name_from_handle, (uint) device_name_from_handle_length) == 0) {
                    return device_name_from_handle.ToString();
                }
            }
            return "";
        }

        public static string jmLaserBridgeGetDeviceFamilyName(string projector_name)
        {
            if (projector_name.Length == 0) { return ""; }
            if (jmLaserBridgeEnumerateDevices() <= 0) { return ""; }
            int device_family_name_length = NativeMethods.JMLaser.jmLaserGetDeviceFamilyNameLength(projector_name);
            if (device_family_name_length > 0)
            {
                StringBuilder device_family_name = new StringBuilder(device_family_name_length);
                if (NativeMethods.JMLaser.jmLaserGetDeviceFamilyName(projector_name, device_family_name, (uint) device_family_name_length) == 0) {
                    return device_family_name.ToString();
                }
            }
            return "";
        }

        public static string jmLaserBridgeGetFriendlyName(string projector_name)
        {
            if (projector_name.Length == 0) { return ""; }
            if (jmLaserBridgeEnumerateDevices() <= 0) { return ""; }
            int device_friendly_name_length = NativeMethods.JMLaser.jmLaserGetFriendlyNameLength(projector_name);
            if (device_friendly_name_length > 0)
            {
                StringBuilder device_friendly_name = new StringBuilder(device_friendly_name_length);
                if (NativeMethods.JMLaser.jmLaserGetFriendlyName(projector_name, device_friendly_name, (uint) device_friendly_name_length) == 0) {
                    return device_friendly_name.ToString();
                }
            }
            return "";
        }

        public static bool jmLaserBridgeSetFriendlyName(int projector_handle, string projector_friendly_name)
        {
            if (projector_handle < 0 || projector_friendly_name.Length == 0) { return false; }
            if (jmLaserBridgeEnumerateDevices() <= 0) { return false; }
            return (NativeMethods.JMLaser.jmLaserSetFriendlyName(projector_handle, projector_friendly_name) == 0);
        }

        public static int jmLaserBridgeOpenDevice(string projector_name)
        {
            if (projector_name.Length == 0) { return 0; }
            if (jmLaserBridgeEnumerateDevices() <= 0) { return 0; }
            return NativeMethods.JMLaser.jmLaserOpenDevice(projector_name);
        }

        public static int jmLaserBridgeGetMaxFrameSize(int projector_handle)
        {
            return NativeMethods.JMLaser.jmLaserGetMaxFrameSize(projector_handle);
        }

        public static int jmLaserBridgeGetDeviceListLength()
        {
            if (s_number_of_projectors_ <= 0)
            {
                s_number_of_projectors_ = NativeMethods.JMLaser.jmLaserEnumerateDevices();
            }
            return NativeMethods.JMLaser.jmLaserGetDeviceListLength();
        }

        public static bool jmLaserBridgeGetIsNetworkDevice(string projector_name)
        {
            if (projector_name.Length == 0) { return false; }
            if (jmLaserBridgeEnumerateDevices() <= 0) { return false; }

            return (NativeMethods.JMLaser.jmLaserGetIsNetworkDevice(projector_name) == 1);
        }

        public static string jmLaserBridgeGetNetworkAddress(string projector_name)
        {
            if (projector_name.Length == 0) { return ""; }
            if (jmLaserBridgeEnumerateDevices() <= 0) { return ""; }
            int network_address_length = NativeMethods.JMLaser.jmLaserGetNetworkAddressLength(projector_name);
            if (network_address_length > 0)
            {
                StringBuilder network_address = new StringBuilder(network_address_length);
                if (NativeMethods.JMLaser.jmLaserGetNetworkAddress(projector_name, network_address, (uint) network_address_length) == 0) {
                    return network_address.ToString();
                }
            }
            return "";
        }

        public static int jmLaserBridgeGetMinSpeed(int projector_handle)
        {
            return NativeMethods.JMLaser.jmLaserGetMinSpeed(projector_handle);
        }

        public static int jmLaserBridgeGetMaxSpeed(int projector_handle)
        {
            return NativeMethods.JMLaser.jmLaserGetMaxSpeed(projector_handle);
        }

        public static int jmLaserBridgeGetSpeedStep(int projector_handle)
        {
            return NativeMethods.JMLaser.jmLaserGetSpeedStep(projector_handle);
        }

        public static NativeMethods.JMLaser.JMVectorStruct createSingleColorLaserPoint(int x_val, int y_val, ushort intensity_val)
        {
            return new NativeMethods.JMLaser.JMVectorStruct() { x=x_val, y=y_val, r=0, g=0, b=0, i=intensity_val, deepblue=0, yellow=0, cyan=0, user4=0};
        }
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   <static functions/>  <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   <methods>  <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        public bool setupProjector()
        {
            s_number_of_projectors_ = jmLaserBridgeGetDeviceListLength();
            if (s_number_of_projectors_ > 0)
            {
                return setupProjectorUsingIndex(0);
            }
            return false;
        }

        public bool setupProjectorUsingName(string projector_name)
        {
            if (projector_name.Length == 0) { return false; }
            s_number_of_projectors_ = jmLaserBridgeGetDeviceListLength();
            if (s_number_of_projectors_ > 0)
            {
                for (int i = 0; i < s_number_of_projectors_; ++i)
                {
                    if (jmLaserBridgeGetDeviceListEntry((uint)i) == projector_name)
                    {
                        return setupProjectorUsingIndex((uint)i);
                    }
                }
            }
            return false;
        }

        public bool setupProjectorUsingFriendlyName(string projector_friendly_name)
        {
            if (projector_friendly_name.Length == 0) { return false; }
            s_number_of_projectors_ = jmLaserBridgeGetDeviceListLength();
            if (s_number_of_projectors_ > 0)
            {
                for (int i = 0; i < s_number_of_projectors_; ++i)
                {
                    string projector_name = jmLaserBridgeGetDeviceListEntry((uint)i);
                    if (jmLaserBridgeGetFriendlyName(projector_name) == projector_friendly_name)
                    {
                        return setupProjectorUsingIndex((uint)i);
                    }
                }
            }
            return false;
        }

        public bool setupProjectorUsingIndex(uint projector_index)
        {
            if (projector_index < 0) { return false; }
            resetProjector();
            projector_list_entry_index_ = projector_index;
            projector_name_ = jmLaserBridgeGetDeviceListEntry(projector_index);
            if (projector_name_.Length > 0)
            {
                projector_friendly_name_ = jmLaserBridgeGetFriendlyName(projector_name_);
                projector_family_name_ = jmLaserBridgeGetDeviceFamilyName(projector_name_);
                projector_handle_ = jmLaserBridgeOpenDevice(projector_name_);

                if (projector_handle_ >= 0)
                {
                    projector_name_from_handle_ = jmLaserBridgeGetDeviceName(projector_handle_);
                    projector_maximum_number_of_vectors_per_frame_ = jmLaserBridgeGetMaxFrameSize(projector_handle_);
                    if (jmLaserBridgeGetIsNetworkDevice(projector_name_))
                    {
                        projector_network_address_ = jmLaserBridgeGetNetworkAddress(projector_name_);
                    }
                    projector_minimum_speed_ = jmLaserBridgeGetMinSpeed(projector_handle_);
                    projector_maximum_speed_ = jmLaserBridgeGetMaxSpeed(projector_handle_);
                    projector_speed_step_ = jmLaserBridgeGetSpeedStep(projector_handle_);
                    if (projector_maximum_number_of_vectors_per_frame_ > 0 && projector_minimum_speed_ >= 0 && projector_maximum_speed_ > 0 && projector_speed_step_ > 0) { return true; }
                }
            }
            resetProjector();
            return false;
        }

        public bool closeProjector()
        {
            if (projector_handle_ >= 0)
            {
                if (NativeMethods.JMLaser.jmLaserCloseDevice(projector_handle_) == 0)
                {
                    projector_handle_ = -1;
                    return true;
                }
            }
            return false;
        }


        public bool setProjectorFriendlyName(string projector_friendly_name)
        {
            if (projector_friendly_name.Length > 0 && projector_handle_ >= 0)
            {
                if (jmLaserBridgeSetFriendlyName(projector_handle_, projector_friendly_name))
                {
                    projector_friendly_name_ = jmLaserBridgeGetFriendlyName(projector_name_);
                    return projector_friendly_name_.Length > 0;
                }
            }
            return false;
        }


        public bool startOutput()
        {
            if (projector_handle_ >= 0)
            {
                if (NativeMethods.JMLaser.jmLaserStartOutput(projector_handle_) == 0)
                {
                    projector_output_started_ = true;
                    return true;
                }
            }
            return false;
        }

        public bool sendVectorImageToProjector(ref List<NativeMethods.JMLaser.JMVectorStruct> points, uint speed, uint repetitions)
        {
            if (projector_handle_ >= 0 && points.Count > 0 /*&& speed >= projector_minimum_speed_*/ && speed <= projector_maximum_speed_ && repetitions >= 0 && projector_maximum_number_of_vectors_per_frame_ > 0)
            {
                if (points.Count > projector_maximum_number_of_vectors_per_frame_)
                {
                    points.RemoveRange(projector_maximum_number_of_vectors_per_frame_, points.Count - projector_maximum_number_of_vectors_per_frame_);
                }
                int wait_status = NativeMethods.JMLaser.jmLaserWaitForDeviceReady(projector_handle_);
                if (wait_status == NativeMethods.JMLaser.JMLASER_ERROR_OUTPUT_NOT_STARTED)
                {
                    if (startOutput())
                    {
                        wait_status = NativeMethods.JMLaser.jmLaserWaitForDeviceReady(projector_handle_);
                    }
                    else {
                        return false;
                    }
                }

                if (wait_status == NativeMethods.JMLaser.JMLASER_DEVICE_READY)
                {
                    int write_status = NativeMethods.JMLaser.jmLaserWriteFrame(projector_handle_, points.ToArray(), (uint)points.Count, speed, repetitions);
                    return (write_status == 0);
                }
            }
            return false;
        }

        public bool stopOutput()
        {
            if (projector_handle_ >= 0 && projector_output_started_)
            {
                if (NativeMethods.JMLaser.jmLaserStopOutput(projector_handle_) == 0)
                {
                    projector_output_started_ = false;
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("############################## JMLaserProjector ###############################\n");
            str.Append("# number_of_projectors:\t\t\t" + s_number_of_projectors_ + "\n");
            str.Append("# projector_list_entry_index:\t\t" + projector_list_entry_index_ + "\n");
            str.Append("# projector_handle:\t\t\t" + projector_handle_ + "\n");
            str.Append("# maximum_number_of_vectors_per_frame:\t" + projector_maximum_number_of_vectors_per_frame_ + "\n");
            str.Append("# minimum_projection_speed:\t\t" + projector_minimum_speed_ + "\n");
            str.Append("# maximum_projection_speed:\t\t" + projector_maximum_speed_ + "\n");
            str.Append("# projection_speed_step:\t\t" + projector_speed_step_ + "\n");
            str.Append("# network_address:\t\t\t" + projector_network_address_ + "\n");
            str.Append("# projector_name:\t\t\t" + projector_name_ + "\n");
            str.Append("# projector_name_from_handle:\t\t" + projector_name_from_handle_ + "\n");
            str.Append("# projector_friendly_name:\t\t" + projector_friendly_name_ + "\n");
            str.Append("# projector_family_name:\t\t" + projector_family_name_ + "\n");
            str.Append("# projector_output_started:\t\t" + projector_output_started_ + "\n");
            str.Append("###############################################################################\n");
            return str.ToString();
        }
        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   <methods/>  <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    }
}
