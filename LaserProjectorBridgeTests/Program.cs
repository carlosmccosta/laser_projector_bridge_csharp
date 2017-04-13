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
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0             , Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue, 0             , UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue, Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0             , Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, 0             , UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, 0));
        }


        public static void CreateLaserOutputPatternSquareScaled(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points, double positionScale = 0.9)
        {
            ushort intensity = (ushort)(UInt16.MaxValue * 0.9);
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * positionScale), (int)(Int32.MinValue * positionScale), 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * positionScale), (int)(Int32.MinValue * positionScale), intensity));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MaxValue * positionScale), (int)(Int32.MinValue * positionScale), intensity));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MaxValue * positionScale), (int)(Int32.MaxValue * positionScale), intensity));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * positionScale), (int)(Int32.MaxValue * positionScale), intensity));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * positionScale), (int)(Int32.MinValue * positionScale), intensity));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * positionScale), (int)(Int32.MinValue * positionScale), 0));
        }


        public static void CreateLaserOutputPatternSquareScaledUsingVectorImageBuilder(ref LaserProjectorBridge.VectorImageBuilder vectorImageBuilder, double scale = 1.0)
        {
            double xOffset = (vectorImageBuilder.DrawingAreaWidth - (vectorImageBuilder.DrawingAreaWidth * scale)) * 0.5;
            double yOffset = (vectorImageBuilder.DrawingAreaHeight - (vectorImageBuilder.DrawingAreaHeight * scale)) * 0.5;
            double xMin = xOffset;
            double xMax = vectorImageBuilder.DrawingAreaWidth - xOffset;
            double yMin = yOffset;
            double yMax = vectorImageBuilder.DrawingAreaHeight - yOffset;
            vectorImageBuilder.AddNewLine(xMin, yMin, xMax, yMin);
            vectorImageBuilder.AddNewLine(xMax, yMin, xMax, yMax);
            vectorImageBuilder.AddNewLine(xMax, yMax, xMin, yMax);
            vectorImageBuilder.AddNewLine(xMin, yMax, xMin, yMin);
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


        public static void CreateLaserOutputPatternPlusFullRangeUsingVectorImageBuilder(ref LaserProjectorBridge.VectorImageBuilder vectorImageBuilder)
        {
            double halfWidth = vectorImageBuilder.DrawingAreaWidth * 0.5;
            double halfHeight = vectorImageBuilder.DrawingAreaHeight * 0.5;
            vectorImageBuilder.AddNewLine(0.0, halfHeight, vectorImageBuilder.DrawingAreaWidth, halfHeight);
            vectorImageBuilder.AddNewLine(halfWidth, 0.0, halfWidth, vectorImageBuilder.DrawingAreaHeight);
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


        public static void CreateLaserOutputPatternCrossFullRangeUsingVectorImageBuilder(ref LaserProjectorBridge.VectorImageBuilder vectorImageBuilder)
        {
            vectorImageBuilder.AddNewLine(Int32.MinValue, Int32.MaxValue, Int32.MaxValue, Int32.MinValue);
            vectorImageBuilder.AddNewLine(Int32.MinValue, Int32.MinValue, Int32.MaxValue, Int32.MaxValue);
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

        public static void CreateLaserOutputPatternHorizontalDiamondOutsideDrawingAreaUsingVectorImageBuilder(ref LaserProjectorBridge.VectorImageBuilder vectorImageBuilder)
        {
            double halfWidth = vectorImageBuilder.DrawingAreaWidth * 0.5;
            double halfHeight = vectorImageBuilder.DrawingAreaHeight * 0.5;
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * -0.25, halfHeight, halfWidth, vectorImageBuilder.DrawingAreaHeight * 0.75);
            vectorImageBuilder.AddNewLine(halfWidth, vectorImageBuilder.DrawingAreaHeight * 0.75, vectorImageBuilder.DrawingAreaWidth * 1.25, halfHeight);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * 1.25, halfHeight, halfWidth, vectorImageBuilder.DrawingAreaHeight * 0.25);
            vectorImageBuilder.AddNewLine(halfWidth, vectorImageBuilder.DrawingAreaHeight * 0.25, vectorImageBuilder.DrawingAreaWidth * -0.25, halfHeight);
        }

        public static void CreateLaserOutputPatternVerticalDiamondOutsideDrawingAreaUsingVectorImageBuilder(ref LaserProjectorBridge.VectorImageBuilder vectorImageBuilder)
        {
            double halfWidth = vectorImageBuilder.DrawingAreaWidth * 0.5;
            double halfHeight = vectorImageBuilder.DrawingAreaHeight * 0.5;
            vectorImageBuilder.AddNewLine(halfHeight, vectorImageBuilder.DrawingAreaWidth * -0.25, vectorImageBuilder.DrawingAreaHeight * 0.75, halfWidth);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaHeight * 0.75, halfWidth, halfHeight, vectorImageBuilder.DrawingAreaWidth * 1.25);
            vectorImageBuilder.AddNewLine(halfHeight, vectorImageBuilder.DrawingAreaWidth * 1.25, vectorImageBuilder.DrawingAreaHeight * 0.25, halfWidth);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaHeight * 0.25, halfWidth, halfHeight, vectorImageBuilder.DrawingAreaWidth * -0.25);
        }

        public static void CreateLaserOutputPatternCrossOutsideDrawingAreaUsingVectorImageBuilder(ref LaserProjectorBridge.VectorImageBuilder vectorImageBuilder)
        {
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * -0.25, vectorImageBuilder.DrawingAreaHeight * -0.25, vectorImageBuilder.DrawingAreaWidth * 0.25, vectorImageBuilder.DrawingAreaHeight * 0.25);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * -0.25, vectorImageBuilder.DrawingAreaHeight * -0.25, vectorImageBuilder.DrawingAreaWidth * 0.15, vectorImageBuilder.DrawingAreaHeight * 0.25);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * -0.25, vectorImageBuilder.DrawingAreaHeight * -0.25, vectorImageBuilder.DrawingAreaWidth * 0.35, vectorImageBuilder.DrawingAreaHeight * 0.25);

            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * -0.25, vectorImageBuilder.DrawingAreaHeight * 1.25, vectorImageBuilder.DrawingAreaWidth * 0.25, vectorImageBuilder.DrawingAreaHeight * 0.75);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * -0.25, vectorImageBuilder.DrawingAreaHeight * 1.25, vectorImageBuilder.DrawingAreaWidth * 0.15, vectorImageBuilder.DrawingAreaHeight * 0.75);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * -0.25, vectorImageBuilder.DrawingAreaHeight * 1.25, vectorImageBuilder.DrawingAreaWidth * 0.35, vectorImageBuilder.DrawingAreaHeight * 0.75);

            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * 1.25, vectorImageBuilder.DrawingAreaHeight * -0.25, vectorImageBuilder.DrawingAreaWidth * 0.75, vectorImageBuilder.DrawingAreaHeight * 0.25);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * 1.25, vectorImageBuilder.DrawingAreaHeight * -0.25, vectorImageBuilder.DrawingAreaWidth * 0.65, vectorImageBuilder.DrawingAreaHeight * 0.25);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * 1.25, vectorImageBuilder.DrawingAreaHeight * -0.25, vectorImageBuilder.DrawingAreaWidth * 0.85, vectorImageBuilder.DrawingAreaHeight * 0.25);

            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * 1.25, vectorImageBuilder.DrawingAreaHeight * 1.25, vectorImageBuilder.DrawingAreaWidth * 0.75, vectorImageBuilder.DrawingAreaHeight * 0.75);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * 1.25, vectorImageBuilder.DrawingAreaHeight * 1.25, vectorImageBuilder.DrawingAreaWidth * 0.65, vectorImageBuilder.DrawingAreaHeight * 0.75);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * 1.25, vectorImageBuilder.DrawingAreaHeight * 1.25, vectorImageBuilder.DrawingAreaWidth * 0.85, vectorImageBuilder.DrawingAreaHeight * 0.75);
        }

        public static void CreateLaserOutputPatternPlusOutsideDrawingAreaUsingVectorImageBuilder(ref LaserProjectorBridge.VectorImageBuilder vectorImageBuilder)
        {
            double halfWidth = vectorImageBuilder.DrawingAreaWidth * 0.5;
            double halfHeight = vectorImageBuilder.DrawingAreaHeight * 0.5;
            vectorImageBuilder.AddNewLine(halfWidth * 0.5, vectorImageBuilder.DrawingAreaHeight * 0.25, halfWidth * 0.5, vectorImageBuilder.DrawingAreaHeight * 1.25);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * 0.25, halfHeight * 0.5, vectorImageBuilder.DrawingAreaWidth * 1.25, halfHeight * 0.5);
            vectorImageBuilder.AddNewLine(halfWidth * 1.5, vectorImageBuilder.DrawingAreaHeight * -0.25, halfWidth * 1.5, vectorImageBuilder.DrawingAreaHeight * 0.75);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * -0.25, halfHeight * 1.5, vectorImageBuilder.DrawingAreaWidth * 0.75, halfHeight * 1.5);
        }


        public static void CreateLaserOutputPatternUsingVectorImageBuilder(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            LaserProjectorBridge.VectorImageBuilder vectorImageBuilder = new LaserProjectorBridge.VectorImageBuilder();
            vectorImageBuilder.InterpolationDistanceInProjectorRange = (Int32)(UInt32.MaxValue * 0.05);
            vectorImageBuilder.LineFirstPointIgnoreDistanceSquaredInProjectorRange = Math.Pow(UInt32.MaxValue * 0.0013, 2);
            vectorImageBuilder.LineFirstPointMergeDistanceSquaredInProjectorRange = Math.Pow(UInt32.MaxValue * 0.0005, 2);
            vectorImageBuilder.RadialDistortionCoefficientSecondDegreeInvertedUV = 0.08;
            vectorImageBuilder.RadialDistortionCoefficientSecondDegree = -0.044;
            vectorImageBuilder.RadialDistortionCoefficientFourthDegree = -0.007;
            vectorImageBuilder.RadialDistortionCoefficientSixthDegree = -0.005;
            vectorImageBuilder.StartNewVectorImage();
            vectorImageBuilder.VectorImagePoints = points;

            for (double scale = 0.0; scale <= 1.0; scale += 0.1)
            {
                CreateLaserOutputPatternSquareScaledUsingVectorImageBuilder(ref vectorImageBuilder, scale);
            }

            CreateLaserOutputPatternPlusFullRangeUsingVectorImageBuilder(ref vectorImageBuilder);
            CreateLaserOutputPatternCrossFullRangeUsingVectorImageBuilder(ref vectorImageBuilder);

            CreateLaserOutputPatternHorizontalDiamondOutsideDrawingAreaUsingVectorImageBuilder(ref vectorImageBuilder);
            CreateLaserOutputPatternVerticalDiamondOutsideDrawingAreaUsingVectorImageBuilder(ref vectorImageBuilder);
            CreateLaserOutputPatternCrossOutsideDrawingAreaUsingVectorImageBuilder(ref vectorImageBuilder);
            CreateLaserOutputPatternPlusOutsideDrawingAreaUsingVectorImageBuilder(ref vectorImageBuilder);

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
