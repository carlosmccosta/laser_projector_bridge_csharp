using System;
using System.Collections.Generic;

namespace LaserProjectorBridgeTests
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("===============================================================================");
            Console.WriteLine(">>> Testing LaserProjectorBridge.JMLaserProjector");
            Console.WriteLine("===============================================================================\n");

            LaserProjectorBridge.JMLaserProjector.jmLaserBridgeOpenDll();

            test_jmlaser_projector_without_enumeration();

            int number_of_projectors = LaserProjectorBridge.JMLaserProjector.jmLaserBridgeEnumerateDevices();
            if (number_of_projectors <= 0)
            {
                Console.WriteLine(">>> No projectors were found!");
                return -1;
            }
            else {
                //test_jmlaser_projector_setup(number_of_projectors);
                test_jmlaser_output(number_of_projectors);
            }

            LaserProjectorBridge.JMLaserProjector.jmLaserBridgeCloseDll();
            return 0;
        }


        public static void test_jmlaser_projector_without_enumeration()
        {
            string projector_name = LaserProjectorBridge.JMLaserProjector.jmLaserBridgeGetDeviceListEntry(0);
            projector_name = LaserProjectorBridge.JMLaserProjector.jmLaserBridgeGetDeviceListEntry(0);
            Console.WriteLine("Projector name: " + projector_name);
        }


        public static void test_jmlaser_projector_setup(int number_of_projectors)
        {
            for (int i = 0; i < number_of_projectors; ++i)
            {
                LaserProjectorBridge.JMLaserProjector laser_projector = new LaserProjectorBridge.JMLaserProjector();

                Console.WriteLine(">>> |setupProjector()");
                laser_projector.setupProjector();
                Console.WriteLine(laser_projector + "\n\n");

                //string friendly_name = laser_projector.getProjectorFriendlyName();
                //laser_projector.setProjectorFriendlyName("Test Name");

                Console.WriteLine(">>> |setupProjectorUsingIndex(" + i + ")");
                laser_projector.setupProjectorUsingIndex((uint)i);
                Console.WriteLine(laser_projector + "\n\n");

                //laser_projector.setProjectorFriendlyName(friendly_name);

                Console.WriteLine(">>> |setupProjectorUsingName(string(\"Netlase 1552 #0\"))");
                laser_projector.setupProjectorUsingName("Netlase 1552 #0");
                Console.WriteLine(laser_projector + "\n\n");

                Console.WriteLine(">>> |setupProjectorUsingFriendlyName(string(\"ILP 622 LAN\"))");
                laser_projector.setupProjectorUsingFriendlyName("ILP 622 LAN");
                Console.WriteLine(laser_projector + "\n\n");
            }
        }


        public static void create_laser_output_pattern_square_full_range(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MaxValue, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MaxValue, Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MinValue, Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, 0));
        }


        public static void create_laser_output_pattern_square_scaled(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points, double position_scale = 0.9)
        {
            ushort intensity = (ushort)(UInt16.MaxValue * 0.5);
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint((int)(Int32.MinValue * position_scale), (int)(Int32.MinValue * position_scale), 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint((int)(Int32.MinValue * position_scale), (int)(Int32.MinValue * position_scale), intensity));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint((int)(Int32.MaxValue * position_scale), (int)(Int32.MinValue * position_scale), intensity));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint((int)(Int32.MaxValue * position_scale), (int)(Int32.MaxValue * position_scale), intensity));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint((int)(Int32.MinValue * position_scale), (int)(Int32.MaxValue * position_scale), intensity));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint((int)(Int32.MinValue * position_scale), (int)(Int32.MinValue * position_scale), intensity));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint((int)(Int32.MinValue * position_scale), (int)(Int32.MinValue * position_scale), 0));
        }


        public static void create_laser_output_pattern_plus_full_range(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(0, Int32.MinValue, 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(0, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(0, Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MinValue, 0, 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MinValue, 0, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MaxValue, 0, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MaxValue, 0, 0));
        }


        public static void create_laser_output_pattern_cross_full_range(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MinValue / 2, Int32.MinValue / 2, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(0, 0, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MaxValue / 2, Int32.MaxValue / 2, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MaxValue, Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MaxValue, Int32.MinValue, 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MaxValue, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MaxValue / 2, Int32.MinValue / 2, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(0, 0, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MinValue / 2, Int32.MaxValue / 2, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MinValue, Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(Int32.MinValue, Int32.MaxValue, 0));
        }


        public static void create_laser_output_pattern_cross_scaled(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            double position_scale = 0.75;
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint((int)(Int32.MinValue * position_scale), (int)(Int32.MinValue * position_scale), 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint((int)(Int32.MinValue * position_scale), (int)(Int32.MinValue * position_scale), UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(0, 0, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint((int)(Int32.MaxValue * position_scale), (int)(Int32.MaxValue * position_scale), UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint((int)(Int32.MaxValue * position_scale), (int)(Int32.MaxValue * position_scale), 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint((int)(Int32.MaxValue * position_scale), (int)(Int32.MinValue * position_scale), 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint((int)(Int32.MaxValue * position_scale), (int)(Int32.MinValue * position_scale), UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint(0, 0, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint((int)(Int32.MinValue * position_scale), (int)(Int32.MaxValue * position_scale), UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.createSingleColorLaserPoint((int)(Int32.MinValue * position_scale), (int)(Int32.MaxValue * position_scale), 0));
        }


        public static void create_laser_output_pattern(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            create_laser_output_pattern_square_full_range(ref points);
            create_laser_output_pattern_square_scaled(ref points, 0.75);
            create_laser_output_pattern_square_scaled(ref points, 0.6);
            create_laser_output_pattern_square_scaled(ref points, 0.5);
            create_laser_output_pattern_square_scaled(ref points, 0.4);
            create_laser_output_pattern_square_scaled(ref points, 0.25);
            create_laser_output_pattern_plus_full_range(ref points);
            create_laser_output_pattern_cross_full_range(ref points);
            create_laser_output_pattern_cross_scaled(ref points);
        }


        public static void test_jmlaser_output(int number_of_projectors)
        {
            List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points = new List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct>();
            create_laser_output_pattern(ref points);

            for (int i = 0; i < number_of_projectors; ++i)
            {
                LaserProjectorBridge.JMLaserProjector laser_projector = new LaserProjectorBridge.JMLaserProjector();
                Console.WriteLine(">>> |setupProjectorUsingIndex(" + i + ")");
                laser_projector.setupProjectorUsingIndex((uint)i);
                Console.WriteLine(laser_projector + "\n\n");
                laser_projector.startOutput();
                Console.WriteLine(">>> Sending pattern to projector " + i);
                if (laser_projector.sendVectorImageToProjector(ref points, 500, 0))
                {
                    Console.WriteLine(">>> - Pattern was sent successfully");
                }
                else {
                    Console.WriteLine(">>> - Failed to send pattern");
                }
                Console.WriteLine(">>> - Press ENTER to continue...");
                Console.ReadLine();
                laser_projector.stopOutput();
            }
        }
    }
}
