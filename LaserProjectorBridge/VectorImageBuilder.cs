using System;
using System.Collections.Generic;
using System.Linq;

namespace LaserProjectorBridge
{
    public class VectorImageBuilder
    {
        public enum AxisPosition
        {
            TopLeft,        // x -> right | y -> down
            BottomLeft,     // x -> right | y -> up
            Middle          // x -> right | y -> up
        }

        #region public fields
        public List<NativeMethods.JMLaser.JMVectorStruct> VectorImagePoints { get; set; }
        public ProjectionModelProperties ProjectionModelProperties { get; set; } = new ProjectionModelProperties();
        public double DrawingAreaXOffset { get; set; } = 0.0;
        public double DrawingAreaYOffset { get; set; } = 0.0;
        public Int32 MinimumProjectorRangeValueForClipping { get; set; } = (Int32)(Int32.MinValue * 0.99);
        public Int32 MaximumProjectorRangeValueForClipping { get; set; } = (Int32)(Int32.MaxValue * 0.99);
        public bool TrimPointsOutsideDrawingArea { get; set; } = true;
        public double LineFirstPointMergeDistanceSquaredInProjectorRange { get; set; } = Math.Pow(UInt32.MaxValue * 0.0005, 2);
        public double LineFirstPointIgnoreDistanceSquaredInProjectorRange { get; set; } = Math.Pow(UInt32.MaxValue * 0.001, 2);
        public Int64 InterpolationDistanceInProjectorRange { get; set; } = (Int32)(UInt32.MaxValue * 0.001);
        public Int32 NumberOfBlankingPointsForLineStartAndEnd { get; set; } = 15;
        public Int32 MaximumNumberOfPoints { get; set; } = 16000;
        //public Int32 BlankingDistanceInProjectorRange { get; set; } = (Int32)(UInt32.MaxValue * 0.001);
        //public int NumberOfPointRepetitionsOnLineMiddlePoints { get; set; } = 1;
        //public int NumberOfPointRepetitionsOnLineStartPoint { get; set; } = 1;
        //public int NumberOfPointRepetitionsOnLineEndPoint { get; set; } = 1;
        #endregion

        #region private fields
        private double _drawingAreaToProjectorRangeXScale;
        private double _drawingAreaToProjectorRangeYScale;
        #endregion

        public void StartNewVectorImage()
        {
            VectorImagePoints = new List<NativeMethods.JMLaser.JMVectorStruct>();
            _drawingAreaToProjectorRangeXScale = ((double)UInt32.MaxValue) / ProjectionModelProperties.ImageWidthInPixels;
            _drawingAreaToProjectorRangeYScale = ((double)UInt32.MaxValue) / ProjectionModelProperties.ImageHeightInPixels;
        }

        public void FinishVectorImage()
        {
            if (VectorImagePoints.Count > 0)
            {
                AddLastPointBlankingPoints();
                CorrectDistortionOnVectorImage();
                // todo: laser path optimization and blanking
            }
        }

        public void AddReverseImage()
        {
            if (VectorImagePoints.Count * 2 <= MaximumNumberOfPoints)
            {
                for (int i = VectorImagePoints.Count - 1; i >= 0; --i)
                {
                    NativeMethods.JMLaser.JMVectorStruct point = VectorImagePoints[i];
                    VectorImagePoints.Add(point);
                }
            }
        }

        public void convertPointFromDrawingAreaOriginToProjectorOrigin(double x, double y, out double xPointInDrawingAreaAndProjectorOrigin, out double yPointInDrawingAreaAndProjectorOrigin, AxisPosition projectorOriginAxisPosition = AxisPosition.BottomLeft)
        {
            switch (projectorOriginAxisPosition)
            {
                case AxisPosition.TopLeft:
                {
                    xPointInDrawingAreaAndProjectorOrigin = x - ProjectionModelProperties.ImageWidthInPixels * 0.5;
                    yPointInDrawingAreaAndProjectorOrigin = (ProjectionModelProperties.ImageHeightInPixels - y) - ProjectionModelProperties.ImageHeightInPixels * 0.5;
                    break;
                }

                case AxisPosition.BottomLeft:
                {
                    xPointInDrawingAreaAndProjectorOrigin = x - ProjectionModelProperties.ImageWidthInPixels * 0.5;
                    yPointInDrawingAreaAndProjectorOrigin = y - ProjectionModelProperties.ImageHeightInPixels * 0.5;
                    break;
                }

                case AxisPosition.Middle:
                {
                    xPointInDrawingAreaAndProjectorOrigin = x;
                    yPointInDrawingAreaAndProjectorOrigin = y;
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(projectorOriginAxisPosition), projectorOriginAxisPosition, null);
            }
        }

        public void ConvertPointFromProjectorOriginToDrawingAreaOrigin(double x, double y, out double xPointInDrawingAreaOrigin, out double yPointInDrawingAreaOrigin, AxisPosition drawingAreaOriginAxisPosition = AxisPosition.BottomLeft)
        {
            switch (drawingAreaOriginAxisPosition)
            {
                case AxisPosition.TopLeft:
                    {
                        xPointInDrawingAreaOrigin = x + ProjectionModelProperties.ImageWidthInPixels * 0.5;
                        yPointInDrawingAreaOrigin = (ProjectionModelProperties.ImageHeightInPixels - y) + ProjectionModelProperties.ImageHeightInPixels * 0.5;
                        break;
                    }

                case AxisPosition.BottomLeft:
                    {
                        xPointInDrawingAreaOrigin = x + ProjectionModelProperties.ImageWidthInPixels * 0.5;
                        yPointInDrawingAreaOrigin = y + ProjectionModelProperties.ImageHeightInPixels * 0.5;
                        break;
                    }

                case AxisPosition.Middle:
                    {
                        xPointInDrawingAreaOrigin = x;
                        yPointInDrawingAreaOrigin = y;
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(drawingAreaOriginAxisPosition), drawingAreaOriginAxisPosition, null);
            }
        }


