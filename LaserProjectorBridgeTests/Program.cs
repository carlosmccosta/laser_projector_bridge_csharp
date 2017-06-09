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
            else
            {
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

        public static void CreateLaserOutputPattern(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            LaserProjectorBridge.PatternBuilder.CreateSquareFullRange(ref points);
            LaserProjectorBridge.PatternBuilder.CreateSquareScaled(ref points, 0.75);
            LaserProjectorBridge.PatternBuilder.CreateSquareScaled(ref points, 0.6);
            LaserProjectorBridge.PatternBuilder.CreateSquareScaled(ref points, 0.5);
            LaserProjectorBridge.PatternBuilder.CreateSquareScaled(ref points, 0.4);
            LaserProjectorBridge.PatternBuilder.CreateSquareScaled(ref points, 0.25);
            LaserProjectorBridge.PatternBuilder.CreatePlusFullRange(ref points);
            LaserProjectorBridge.PatternBuilder.CreateCrossFullRange(ref points);
            LaserProjectorBridge.PatternBuilder.CreateCrossScaled(ref points);
        }


        public static void CreateLaserOutputPatternUsingVectorImageBuilder(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            LaserProjectorBridge.VectorImageBuilder vectorImageBuilder = new LaserProjectorBridge.VectorImageBuilder();
            vectorImageBuilder.InterpolationDistanceInProjectorRange = (Int64)(UInt32.MaxValue * 0.05);
            vectorImageBuilder.LineFirstPointIgnoreDistanceSquaredInProjectorRange = Math.Pow(UInt32.MaxValue * 0.0013, 2);
            vectorImageBuilder.LineFirstPointMergeDistanceSquaredInProjectorRange = Math.Pow(UInt32.MaxValue * 0.0005, 2);
            vectorImageBuilder.RadialDistortionCoefficientSecondDegreeInvertedUV = 0.08;
            vectorImageBuilder.RadialDistortionCoefficientSecondDegree = -0.044;
            vectorImageBuilder.RadialDistortionCoefficientFourthDegree = -0.007;
            vectorImageBuilder.RadialDistortionCoefficientSixthDegree = -0.005;
            vectorImageBuilder.StartNewVectorImage();
            vectorImageBuilder.VectorImagePoints = points;

            LaserProjectorBridge.PatternBuilder.CreateGridInProjectorRange(ref vectorImageBuilder, 6, 6, (Int32)(UInt32.MaxValue / 10), (Int32)(UInt32.MaxValue / 10), Int32.MinValue + (Int32)((UInt32.MaxValue / 10) * 3), Int32.MinValue + (Int32)((UInt32.MaxValue / 10) * 3));

            for (double scale = 0.0; scale <= 1.0; scale += 0.1)
            {
                LaserProjectorBridge.PatternBuilder.CreateSquareScaled(ref vectorImageBuilder, scale);
            }

            LaserProjectorBridge.PatternBuilder.CreatePlusFullRange(ref vectorImageBuilder);
            LaserProjectorBridge.PatternBuilder.CreateCrossFullRange(ref vectorImageBuilder);

            LaserProjectorBridge.PatternBuilder.CreateHorizontalDiamondOutsideDrawingArea(ref vectorImageBuilder);
            LaserProjectorBridge.PatternBuilder.CreateVerticalDiamondOutsideDrawingArea(ref vectorImageBuilder);
            LaserProjectorBridge.PatternBuilder.CreateCrossOutsideDrawingArea(ref vectorImageBuilder);
            LaserProjectorBridge.PatternBuilder.CreatePlusOutsideDrawingArea(ref vectorImageBuilder);

            vectorImageBuilder.FinishVectorImage();
        }


        public static void TestJMLaserOutput(int numberOfProjectors)
        {
            List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points = new List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct>();
            //CreateLaserOutputPattern(ref points);
            CreateLaserOutputPatternUsingVectorImageBuilder(ref points);

            for (int i = 0; i < numberOfProjectors; ++i)
            {
                LaserProjectorBridge.JMLaserProjector laserProjector = new LaserProjectorBridge.JMLaserProjector();
                Console.WriteLine(">>> |setupProjectorUsingIndex(" + i + ")");
                laserProjector.SetupProjectorUsingIndex((uint)i);
                Console.WriteLine(laserProjector + "\n\n");
                laserProjector.StartOutput();
                Console.WriteLine(">>> Sending pattern to projector " + i);
                if (laserProjector.SendVectorImageToProjector(ref points, 1000, 0))
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
