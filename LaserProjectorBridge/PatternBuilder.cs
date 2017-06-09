using System;
using System.Collections.Generic;

namespace LaserProjectorBridge
{
    public class PatternBuilder
    {
        public static void CreateGridInProjectorRange(ref LaserProjectorBridge.VectorImageBuilder vectorImageBuilder, int numberOfColumns, int numberOfRows, UInt32 columnSize, UInt32 rowSize, Int32 columnStartOffset = 0, Int32 rowStartOffset = 0)
        {
            Int64 columnStart = columnStartOffset;
            Int64 columnEnd = columnStart + columnSize * numberOfColumns;
            Int64 rowStart = rowStartOffset;
            Int64 rowEnd = rowStartOffset;
            Int64 switchValue;

            for (int rowLine = 0; rowLine < numberOfRows + 1; ++rowLine)
            {
                vectorImageBuilder.AddNewLine((Int32)columnStart, (Int32)rowStart, (Int32)columnEnd, (Int32)rowEnd);
                rowStart += rowSize;
                rowEnd += rowSize;
                switchValue = rowStart;
                rowStart = rowEnd;
                rowEnd = switchValue;
            }

            columnStart = columnStartOffset;
            columnEnd = columnStartOffset;
            rowStart = rowStartOffset;
            rowEnd = rowStart + rowSize * numberOfRows;

            for (int column = 0; column < numberOfColumns + 1; ++column)
            {
                vectorImageBuilder.AddNewLine((Int32)columnStart, (Int32)rowStart, (Int32)columnEnd, (Int32)rowEnd);
                columnStart += columnSize;
                columnEnd += columnSize;
                switchValue = columnStart;
                columnStart = columnEnd;
                columnEnd = switchValue;
            }
        }