        public bool ConvertPointFromDrawingAreaToProjectorOrigin(double x, double y, out double xPointInDrawingAreaAndProjectorOrigin, out double yPointInDrawingAreaAndProjectorOrigin, AxisPosition originAxisPosition = AxisPosition.BottomLeft)
        {
            try
            {
                checked { convertPointFromDrawingAreaOriginToProjectorOrigin(x, y, out xPointInDrawingAreaAndProjectorOrigin, out yPointInDrawingAreaAndProjectorOrigin, originAxisPosition); }
                return true;
            }
            catch (System.OverflowException)
            {
                xPointInDrawingAreaAndProjectorOrigin = 0;
                yPointInDrawingAreaAndProjectorOrigin = 0;
                return false;
            }
        }

        public bool ConvertPointFromDrawingAreaInProjectorOriginToProjectorRange(double xPointInDrawingAreaAndProjectorOrigin, double yPointInDrawingAreaAndProjectorOrigin, out Int32 xPointInProjectorRange, out Int32 yPointInProjectorRange)
        {
            bool pointOverflow = false;

            try
            {
                checked { xPointInProjectorRange = (Int32)((xPointInDrawingAreaAndProjectorOrigin - DrawingAreaXOffset) * _drawingAreaToProjectorRangeXScale); }
            }
            catch (System.OverflowException)
            {
                xPointInProjectorRange = xPointInDrawingAreaAndProjectorOrigin - DrawingAreaXOffset < ProjectionModelProperties.ImageWidthInPixels * 0.5 ? MinimumProjectorRangeValueForClipping : MaximumProjectorRangeValueForClipping;
                pointOverflow = true;
            }

            try
            {
                checked { yPointInProjectorRange = (Int32)((yPointInDrawingAreaAndProjectorOrigin - DrawingAreaYOffset) * _drawingAreaToProjectorRangeYScale); }
            }
            catch (System.OverflowException)
            {
                yPointInProjectorRange = yPointInDrawingAreaAndProjectorOrigin - DrawingAreaYOffset < ProjectionModelProperties.ImageHeightInPixels * 0.5 ? MinimumProjectorRangeValueForClipping : MaximumProjectorRangeValueForClipping;
                pointOverflow = true;
            }

            return !pointOverflow;
        }

        public bool ConvertPointFromProjectorRangeToDrawingArea(Int32 xPointInProjectorRange, Int32 yPointInProjectorRange, out double xPointInDrawingAreaRange, out double yPointInDrawingAreaRange, AxisPosition originAxisPosition = AxisPosition.BottomLeft)
        {
            double xPointInDrawingAreaAndProjectorOrigin;
            double yPointInDrawingAreaAndProjectorOrigin;

            bool pointOverflow = false;

            try
            {
                checked { xPointInDrawingAreaAndProjectorOrigin = ((double)xPointInProjectorRange / _drawingAreaToProjectorRangeXScale) + DrawingAreaXOffset; }
            }
            catch (System.OverflowException)
            {
                xPointInDrawingAreaAndProjectorOrigin = 0;
                pointOverflow = true;
            }

            try
            {
                checked { yPointInDrawingAreaAndProjectorOrigin = ((double)yPointInProjectorRange / _drawingAreaToProjectorRangeYScale) + DrawingAreaYOffset; }
            }
            catch (System.OverflowException)
            {
                yPointInDrawingAreaAndProjectorOrigin = 0;
                pointOverflow = true;
            }

            try
            {
                checked { ConvertPointFromProjectorOriginToDrawingAreaOrigin(xPointInDrawingAreaAndProjectorOrigin, yPointInDrawingAreaAndProjectorOrigin, out xPointInDrawingAreaRange, out yPointInDrawingAreaRange, originAxisPosition); }
            }
            catch (System.OverflowException)
            {
                xPointInDrawingAreaRange = 0;
                yPointInDrawingAreaRange = 0;
                return false;
            }

            return !pointOverflow;
        }

