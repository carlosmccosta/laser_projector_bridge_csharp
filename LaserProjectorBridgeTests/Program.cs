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

            TestJMLaserProjectorWithoutEnumeration();

            int numberOfProjectors = LaserProjectorBridge.JMLaserProjector.jmLaserBridgeEnumerateDevices();
            if (numberOfProjectors <= 0)
            {
                Console.WriteLine(">>> No projectors were found!");
                return -1;
            }
            else {
                //TestJMLaserProjectorSetup(number_of_projectors);
                TestJMLaserOutput(numberOfProjectors);
            }

            LaserProjectorBridge.JMLaserProjector.jmLaserBridgeCloseDll();
            return 0;
        }


        public static void TestJMLaserProjectorWithoutEnumeration()
        {
            string projectorName = LaserProjectorBridge.JMLaserProjector.jmLaserBridgeGetDeviceListEntry(0);
            projectorName = LaserProjectorBridge.JMLaserProjector.jmLaserBridgeGetDeviceListEntry(0);
            Console.WriteLine("Projector name: " + projectorName);
        }


        public static void TestJMLaserProjectorSetup(int numberOfProjectors)
        {
            for (int i = 0; i < numberOfProjectors; ++i)
            {
                LaserProjectorBridge.JMLaserProjector laserProjector = new LaserProjectorBridge.JMLaserProjector();

                Console.WriteLine(">>> |setupProjector()");
                laserProjector.SetupProjector();
                Console.WriteLine(laserProjector + "\n\n");

                //string friendly_name = laser_projector.getProjectorFriendlyName();
                //laser_projector.setProjectorFriendlyName("Test Name");

                Console.WriteLine(">>> |setupProjectorUsingIndex(" + i + ")");
                laserProjector.SetupProjectorUsingIndex((uint)i);
                Console.WriteLine(laserProjector + "\n\n");

                //laser_projector.setProjectorFriendlyName(friendly_name);

                Console.WriteLine(">>> |setupProjectorUsingName(string(\"Netlase 1552 #0\"))");
                laserProjector.SetupProjectorUsingName("Netlase 1552 #0");
                Console.WriteLine(laserProjector + "\n\n");

                Console.WriteLine(">>> |setupProjectorUsingFriendlyName(string(\"ILP 622 LAN\"))");
                laserProjector.SetupProjectorUsingFriendlyName("ILP 622 LAN");
                Console.WriteLine(laserProjector + "\n\n");
            }
        }


        public static void CreateLaserOutputPatternSquareFullRange(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue, Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, 0));
        }


        public static void CreateLaserOutputPatternSquareScaled(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points, double positionScale = 0.9)
        {
            ushort intensity = (ushort)(UInt16.MaxValue * 0.5);
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * positionScale), (int)(Int32.MinValue * positionScale), 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * positionScale), (int)(Int32.MinValue * positionScale), intensity));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MaxValue * positionScale), (int)(Int32.MinValue * positionScale), intensity));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MaxValue * positionScale), (int)(Int32.MaxValue * positionScale), intensity));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * positionScale), (int)(Int32.MaxValue * positionScale), intensity));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * positionScale), (int)(Int32.MinValue * positionScale), intensity));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * positionScale), (int)(Int32.MinValue * positionScale), 0));
        }


        public static void CreateLaserOutputPatternPlusFullRange(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0,                 Int32.MinValue,     0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0,                 Int32.MinValue,     UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0,                 Int32.MaxValue,     UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue,    0,                  0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue,    0,                  UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue,    0,                  UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue,    0,                  0));
        }


        public static void CreateLaserOutputPatternCrossFullRange(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue,        Int32.MinValue,     0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue,        Int32.MinValue,     UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue / 2,    Int32.MinValue / 2, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0,                     0,                  UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue / 2,    Int32.MaxValue / 2, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue,        Int32.MaxValue,     UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue,        Int32.MinValue,     0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue,        Int32.MinValue,     UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue / 2,    Int32.MinValue / 2, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0,                     0,                  UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue / 2,    Int32.MaxValue / 2, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue,        Int32.MaxValue,     UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue,        Int32.MaxValue,     0));
        }


        public static void CreateLaserOutputPatternCrossScaled(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            double position_scale = 0.75;
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * position_scale),    (int)(Int32.MinValue * position_scale), 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * position_scale),    (int)(Int32.MinValue * position_scale), UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0,                                         0,                                      UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MaxValue * position_scale),    (int)(Int32.MaxValue * position_scale), UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MaxValue * position_scale),    (int)(Int32.MaxValue * position_scale), 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MaxValue * position_scale),    (int)(Int32.MinValue * position_scale), 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MaxValue * position_scale),    (int)(Int32.MinValue * position_scale), UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0,                                         0,                                      UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * position_scale),    (int)(Int32.MaxValue * position_scale), UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * position_scale),    (int)(Int32.MaxValue * position_scale), 0));
        }


        public static void CreateLaserOutputPattern(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            CreateLaserOutputPatternSquareFullRange(ref points);
            CreateLaserOutputPatternSquareScaled(ref points, 0.75);
            CreateLaserOutputPatternSquareScaled(ref points, 0.6);
            CreateLaserOutputPatternSquareScaled(ref points, 0.5);
            CreateLaserOutputPatternSquareScaled(ref points, 0.4);
            CreateLaserOutputPatternSquareScaled(ref points, 0.25);
            CreateLaserOutputPatternPlusFullRange(ref points);
            CreateLaserOutputPatternCrossFullRange(ref points);
            CreateLaserOutputPatternCrossScaled(ref points);
        }


        public static void TestJMLaserOutput(int numberOfProjectors)
        {
            List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points = new List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct>();
            CreateLaserOutputPattern(ref points);

            for (int i = 0; i < numberOfProjectors; ++i)
            {
                LaserProjectorBridge.JMLaserProjector laserProjector = new LaserProjectorBridge.JMLaserProjector();
                Console.WriteLine(">>> |setupProjectorUsingIndex(" + i + ")");
                laserProjector.SetupProjectorUsingIndex((uint)i);
                Console.WriteLine(laserProjector + "\n\n");
                laserProjector.StartOutput();
                Console.WriteLine(">>> Sending pattern to projector " + i);
                if (laserProjector.SendVectorImageToProjector(ref points, 500, 0))
                {
                    Console.WriteLine(">>> - Pattern was sent successfully");
                }
                else {
                    Console.WriteLine(">>> - Failed to send pattern");
                }
                Console.WriteLine(">>> - Press ENTER to continue...");
                Console.ReadLine();
                laserProjector.StopOutput();
            }
        }
    }
}
