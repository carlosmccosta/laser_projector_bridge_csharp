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
        public double DrawingAreaWidth { get; set; } = 2000.0;
        public double DrawingAreaHeight { get; set; } = 2000.0;
        public double DrawingAreaXOffset { get; set; } = 0.0;
        public double DrawingAreaYOffset { get; set; } = 0.0;
        public double RadialDistortionCoefficientSecondDegreeInvertedUV { get; set; } = 0.08;
        public double RadialDistortionCoefficientSecondDegree { get; set; } = -0.044;
        public double RadialDistortionCoefficientFourthDegree { get; set; } = -0.007;
        public double RadialDistortionCoefficientSixthDegree { get; set; } = -0.005;
        public double LineFirstPointMergeDistanceSquaredInProjectorRange { get; set; } = Math.Pow(UInt32.MaxValue * 0.0005, 2);
        public double LineFirstPointIgnoreDistanceSquaredInProjectorRange { get; set; } = Math.Pow(UInt32.MaxValue * 0.001, 2);
        public Int32 InterpolationDistanceInProjectorRange { get; set; } = (Int32)(UInt32.MaxValue * 0.002);
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
            _drawingAreaToProjectorRangeXScale = ((double)UInt32.MaxValue) / DrawingAreaWidth;
            _drawingAreaToProjectorRangeYScale = ((double)UInt32.MaxValue) / DrawingAreaHeight;
        }

        public void FinishVectorImage()
        {
            if (VectorImagePoints.Count > 0) {
                NativeMethods.JMLaser.JMVectorStruct lastPoint = VectorImagePoints.Last();
                if (lastPoint.i != 0)
                {
                    lastPoint.i = 0;
                    VectorImagePoints.Add(lastPoint);
                }
                CorrectRadialDistortionOnVectorImage();
                // todo: laser path optimization and blanking
            }
        }

        public void ConvertPointOriginToProjectorOrigin(double x, double y, out double newX, out double newY, AxisPosition pointOriginAxisPosition = AxisPosition.BottomLeft)
        {
            switch (pointOriginAxisPosition)
            {
                case AxisPosition.TopLeft:
                {
                    newX = x - DrawingAreaWidth * 0.5;
                    newY = (DrawingAreaHeight - y) - DrawingAreaHeight * 0.5;
                    break;
                }

                case AxisPosition.BottomLeft:
                {
                    newX = x - DrawingAreaWidth * 0.5;
                    newY = y - DrawingAreaHeight * 0.5;
                    break;
                }

                case AxisPosition.Middle:
                {
                    newX = x;
                    newY = y;
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(pointOriginAxisPosition), pointOriginAxisPosition, null);
            }
        }

        public void ConvertProjectorOriginToDrawingAreaOrigin(double x, double y, out double newX, out double newY, AxisPosition drawingAreaOriginAxisPosition = AxisPosition.BottomLeft)
        {
            switch (drawingAreaOriginAxisPosition)
            {
                case AxisPosition.TopLeft:
                    {
                        newX = x + DrawingAreaWidth * 0.5;
                        newY = (DrawingAreaHeight - y) + DrawingAreaHeight * 0.5;
                        break;
                    }

                case AxisPosition.BottomLeft:
                    {
                        newX = x + DrawingAreaWidth * 0.5;
                        newY = y + DrawingAreaHeight * 0.5;
                        break;
                    }

                case AxisPosition.Middle:
                    {
                        newX = x;
                        newY = y;
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
                checked { ConvertPointOriginToProjectorOrigin(x, y, out xPointInDrawingAreaAndProjectorOrigin, out yPointInDrawingAreaAndProjectorOrigin, originAxisPosition); }
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
                xPointInProjectorRange = xPointInDrawingAreaAndProjectorOrigin - DrawingAreaXOffset < DrawingAreaWidth * 0.5 ? Int32.MinValue : Int32.MaxValue;
                pointOverflow = true;
            }

            try
            {
                checked { yPointInProjectorRange = (Int32)((yPointInDrawingAreaAndProjectorOrigin - DrawingAreaYOffset) * _drawingAreaToProjectorRangeYScale); }
            }
            catch (System.OverflowException)
            {
                yPointInProjectorRange = yPointInDrawingAreaAndProjectorOrigin - DrawingAreaYOffset < DrawingAreaHeight * 0.5 ? Int32.MinValue : Int32.MaxValue;
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
                checked { ConvertProjectorOriginToDrawingAreaOrigin(xPointInDrawingAreaAndProjectorOrigin, yPointInDrawingAreaAndProjectorOrigin, out xPointInDrawingAreaRange, out yPointInDrawingAreaRange, originAxisPosition); }
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

            double halfWidth = DrawingAreaWidth * 0.5;
            double halfHeight = DrawingAreaHeight * 0.5;
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
            double halfWidth = DrawingAreaWidth * 0.5;
            double halfHeight = DrawingAreaHeight * 0.5;
            if (x >= -halfWidth && x <= halfWidth && y >= -halfHeight && y <= halfHeight)
                return true;
            else
                return false;
        }

        public static bool LineIntersection(double p0_x, double p0_y, double p1_x, double p1_y,
                                     double p2_x, double p2_y, double p3_x, double p3_y,
                                     ref double i_x, ref double i_y)
        {
            double s02_x, s02_y, s10_x, s10_y, s32_x, s32_y, t_numerator, denominator, t;
            s10_x = p1_x - p0_x;
            s10_y = p1_y - p0_y;
            s32_x = p3_x - p2_x;
            s32_y = p3_y - p2_y;

            denominator = s10_x * s32_y - s32_x * s10_y;
            if (denominator == 0)
                return false; // Collinear

            s02_x = p0_x - p2_x;
            s02_y = p0_y - p2_y;

            t_numerator = s32_x * s02_y - s32_y * s02_x;

            // Collision detected
            t = t_numerator / denominator;
            i_x = p0_x + (t * s10_x);
            i_y = p0_y + (t * s10_y);

            return true;
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

            AddNewLine(startXInProjectorRange, startYInProjectorRange, endXInProjectorRange, endYInProjectorRange, red, green, blue, intensity);
            return true;
        }

        public void AddNewLine(Int32 startX, Int32 startY, Int32 endX, Int32 endY,
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

            AddNewLine(ref startPoint, ref endPoint);
        }

        public void AddNewLine(ref NativeMethods.JMLaser.JMVectorStruct startPoint, ref NativeMethods.JMLaser.JMVectorStruct endPoint)
        {
            if (VectorImagePoints.Count > 0)
            {
                NativeMethods.JMLaser.JMVectorStruct lastPoint = VectorImagePoints.Last();
                double distanceToLastPointSquared = NativeMethods.JMLaser.JMVectorStructDistanceSquared(startPoint, lastPoint);
                if (distanceToLastPointSquared >= LineFirstPointIgnoreDistanceSquaredInProjectorRange ||
                    lastPoint.i != startPoint.i || lastPoint.r != startPoint.r || lastPoint.g != startPoint.g ||
                    lastPoint.b != startPoint.b ||
                    lastPoint.cyan != startPoint.cyan || lastPoint.deepblue != startPoint.deepblue ||
                    lastPoint.yellow != startPoint.yellow || lastPoint.user4 != startPoint.user4)
                {
                    if (lastPoint.i != 0)
                    {
                        lastPoint.i = 0;
                        AddNewPoint(ref lastPoint);
                    }

                    NativeMethods.JMLaser.JMVectorStruct startPointOff = startPoint;
                    startPointOff.i = 0;
                    AddNewPoint(ref startPointOff);
                    AddNewPoint(ref startPoint);
                }
                else
                {
                    if (distanceToLastPointSquared > 0 && distanceToLastPointSquared < LineFirstPointMergeDistanceSquaredInProjectorRange)
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
                AddNewPoint(ref startPoint);
            }

            AddNewPointWithLinearInterpolationFromLastPoint(ref endPoint);
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

        public void AddNewPoint(Int32 x, Int32 y, 
            UInt16 red = UInt16.MaxValue, UInt16 green = UInt16.MaxValue, UInt16 blue = UInt16.MaxValue, UInt16 intensity = UInt16.MaxValue)
        {
            var newPoint = new NativeMethods.JMLaser.JMVectorStruct()
            {
                x = x, y = y, r = red, g = green, b = blue, i = intensity, deepblue = UInt16.MaxValue, yellow = UInt16.MaxValue, cyan = UInt16.MaxValue, user4 = UInt16.MaxValue
            };

            AddNewPoint(ref newPoint);
        }

        public void AddNewPoint(ref NativeMethods.JMLaser.JMVectorStruct point)
        {
            if (VectorImagePoints.Count == 0)
            {
                NativeMethods.JMLaser.JMVectorStruct pointStartOff = point;
                pointStartOff.i = 0;
                VectorImagePoints.Add(pointStartOff);
            }
            VectorImagePoints.Add(point);
        }

        public void AddNewPointWithLinearInterpolationFromLastPoint(ref NativeMethods.JMLaser.JMVectorStruct point)
        {
            NativeMethods.JMLaser.JMVectorStruct lastPoint = VectorImagePoints.Last();
            double distanceToLastPoint = Math.Sqrt(NativeMethods.JMLaser.JMVectorStructDistanceSquared(lastPoint, point));
            int numberOfInterpolationPoints = (int)(distanceToLastPoint / (double)InterpolationDistanceInProjectorRange) - 1;

            if (numberOfInterpolationPoints > 0)
            {
                double tIncrement = 1.0 / (double) numberOfInterpolationPoints;
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

            AddNewPoint(ref point);
        }

        public static double LinearInterpolation(double a, double b, double t)
        {
            return a * (1 - t) + b * t;
        }

        public void CorrectRadialDistortion(ref NativeMethods.JMLaser.JMVectorStruct point)
        {
            if (RadialDistortionCoefficientSecondDegree != 0 || RadialDistortionCoefficientFourthDegree != 0 || RadialDistortionCoefficientSixthDegree != 0)
            {
                double u = (double)point.x / (double)System.Int32.MaxValue;
                double v = (double)point.y / (double)System.Int32.MaxValue;
                double uInverted = ((double)System.Int32.MaxValue - Math.Abs((double)point.x)) / (double)System.Int32.MaxValue;
                double vInverted = ((double)System.Int32.MaxValue - Math.Abs((double)point.y)) / (double)System.Int32.MaxValue;
                double r = System.Math.Pow(u, 2.0) * System.Math.Pow(v, 2.0);
                double rWithInvertedUV = System.Math.Pow(uInverted, 2.0) * System.Math.Pow(vInverted, 2.0);
                double warp = RadialDistortionCoefficientSecondDegreeInvertedUV * rWithInvertedUV +
                              RadialDistortionCoefficientSecondDegree * r +
                              RadialDistortionCoefficientFourthDegree * r * r +
                              RadialDistortionCoefficientSixthDegree  * r * r * r;
                point.x = (int)((1.0 + warp) * (double)point.x);
            }
        }

        public void CorrectRadialDistortionOnVectorImage()
        {
            for (int i = 0; i < VectorImagePoints.Count; ++i)
            {
                NativeMethods.JMLaser.JMVectorStruct point = VectorImagePoints[i];
                CorrectRadialDistortion(ref point);
                VectorImagePoints[i] = point;
            }
        }

        public void ReplaceLastPoint(ref NativeMethods.JMLaser.JMVectorStruct point)
        {
            RemoveLastPoint();
            VectorImagePoints.Add(point);
        }

        public void RemoveLastPoint()
        {
            VectorImagePoints.RemoveAt(VectorImagePoints.Count - 1);
        }
    }
}