        public bool TrimLineInDrawingAreaAndProjectorOrigin(ref double startXInProjectorOrigin, ref double startYInProjectorOrigin, ref double endXInProjectorOrigin, ref double endYInProjectorOrigin)
        {
            bool startPointValid = IsPointInProjectorOriginWithinDrawingArea(startXInProjectorOrigin, startYInProjectorOrigin);
            bool endPointValid = IsPointInProjectorOriginWithinDrawingArea(endXInProjectorOrigin, endYInProjectorOrigin);
            if (startPointValid && endPointValid) return true;
            if (!startPointValid && !endPointValid) return false;

            double validPointX, validPointY, invalidPointX, invalidPointY;
            if (!startPointValid)
            {
                invalidPointX = startXInProjectorOrigin;
                invalidPointY = startYInProjectorOrigin;
                validPointX = endXInProjectorOrigin;
                validPointY = endYInProjectorOrigin;
            }
            else
            {
                invalidPointX = endXInProjectorOrigin;
                invalidPointY = endYInProjectorOrigin;
                validPointX = startXInProjectorOrigin;
                validPointY = startYInProjectorOrigin;
            }

            double halfWidth = ProjectionModelProperties.ImageWidthInPixels * 0.5;
            double halfHeight = ProjectionModelProperties.ImageHeightInPixels * 0.5;
            double xIntersection = 0.0, yIntersection = 0.0;

            // left
            if (invalidPointX < -halfWidth && invalidPointY >= -halfHeight && invalidPointY <= halfHeight)
            {
                if (LineIntersection(-halfWidth, -halfHeight, -halfWidth, halfHeight, startXInProjectorOrigin, startYInProjectorOrigin, endXInProjectorOrigin, endYInProjectorOrigin, ref xIntersection, ref yIntersection))
                    xIntersection = -halfWidth;
                else
                    return false;
            }

            // right
            if (invalidPointX > halfWidth && invalidPointY >= -halfHeight && invalidPointY <= halfHeight)
            {
                if (LineIntersection(halfWidth, -halfHeight, halfWidth, halfHeight, startXInProjectorOrigin, startYInProjectorOrigin, endXInProjectorOrigin, endYInProjectorOrigin, ref xIntersection, ref yIntersection))
                    xIntersection = halfWidth;
                else
                    return false;
            }

            // top
            if (invalidPointY > halfHeight && invalidPointX >= -halfWidth && invalidPointX <= halfWidth)
            {
                if (LineIntersection(-halfWidth, halfHeight, halfWidth, halfHeight, startXInProjectorOrigin, startYInProjectorOrigin, endXInProjectorOrigin, endYInProjectorOrigin, ref xIntersection, ref yIntersection))
                    yIntersection = halfHeight;
                else
                    return false;
            }

            // bottom
            if (invalidPointY < -halfHeight && invalidPointX >= -halfWidth && invalidPointX <= halfWidth)
            {
                if (LineIntersection(-halfWidth, -halfHeight, halfWidth, -halfHeight, startXInProjectorOrigin, startYInProjectorOrigin, endXInProjectorOrigin, endYInProjectorOrigin, ref xIntersection, ref yIntersection))
                    yIntersection = -halfHeight;
                else
                    return false;
            }

            // bottom left
            if (invalidPointX < -halfWidth && invalidPointY < -halfHeight)
            {
                double xIntersectionBottom = 0.0, yIntersectionBottom = 0.0;
                double xIntersectionLeft = 0.0, yIntersectionLeft = 0.0;
                if (LineIntersection(-halfWidth, -halfHeight, halfWidth, -halfHeight, startXInProjectorOrigin, startYInProjectorOrigin, endXInProjectorOrigin, endYInProjectorOrigin, ref xIntersectionBottom, ref yIntersectionBottom) &&
                    LineIntersection(-halfWidth, -halfHeight, -halfWidth, halfHeight, startXInProjectorOrigin, startYInProjectorOrigin, endXInProjectorOrigin, endYInProjectorOrigin, ref xIntersectionLeft, ref yIntersectionLeft))
                {
                    double distanceToBottomIntersection = Math.Pow(xIntersectionBottom - validPointX, 2.0) + Math.Pow(yIntersectionBottom - validPointY, 2.0);
                    double distanceToLeftIntersection = Math.Pow(xIntersectionLeft - validPointX, 2.0) + Math.Pow(yIntersectionLeft - validPointY, 2.0);

                    if (distanceToBottomIntersection < distanceToLeftIntersection)
                    {
                        xIntersection = xIntersectionBottom;
                        yIntersection = -halfHeight;
                    }
                    else
                    {
                        xIntersection = -halfWidth;
                        yIntersection = yIntersectionLeft;
                    }
                }
                else
                    return false;
            }

            // top left
            if (invalidPointX < -halfWidth && invalidPointY > halfHeight)
            {
                double xIntersectionTop = 0.0, yIntersectionTop = 0.0;
                double xIntersectionLeft = 0.0, yIntersectionLeft = 0.0;
                if (LineIntersection(-halfWidth, halfHeight, halfWidth, halfHeight, startXInProjectorOrigin, startYInProjectorOrigin, endXInProjectorOrigin, endYInProjectorOrigin, ref xIntersectionTop, ref yIntersectionTop) &&
                    LineIntersection(-halfWidth, -halfHeight, -halfWidth, halfHeight, startXInProjectorOrigin, startYInProjectorOrigin, endXInProjectorOrigin, endYInProjectorOrigin, ref xIntersectionLeft, ref yIntersectionLeft))
                {
                    double distanceToTopIntersection = Math.Pow(xIntersectionTop - validPointX, 2.0) + Math.Pow(yIntersectionTop - validPointY, 2.0);
                    double distanceToLeftIntersection = Math.Pow(xIntersectionLeft - validPointX, 2.0) + Math.Pow(yIntersectionLeft - validPointY, 2.0);

                    if (distanceToTopIntersection < distanceToLeftIntersection)
                    {
                        xIntersection = xIntersectionTop;
                        yIntersection = halfHeight;
                    }
                    else
                    {
                        xIntersection = -halfWidth;
                        yIntersection = yIntersectionLeft;
                    }
                }
                else
                    return false;
            }

            // bottom right
            if (invalidPointX > halfWidth && invalidPointY < -halfHeight)
            {
                double xIntersectionBottom = 0.0, yIntersectionBottom = 0.0;
                double xIntersectionRight = 0.0, yIntersectionRight = 0.0;
                if (LineIntersection(-halfWidth, -halfHeight, halfWidth, -halfHeight, startXInProjectorOrigin, startYInProjectorOrigin, endXInProjectorOrigin, endYInProjectorOrigin, ref xIntersectionBottom, ref yIntersectionBottom) &&
                    LineIntersection(halfWidth, -halfHeight, halfWidth, halfHeight, startXInProjectorOrigin, startYInProjectorOrigin, endXInProjectorOrigin, endYInProjectorOrigin, ref xIntersectionRight, ref yIntersectionRight))
                {
                    double distanceToBottomIntersection = Math.Pow(xIntersectionBottom - validPointX, 2.0) + Math.Pow(yIntersectionBottom - validPointY, 2.0);
                    double distanceToRightIntersection = Math.Pow(xIntersectionRight - validPointX, 2.0) + Math.Pow(yIntersectionRight - validPointY, 2.0);

                    if (distanceToBottomIntersection < distanceToRightIntersection)
                    {
                        xIntersection = xIntersectionBottom;
                        yIntersection = -halfHeight;
                    }
                    else
                    {
                        xIntersection = halfWidth;
                        yIntersection = yIntersectionRight;
                    }
                }
                else
                    return false;
            }

            // top right
            if (invalidPointX > halfWidth && invalidPointY > halfHeight)
            {
                double xIntersectionTop = 0.0, yIntersectionTop = 0.0;
                double xIntersectionRight = 0.0, yIntersectionRight = 0.0;
                if (LineIntersection(-halfWidth, halfHeight, halfWidth, halfHeight, startXInProjectorOrigin, startYInProjectorOrigin, endXInProjectorOrigin, endYInProjectorOrigin, ref xIntersectionTop, ref yIntersectionTop) &&
                    LineIntersection(halfWidth, -halfHeight, halfWidth, halfHeight, startXInProjectorOrigin, startYInProjectorOrigin, endXInProjectorOrigin, endYInProjectorOrigin, ref xIntersectionRight, ref yIntersectionRight))
                {
                    double distanceToTopIntersection = Math.Pow(xIntersectionTop - validPointX, 2.0) + Math.Pow(yIntersectionTop - validPointY, 2.0);
                    double distanceToRightIntersection = Math.Pow(xIntersectionRight - validPointX, 2.0) + Math.Pow(yIntersectionRight - validPointY, 2.0);

                    if (distanceToTopIntersection < distanceToRightIntersection)
                    {
                        xIntersection = xIntersectionTop;
                        yIntersection = halfHeight;
                    }
                    else
                    {
                        xIntersection = halfWidth;
                        yIntersection = yIntersectionRight;
                    }
                }
                else
                    return false;
            }

            if (!startPointValid)
            {
                startXInProjectorOrigin = xIntersection;
                startYInProjectorOrigin = yIntersection;
            }
            else
            {
                endXInProjectorOrigin = xIntersection;
                endYInProjectorOrigin = yIntersection;
            }

            return true;
        }