        public static void CreateSquareFullRange(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue, 0, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue, Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0, Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, 0, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, 0));
        }


        public static void CreateSquareScaled(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points, double positionScale = 0.9)
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


        public static void CreateSquareScaled(ref LaserProjectorBridge.VectorImageBuilder vectorImageBuilder, double scale = 1.0)
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


        public static void CreatePlusFullRange(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0, Int32.MinValue, 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0, Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, 0, 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, 0, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue, 0, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue, 0, 0));
        }


        public static void CreatePlusFullRange(ref LaserProjectorBridge.VectorImageBuilder vectorImageBuilder)
        {
            double halfWidth = vectorImageBuilder.DrawingAreaWidth * 0.5;
            double halfHeight = vectorImageBuilder.DrawingAreaHeight * 0.5;
            vectorImageBuilder.AddNewLine(0.0, halfHeight, vectorImageBuilder.DrawingAreaWidth, halfHeight);
            vectorImageBuilder.AddNewLine(halfWidth, 0.0, halfWidth, vectorImageBuilder.DrawingAreaHeight);
        }


        public static void CreateCrossFullRange(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue / 2, Int32.MinValue / 2, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0, 0, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue / 2, Int32.MaxValue / 2, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue, Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue, Int32.MinValue, 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue, Int32.MinValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MaxValue / 2, Int32.MinValue / 2, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0, 0, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue / 2, Int32.MaxValue / 2, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MaxValue, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(Int32.MinValue, Int32.MaxValue, 0));
        }


        public static void CreateCrossFullRange(ref LaserProjectorBridge.VectorImageBuilder vectorImageBuilder)
        {
            vectorImageBuilder.AddNewLine(Int32.MinValue, Int32.MaxValue, Int32.MaxValue, Int32.MinValue);
            vectorImageBuilder.AddNewLine(Int32.MinValue, Int32.MinValue, Int32.MaxValue, Int32.MaxValue);
        }


        public static void CreateCrossScaled(ref List<LaserProjectorBridge.NativeMethods.JMLaser.JMVectorStruct> points)
        {
            double position_scale = 0.75;
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * position_scale), (int)(Int32.MinValue * position_scale), 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * position_scale), (int)(Int32.MinValue * position_scale), UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0, 0, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MaxValue * position_scale), (int)(Int32.MaxValue * position_scale), UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MaxValue * position_scale), (int)(Int32.MaxValue * position_scale), 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MaxValue * position_scale), (int)(Int32.MinValue * position_scale), 0));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MaxValue * position_scale), (int)(Int32.MinValue * position_scale), UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint(0, 0, UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * position_scale), (int)(Int32.MaxValue * position_scale), UInt16.MaxValue));
            points.Add(LaserProjectorBridge.JMLaserProjector.CreateSingleColorLaserPoint((int)(Int32.MinValue * position_scale), (int)(Int32.MaxValue * position_scale), 0));
        }


        public static void CreateHorizontalDiamondOutsideDrawingArea(ref LaserProjectorBridge.VectorImageBuilder vectorImageBuilder)
        {
            double halfWidth = vectorImageBuilder.DrawingAreaWidth * 0.5;
            double halfHeight = vectorImageBuilder.DrawingAreaHeight * 0.5;
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * -0.25, halfHeight, halfWidth, vectorImageBuilder.DrawingAreaHeight * 0.75);
            vectorImageBuilder.AddNewLine(halfWidth, vectorImageBuilder.DrawingAreaHeight * 0.75, vectorImageBuilder.DrawingAreaWidth * 1.25, halfHeight);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * 1.25, halfHeight, halfWidth, vectorImageBuilder.DrawingAreaHeight * 0.25);
            vectorImageBuilder.AddNewLine(halfWidth, vectorImageBuilder.DrawingAreaHeight * 0.25, vectorImageBuilder.DrawingAreaWidth * -0.25, halfHeight);
        }


        public static void CreateVerticalDiamondOutsideDrawingArea(ref LaserProjectorBridge.VectorImageBuilder vectorImageBuilder)
        {
            double halfWidth = vectorImageBuilder.DrawingAreaWidth * 0.5;
            double halfHeight = vectorImageBuilder.DrawingAreaHeight * 0.5;
            vectorImageBuilder.AddNewLine(halfHeight, vectorImageBuilder.DrawingAreaWidth * -0.25, vectorImageBuilder.DrawingAreaHeight * 0.75, halfWidth);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaHeight * 0.75, halfWidth, halfHeight, vectorImageBuilder.DrawingAreaWidth * 1.25);
            vectorImageBuilder.AddNewLine(halfHeight, vectorImageBuilder.DrawingAreaWidth * 1.25, vectorImageBuilder.DrawingAreaHeight * 0.25, halfWidth);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaHeight * 0.25, halfWidth, halfHeight, vectorImageBuilder.DrawingAreaWidth * -0.25);
        }


        public static void CreateCrossOutsideDrawingArea(ref LaserProjectorBridge.VectorImageBuilder vectorImageBuilder)
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


        public static void CreatePlusOutsideDrawingArea(ref LaserProjectorBridge.VectorImageBuilder vectorImageBuilder)
        {
            double halfWidth = vectorImageBuilder.DrawingAreaWidth * 0.5;
            double halfHeight = vectorImageBuilder.DrawingAreaHeight * 0.5;
            vectorImageBuilder.AddNewLine(halfWidth * 0.5, vectorImageBuilder.DrawingAreaHeight * 0.25, halfWidth * 0.5, vectorImageBuilder.DrawingAreaHeight * 1.25);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * 0.25, halfHeight * 0.5, vectorImageBuilder.DrawingAreaWidth * 1.25, halfHeight * 0.5);
            vectorImageBuilder.AddNewLine(halfWidth * 1.5, vectorImageBuilder.DrawingAreaHeight * -0.25, halfWidth * 1.5, vectorImageBuilder.DrawingAreaHeight * 0.75);
            vectorImageBuilder.AddNewLine(vectorImageBuilder.DrawingAreaWidth * -0.25, halfHeight * 1.5, vectorImageBuilder.DrawingAreaWidth * 0.75, halfHeight * 1.5);
        }
    }
}