        public bool IsPointInProjectorOriginWithinDrawingArea(double x, double y)
        {
            double halfWidth = ProjectionModelProperties.ImageWidthInPixels * 0.5;
            double halfHeight = ProjectionModelProperties.ImageHeightInPixels * 0.5;
            return (x >= -halfWidth && x <= halfWidth && y >= -halfHeight && y <= halfHeight);
        }

        public bool AddNewLine(double startX, double startY, double endX, double endY,
            UInt16 red = UInt16.MaxValue, UInt16 green = UInt16.MaxValue, UInt16 blue = UInt16.MaxValue,
            UInt16 intensity = UInt16.MaxValue, AxisPosition originAxisPosition = AxisPosition.BottomLeft)
        {
            double startXInProjectorOrigin;
            double startYInProjectorOrigin;
            double endXInProjectorOrigin;
            double endYInProjectorOrigin;

            if (!ConvertPointFromDrawingAreaToProjectorOrigin(startX, startY, out startXInProjectorOrigin, out startYInProjectorOrigin, originAxisPosition)) { return false; }
            if (!ConvertPointFromDrawingAreaToProjectorOrigin(endX, endY, out endXInProjectorOrigin, out endYInProjectorOrigin, originAxisPosition)) { return false; }

            if (!TrimLineInDrawingAreaAndProjectorOrigin(ref startXInProjectorOrigin, ref startYInProjectorOrigin, ref endXInProjectorOrigin, ref endYInProjectorOrigin)) { return false; }

            Int32 startXInProjectorRange;
            Int32 startYInProjectorRange;
            Int32 endXInProjectorRange;
            Int32 endYInProjectorRange;

            ConvertPointFromDrawingAreaInProjectorOriginToProjectorRange(startXInProjectorOrigin, startYInProjectorOrigin, out startXInProjectorRange, out startYInProjectorRange);
            ConvertPointFromDrawingAreaInProjectorOriginToProjectorRange(endXInProjectorOrigin, endYInProjectorOrigin, out endXInProjectorRange, out endYInProjectorRange);

            return AddNewLine(startXInProjectorRange, startYInProjectorRange, endXInProjectorRange, endYInProjectorRange, red, green, blue, intensity);
        }

        public bool AddNewLine(Int32 startX, Int32 startY, Int32 endX, Int32 endY,
            UInt16 red = UInt16.MaxValue, UInt16 green = UInt16.MaxValue, UInt16 blue = UInt16.MaxValue, UInt16 intensity = UInt16.MaxValue)
        {
            var startPoint = new NativeMethods.JMLaser.JMVectorStruct()
            {
                x = startX,
                y = startY,
                r = red,
                g = green,
                b = blue,
                i = intensity,
                deepblue = UInt16.MaxValue,
                yellow = UInt16.MaxValue,
                cyan = UInt16.MaxValue,
                user4 = UInt16.MaxValue
            };

            var endPoint = new NativeMethods.JMLaser.JMVectorStruct()
            {
                x = endX,
                y = endY,
                r = red,
                g = green,
                b = blue,
                i = intensity,
                deepblue = UInt16.MaxValue,
                yellow = UInt16.MaxValue,
                cyan = UInt16.MaxValue,
                user4 = UInt16.MaxValue
            };

            return AddNewLine(ref startPoint, ref endPoint);
        }

        public bool AddNewLine(ref NativeMethods.JMLaser.JMVectorStruct startPoint, ref NativeMethods.JMLaser.JMVectorStruct endPoint)
        {
            if (VectorImagePoints.Count + 2 > MaximumNumberOfPoints)
                return false;

            if (VectorImagePoints.Count > 0)
            {
                NativeMethods.JMLaser.JMVectorStruct lastPoint = VectorImagePoints.Last();
                double distanceToLastPointSquared = NativeMethods.JMLaser.JMVectorStructDistanceSquared(startPoint, lastPoint);
                bool startPointDifferentThanLastPoint = false;

                // check if points are the same using the ignore distance as threshold
                if (LineFirstPointIgnoreDistanceSquaredInProjectorRange > 0 && distanceToLastPointSquared >= LineFirstPointIgnoreDistanceSquaredInProjectorRange)
                {
                    startPointDifferentThanLastPoint = true;
                }
                else
                // check if points are the same using their coordinates (when ignore distance threshold is not active)
                if (LineFirstPointIgnoreDistanceSquaredInProjectorRange <= 0 && (lastPoint.x != startPoint.x || lastPoint.y != startPoint.y))
                {
                    startPointDifferentThanLastPoint = true;
                }
                else
                // check if colors are different when points are considered the same
                if (lastPoint.i != startPoint.i || lastPoint.r != startPoint.r || lastPoint.g != startPoint.g || lastPoint.b != startPoint.b ||
                    lastPoint.cyan != startPoint.cyan || lastPoint.deepblue != startPoint.deepblue || lastPoint.yellow != startPoint.yellow || lastPoint.user4 != startPoint.user4)
                {
                    startPointDifferentThanLastPoint = true;
                }

                if (startPointDifferentThanLastPoint)
                {
                    AddLastPointBlankingPoints();
                    AddFirstPointBlankingPoints(startPoint);
                }
                else
                {
                    if (distanceToLastPointSquared > 0 && LineFirstPointMergeDistanceSquaredInProjectorRange > 0 && distanceToLastPointSquared < LineFirstPointMergeDistanceSquaredInProjectorRange)
                    {
                        lastPoint.x = (Int32)((lastPoint.x + startPoint.x) / 2.0);
                        lastPoint.y = (Int32)((lastPoint.y + startPoint.y) / 2.0);
                        ReplaceLastPoint(ref lastPoint);
                    }

                    if (VectorImagePoints.Count > 1)
                    {
                        NativeMethods.JMLaser.JMVectorStruct penultimatePoint = VectorImagePoints[VectorImagePoints.Count - 2];
                        double distanceToPenultimatePointSquared = NativeMethods.JMLaser.JMVectorStructDistanceSquared(endPoint, penultimatePoint);
                        if (distanceToPenultimatePointSquared < LineFirstPointIgnoreDistanceSquaredInProjectorRange)
                        {
                            RemoveLastPoint();
                        }
                    }
                }
            }
            else
            {
                AddFirstPointBlankingPoints(startPoint);
            }

            return AddNewPointWithLinearInterpolationFromLastPoint(ref endPoint);
        }

        public bool AddNewPoint(double x, double y, 
            UInt16 red = UInt16.MaxValue, UInt16 green = UInt16.MaxValue, UInt16 blue = UInt16.MaxValue,
            UInt16 intensity = UInt16.MaxValue, AxisPosition originAxisPosition = AxisPosition.BottomLeft)
        {
            double xInProjectorOrigin;
            double yInProjectorOrigin;
            Int32 xInProjectorRange;
            Int32 yInProjectorRange;
            if (ConvertPointFromDrawingAreaToProjectorOrigin(x, y, out xInProjectorOrigin, out yInProjectorOrigin, originAxisPosition) &&
                ConvertPointFromDrawingAreaInProjectorOriginToProjectorRange(xInProjectorOrigin, yInProjectorOrigin, out xInProjectorRange, out yInProjectorRange))
            {
                AddNewPoint(xInProjectorRange, yInProjectorRange, red, green, blue, intensity);
                return true;
            }
            return false;
        }

        public bool AddNewPoint(Int32 x, Int32 y, 
            UInt16 red = UInt16.MaxValue, UInt16 green = UInt16.MaxValue, UInt16 blue = UInt16.MaxValue, UInt16 intensity = UInt16.MaxValue)
        {
            var newPoint = new NativeMethods.JMLaser.JMVectorStruct()
            {
                x = x, y = y, r = red, g = green, b = blue, i = intensity, deepblue = UInt16.MaxValue, yellow = UInt16.MaxValue, cyan = UInt16.MaxValue, user4 = UInt16.MaxValue
            };

            return AddNewPoint(ref newPoint);
        }

        public bool AddNewPoint(ref NativeMethods.JMLaser.JMVectorStruct point)
        {
            if (VectorImagePoints.Count < MaximumNumberOfPoints)
            {
                if (VectorImagePoints.Count == 0 && point.i > 0)
                {
                    NativeMethods.JMLaser.JMVectorStruct pointStartOff = point;
                    pointStartOff.i = 0;
                    VectorImagePoints.Add(pointStartOff);
                }
                VectorImagePoints.Add(point);
                return true;
            }

            return false;
        }

        public bool AddNewPointWithLinearInterpolationFromLastPoint(ref NativeMethods.JMLaser.JMVectorStruct point)
        {
            if (InterpolationDistanceInProjectorRange > 0)
            {
                NativeMethods.JMLaser.JMVectorStruct lastPoint = VectorImagePoints.Last();
                double distanceToLastPoint = Math.Sqrt(NativeMethods.JMLaser.JMVectorStructDistanceSquared(lastPoint, point));
                int numberOfInterpolationPoints = (int)(distanceToLastPoint / (double)InterpolationDistanceInProjectorRange) - 1;

                if (VectorImagePoints.Count + numberOfInterpolationPoints > MaximumNumberOfPoints)
                {
                    numberOfInterpolationPoints = MaximumNumberOfPoints - VectorImagePoints.Count - 1;
                }

                if (numberOfInterpolationPoints > 0)
                {
                    double tIncrement = 1.0 / (double)numberOfInterpolationPoints;
                    double t = tIncrement;
                    for (int i = 0; i < numberOfInterpolationPoints; ++i)
                    {
                        NativeMethods.JMLaser.JMVectorStruct newPoint = lastPoint;
                        newPoint.x = (Int32)LinearInterpolation(lastPoint.x, point.x, t);
                        newPoint.y = (Int32)LinearInterpolation(lastPoint.y, point.y, t);
                        AddNewPoint(ref newPoint);
                        t += tIncrement;
                    }
                }
            }

            return AddNewPoint(ref point);
        }

        public void AddLastPointTurnedOff()
        {
            if (VectorImagePoints.Count > 0)
            {
                NativeMethods.JMLaser.JMVectorStruct point = VectorImagePoints.Last();
                point.i = 0;
                AddNewPoint(ref point);
            }
        }

        public void AddLastPointBlankingPoints()
        {
            if (VectorImagePoints.Count > 0)
            {
                NativeMethods.JMLaser.JMVectorStruct lastPoint = VectorImagePoints.Last();
                if (lastPoint.i != 0)
                {
                    for (int i = 0; i < NumberOfBlankingPointsForLineStartAndEnd - 1; ++i)
                        AddNewPoint(ref lastPoint);
                }

                lastPoint.i = 0;
                AddNewPoint(ref lastPoint);
                for (int i = 0; i < NumberOfBlankingPointsForLineStartAndEnd; ++i)
                    AddNewPoint(ref lastPoint);
            }
        }

        public void ReplaceLastPoint(ref NativeMethods.JMLaser.JMVectorStruct point)
        {
            if (VectorImagePoints.Count > 0)
                RemoveLastPoint();

            VectorImagePoints.Add(point);
        }

        public void RemoveLastPoint()
        {
            if (VectorImagePoints.Count > 0)
                VectorImagePoints.RemoveAt(VectorImagePoints.Count - 1);
        }

        public void AddFirstPointBlankingPoints(NativeMethods.JMLaser.JMVectorStruct startPoint)
        {
            NativeMethods.JMLaser.JMVectorStruct startPointOff = startPoint;
            startPointOff.i = 0;
            AddNewPoint(ref startPointOff);
            for (int i = 0; i < NumberOfBlankingPointsForLineStartAndEnd - 1; ++i)
                AddNewPoint(ref startPointOff);

            AddNewPoint(ref startPoint);
            for (int i = 0; i < NumberOfBlankingPointsForLineStartAndEnd - 1; ++i)
                AddNewPoint(ref startPoint);
        }

        public void CorrectDistortionOnVectorImage()
        {
            if (ProjectionModelProperties.ComputeDistancesToImagePlanes)
            {
                ProjectionModelProperties.DistanceToImagePlaneForConvertingXGalvoAngleToDrawingArea = ComputeDistanceToImagePlane(ProjectionModelProperties.FocalLengthXInPixels, ProjectionModelProperties.ImageWidthInPixels);
                ProjectionModelProperties.DistanceToImagePlaneForConvertingYGalvoAngleToDrawingArea = ComputeDistanceToImagePlane(ProjectionModelProperties.FocalLengthYInPixels, ProjectionModelProperties.ImageHeightInPixels);
                ProjectionModelProperties.DistanceToImagePlaneForCorrectingDistortion = ProjectionModelProperties.DistanceToImagePlaneForConvertingXGalvoAngleToDrawingArea;
            }

            ComputeScalingFactorsFromImagePlaneToDrawingArea(ProjectionModelProperties.FocalLengthXInPixels, ProjectionModelProperties.ImageWidthInPixels,
                ProjectionModelProperties.FocalLengthYInPixels, ProjectionModelProperties.ImageHeightInPixels, ProjectionModelProperties.DistanceBetweenMirrors, ProjectionModelProperties.DistanceToImagePlaneForCorrectingDistortion,
                out double imagePlaneToDrawingAreaXScale, out double imagePlaneToDrawingAreaYScale);

            List<NativeMethods.JMLaser.JMVectorStruct> VectorImagePointsTrimmed = new List<NativeMethods.JMLaser.JMVectorStruct>();

            for (int i = 0; i < VectorImagePoints.Count; ++i)
            {
                NativeMethods.JMLaser.JMVectorStruct point = VectorImagePoints[i];
                double originalX = (double)point.x / _drawingAreaToProjectorRangeXScale;
                double originalY = (double)point.y / _drawingAreaToProjectorRangeYScale;
                double newX = originalX;
                double newY = originalY;

                if (ProjectionModelProperties.DistanceBetweenMirrors != 0.0)
                {
                    if (ProjectionModelProperties.ChangeToPrincipalPointOriginWhenCorrectingGalvanometerDistortion)
                    {
                        newX = ChangeFromDrawingAreaOriginToPrincipalPointOrigin(newX, ProjectionModelProperties.ImageWidthInPixels, ProjectionModelProperties.PrincipalPointXInPixels);
                        newY = ChangeFromDrawingAreaOriginToPrincipalPointOrigin(newY, ProjectionModelProperties.ImageHeightInPixels, ProjectionModelProperties.PrincipalPointYInPixels);
                    }

                    CorrectGalvanometerMirrorsDistortion(ref newX, ref newY, ProjectionModelProperties.DistanceBetweenMirrors,
                        ProjectionModelProperties.DistanceToImagePlaneForCorrectingDistortion,
                        ProjectionModelProperties.DistanceToImagePlaneForConvertingXGalvoAngleToDrawingArea,
                        ProjectionModelProperties.DistanceToImagePlaneForConvertingYGalvoAngleToDrawingArea,
                        ProjectionModelProperties.UseRayToPlaneIntersectionForConvertingGalvosAnglesToDrawingArea);

                    if (ProjectionModelProperties.ScaleImagePlanePointsUsingIntrinsics)
                    {
                        newX *= imagePlaneToDrawingAreaXScale;
                        newY *= imagePlaneToDrawingAreaYScale;
                    }

                    if (ProjectionModelProperties.ChangeToPrincipalPointOriginWhenCorrectingGalvanometerDistortion)
                    {
                        newX = ChangeFromPrincipalPointOriginToDrawingAreaOrigin(newX, ProjectionModelProperties.ImageWidthInPixels, ProjectionModelProperties.PrincipalPointXInPixels);
                        newY = ChangeFromPrincipalPointOriginToDrawingAreaOrigin(newY, ProjectionModelProperties.ImageHeightInPixels, ProjectionModelProperties.PrincipalPointYInPixels);
                    }
                }

                if (ProjectionModelProperties.HasLensDistortionCoefficients())
                    CorrectLensDistortion(ref newX, ref newY, ProjectionModelProperties);

                bool pointOutsideDrawingArea = false;
                if (newX < -ProjectionModelProperties.ImageWidthInPixels / 2.0 || newX > ProjectionModelProperties.ImageWidthInPixels / 2.0 ||
                    newY < -ProjectionModelProperties.ImageHeightInPixels / 2.0 || newY > ProjectionModelProperties.ImageHeightInPixels / 2.0)
                {
                    pointOutsideDrawingArea = true;
                    if (TrimPointsOutsideDrawingArea)
                    {
                        if (i == 0)
                        {
                            TrimLineInDrawingAreaAndProjectorOrigin(ref originalX, ref originalY, ref newX, ref newY);
                        }
                        else
                        {
                            NativeMethods.JMLaser.JMVectorStruct previousPoint = VectorImagePoints[i - 1];
                            double previousPointX = (double)previousPoint.x / _drawingAreaToProjectorRangeXScale;
                            double previousPointY = (double)previousPoint.y / _drawingAreaToProjectorRangeYScale;
                            TrimLineInDrawingAreaAndProjectorOrigin(ref previousPointX, ref previousPointY, ref newX, ref newY);
                        }
                    }
                }

                if (!pointOutsideDrawingArea || (pointOutsideDrawingArea && TrimPointsOutsideDrawingArea))
                {
                    newX *= _drawingAreaToProjectorRangeXScale;
                    newY *= _drawingAreaToProjectorRangeYScale;

                    point.x = (Int32) newX;
                    point.y = (Int32) newY;

                    if (newX <= MinimumProjectorRangeValueForClipping)
                        point.x = MinimumProjectorRangeValueForClipping;
                    else if (newX >= MaximumProjectorRangeValueForClipping)
                        point.x = MaximumProjectorRangeValueForClipping;

                    if (newY <= MinimumProjectorRangeValueForClipping)
                        point.y = MinimumProjectorRangeValueForClipping;
                    else if (newY >= MaximumProjectorRangeValueForClipping)
                        point.y = MaximumProjectorRangeValueForClipping;

                    if (TrimPointsOutsideDrawingArea)
                        VectorImagePoints[i] = point;
                    else
                        VectorImagePointsTrimmed.Add(point);
                }
                else
                {
                    if (point.i == 0 && VectorImagePointsTrimmed.Count > 0)
                    {
                        NativeMethods.JMLaser.JMVectorStruct previousPoint = VectorImagePointsTrimmed.Last();
                        previousPoint.i = 0;
                        VectorImagePointsTrimmed.Add(previousPoint);
                    }
                }
            }

            if (!TrimPointsOutsideDrawingArea)
            {
                VectorImagePoints = VectorImagePointsTrimmed;
            }
        }

        public static void CorrectGalvanometerMirrorsDistortion(ref double x, ref double y, double distanceBetweenMirrors, double distanceToImagePlane, double distanceToImagePlaneForConvertingXGalvoAngleToDrawingArea, double distanceToImagePlaneForConvertingYGalvoAngleToDrawingArea, bool useRayToPlaneIntersectionForConvertingGalvosAnglesToDrawingArea)
        {
            DrawingAreaToGalvoAngles(x, y, distanceBetweenMirrors, distanceToImagePlane, out double galvoXAngle, out double galvoYAngle);
            PinHoleAnglesToDrawingArea(galvoXAngle, galvoYAngle, distanceToImagePlaneForConvertingXGalvoAngleToDrawingArea, distanceToImagePlaneForConvertingYGalvoAngleToDrawingArea, out x, out y, useRayToPlaneIntersectionForConvertingGalvosAnglesToDrawingArea);

            // Reverse of above
            //DrawingAreaToPinHoleAngles(x, y, distanceToImagePlaneForConvertingXGalvoAngleToDrawingArea, distanceToImagePlaneForConvertingYGalvoAngleToDrawingArea, out double xAngle, out double yAngle, useRayToPlaneIntersectionForConvertingGalvosAnglesToDrawingArea);
            //GalvoAnglesToDrawingArea(xAngle, yAngle, distanceBetweenMirrors, distanceToImagePlane, out x, out y);
        }

        public static void DrawingAreaToGalvoAngles(double x, double y, double distanceBetweenMirrors, double distanceToImagePlane, out double galvoXAngle, out double galvoYAngle)
        {
            double yDenominator = distanceToImagePlane - distanceBetweenMirrors;
            galvoYAngle = Math.Atan(y / yDenominator);
            double xDenominator = distanceBetweenMirrors * (((distanceToImagePlane - distanceBetweenMirrors) / (Math.Cos(galvoYAngle) * distanceBetweenMirrors)) + 1);
            galvoXAngle = Math.Atan(x / xDenominator);
        }

        public static void GalvoAnglesToDrawingArea(double galvoXAngle, double galvoYAngle, double distanceBetweenMirrors, double distanceToImagePlane, out double x, out double y)
        {
            x = Math.Tan(galvoXAngle) * distanceBetweenMirrors * (((distanceToImagePlane - distanceBetweenMirrors) / (Math.Cos(galvoYAngle) * distanceBetweenMirrors)) + 1);
            y = Math.Tan(galvoYAngle) * (distanceToImagePlane - distanceBetweenMirrors);
        }

        public static void DrawingAreaToPinHoleAngles(double x, double y, double distanceToImagePlaneForX, double distanceToImagePlaneForY, out double xAngle, out double yAngle, bool useRayToPlaneIntersectionForConvertingGalvosAnglesToDrawingArea)
        {
            if (useRayToPlaneIntersectionForConvertingGalvosAnglesToDrawingArea)
            {
                xAngle = Math.Atan(x / distanceToImagePlaneForX);
                yAngle = Math.Atan(y / distanceToImagePlaneForY);
            }
            else
            {
                xAngle = Math.Asin(x / distanceToImagePlaneForX);
                yAngle = Math.Asin(y / distanceToImagePlaneForY);
            }
        }

        public static void PinHoleAnglesToDrawingArea(double galvoXAngle, double galvoYAngle, double distanceToImagePlaneForUpdatingX, double distanceToImagePlaneForUpdatingY, out double x, out double y, bool useRayToPlaneIntersectionForConvertingGalvosAnglesToDrawingArea)
        {
            // Pinhole angles to screen (TODO: improve conversion because galvos do not have a center of projection)
            if (useRayToPlaneIntersectionForConvertingGalvosAnglesToDrawingArea)
            {
                x = Math.Tan(galvoXAngle) * distanceToImagePlaneForUpdatingX;
                y = Math.Tan(galvoYAngle) * distanceToImagePlaneForUpdatingY;
            }
            else
            {
                x = Math.Sin(galvoXAngle) * distanceToImagePlaneForUpdatingX;
                y = Math.Sin(galvoYAngle) * distanceToImagePlaneForUpdatingY;
            }
        }

        public static double ComputeDistanceToImagePlane(double focalLengthInPixels, double imageSizeInPixels)
        {
            double fov = ComputeFieldOfView(focalLengthInPixels, imageSizeInPixels);
            return (imageSizeInPixels / 2.0) / Math.Tan(fov / 2);
        }

        public static double ComputeFieldOfView(double focalLengthInPixels, double imageSizeInPixels)
        {
            return Math.Atan(imageSizeInPixels / (focalLengthInPixels * 2.0)) * 2.0;
        }

        public static void ComputeScalingFactorsFromImagePlaneToDrawingArea(double focalLengthInPixelsX, double imageSizeInPixelsX, double focalLengthInPixelsY, double imageSizeInPixelsY, double distanceBetweenMirrors, double distanceToImagePlane, out double xScale, out double yScale)
        {
            double fovX = ComputeFieldOfView(focalLengthInPixelsX, imageSizeInPixelsX);
            double fovY = ComputeFieldOfView(focalLengthInPixelsY, imageSizeInPixelsY);
            GalvoAnglesToDrawingArea(fovX / 2.0, fovY / 2.0, distanceBetweenMirrors, distanceToImagePlane, out double x, out double y);
            xScale = imageSizeInPixelsX / x;
            yScale = imageSizeInPixelsY / y;
        }

        public static double ChangeFromDrawingAreaOriginToPrincipalPointOrigin(double drawingAreaValue, double imageSize, double principalPoint)
        {
            return (drawingAreaValue + (imageSize / 2.0)) - principalPoint;
        }

        public static double ChangeFromPrincipalPointOriginToDrawingAreaOrigin(double drawingAreaValue, double imageSize, double principalPoint)
        {
            return (drawingAreaValue - (imageSize / 2.0)) + principalPoint;
        }

        public static void CorrectLensDistortion(ref double x, ref double y, ProjectionModelProperties projectionModelProperties)
        {
            double normalizedX = ChangeFromDrawingAreaOriginToPrincipalPointOrigin(x, projectionModelProperties.ImageWidthInPixels, projectionModelProperties.PrincipalPointXInPixels) / projectionModelProperties.FocalLengthXInPixels;
            double normalizedY = ChangeFromDrawingAreaOriginToPrincipalPointOrigin(y, projectionModelProperties.ImageHeightInPixels, projectionModelProperties.PrincipalPointYInPixels) / projectionModelProperties.FocalLengthYInPixels;
            double distanceSquared = normalizedX * normalizedX + normalizedY * normalizedY;

            double radialDistortionCorrectionScalingFactor = (1 +
                projectionModelProperties.RadialDistortionCorrectionFirstCoefficient * distanceSquared + 
                projectionModelProperties.RadialDistortionCorrectionSecondCoefficient * distanceSquared * distanceSquared +
                projectionModelProperties.RadialDistortionCorrectionThirdCoefficient * distanceSquared * distanceSquared * distanceSquared);

            double tangentialDistortionCorrectionOffsetFactorX = 2 * projectionModelProperties.TangentialDistortionCorrectionFirstCoefficient * normalizedX * normalizedY + 
                projectionModelProperties.TangentialDistortionCorrectionSecondCoefficient * (distanceSquared + 2 * normalizedX * normalizedX);
            double tangentialDistortionCorrectionOffsetFactorY = projectionModelProperties.TangentialDistortionCorrectionFirstCoefficient * (distanceSquared + 2 * normalizedY * normalizedY) + 
                2 * projectionModelProperties.TangentialDistortionCorrectionSecondCoefficient * normalizedX * normalizedY;

            double normalizedXUndistorted = normalizedX * radialDistortionCorrectionScalingFactor + tangentialDistortionCorrectionOffsetFactorX;
            double normalizedYUndistorted = normalizedY * radialDistortionCorrectionScalingFactor + tangentialDistortionCorrectionOffsetFactorY;

            x = ChangeFromPrincipalPointOriginToDrawingAreaOrigin(normalizedXUndistorted * projectionModelProperties.FocalLengthXInPixels, projectionModelProperties.ImageWidthInPixels, projectionModelProperties.PrincipalPointXInPixels);
            y = ChangeFromPrincipalPointOriginToDrawingAreaOrigin(normalizedYUndistorted * projectionModelProperties.FocalLengthYInPixels, projectionModelProperties.ImageHeightInPixels, projectionModelProperties.PrincipalPointYInPixels);
        }


        public static bool LineIntersection(double p0X, double p0Y, double p1X, double p1Y,
            double p2X, double p2Y, double p3X, double p3Y,
            ref double iX, ref double iY, double comparisonEpsilon = 1e-8)
        {
            double s02X, s02Y, s10X, s10Y, s32X, s32Y, tNumerator, denominator, t;
            s10X = p1X - p0X;
            s10Y = p1Y - p0Y;
            s32X = p3X - p2X;
            s32Y = p3Y - p2Y;

            denominator = s10X * s32Y - s32X * s10Y;
            if (Math.Abs(denominator) < comparisonEpsilon)
                return false; // Collinear

            s02X = p0X - p2X;
            s02Y = p0Y - p2Y;

            tNumerator = s32X * s02Y - s32Y * s02X;

            // Collision detected
            t = tNumerator / denominator;
            iX = p0X + (t * s10X);
            iY = p0Y + (t * s10Y);

            return true;
        }

        public static double LinearInterpolation(double a, double b, double t)
        {
            return a * (1 - t) + b * t;
        }
    }
}
