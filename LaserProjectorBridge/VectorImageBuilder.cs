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
        public double LineFirstPointMergeDistanceSquaredInProjectorRange { get; set; } = Math.Pow(UInt32.MaxValue * 0.001, 2);
        //public Int32 InterpolationDistanceInProjectorRange { get; set; } = (Int32)(UInt32.MaxValue * 0.002);
        //public Int32 BlankingDistanceInProjectorRange { get; set; } = (Int32)(UInt32.MaxValue * 0.002);
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

        public bool ConvertPointFromDrawingAreaToProjectorRange(double x, double y, out Int32 xPointInProjectorRange, out Int32 yPointInProjectorRange, AxisPosition originAxisPosition = AxisPosition.BottomLeft)
        {
            double xPointInDrawingAreaAndProjectorOrigin;
            double yPointInDrawingAreaAndProjectorOrigin;
            try
            {
                checked { ConvertPointOriginToProjectorOrigin(x, y, out xPointInDrawingAreaAndProjectorOrigin, out yPointInDrawingAreaAndProjectorOrigin, originAxisPosition); }
            }
            catch (System.OverflowException)
            {
                xPointInProjectorRange = 0;
                yPointInProjectorRange = 0;
                return false;
            }

            bool pointOverflow = false;

            try
            {
                checked { xPointInProjectorRange = (Int32)((xPointInDrawingAreaAndProjectorOrigin - DrawingAreaXOffset) * _drawingAreaToProjectorRangeXScale); }
            }
            catch (System.OverflowException)
            {
                xPointInProjectorRange = xPointInDrawingAreaAndProjectorOrigin - DrawingAreaXOffset < 0 ? Int32.MinValue : Int32.MaxValue;
                pointOverflow = true;
            }

            try
            {
                checked { yPointInProjectorRange = (Int32)((yPointInDrawingAreaAndProjectorOrigin - DrawingAreaYOffset) * _drawingAreaToProjectorRangeYScale); }
            }
            catch (System.OverflowException)
            {
                yPointInProjectorRange = yPointInDrawingAreaAndProjectorOrigin - DrawingAreaYOffset < 0 ? Int32.MinValue : Int32.MaxValue;
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

        public bool AddNewLine(double startX, double startY, double endX, double endY,
            UInt16 red = UInt16.MaxValue, UInt16 green = UInt16.MaxValue, UInt16 blue = UInt16.MaxValue,
            UInt16 intensity = UInt16.MaxValue, AxisPosition originAxisPosition = AxisPosition.BottomLeft)
        {
            Int32 startXInProjectorRange;
            Int32 startYInProjectorRange;
            Int32 endXInProjectorRange;
            Int32 endYInProjectorRange;
            bool lineTrimmed = !ConvertPointFromDrawingAreaToProjectorRange(startX, startY, out startXInProjectorRange, out startYInProjectorRange, originAxisPosition);
            if (!ConvertPointFromDrawingAreaToProjectorRange(endX, endY, out endXInProjectorRange, out endYInProjectorRange, originAxisPosition)) { lineTrimmed = true; }
            AddNewLine(startXInProjectorRange, startYInProjectorRange, endXInProjectorRange, endYInProjectorRange, red, green, blue, intensity);
            return !lineTrimmed;
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
                double distanceToLastPointSquared = Math.Pow(startPoint.x - lastPoint.x, 2) + Math.Pow(startPoint.y - lastPoint.y, 2);
                if (distanceToLastPointSquared >= LineFirstPointMergeDistanceSquaredInProjectorRange ||
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
                    lastPoint.x = (Int32)((lastPoint.x + startPoint.x) / 2.0);
                    lastPoint.y = (Int32)((lastPoint.y + startPoint.y) / 2.0);
                    ReplaceLastPoint(ref lastPoint);
                }
            }
            else
            {
                AddNewPoint(ref startPoint);
            }

            // todo: add line interpolation
            AddNewPoint(ref endPoint);
        }

        public bool AddNewPoint(double x, double y, 
            UInt16 red = UInt16.MaxValue, UInt16 green = UInt16.MaxValue, UInt16 blue = UInt16.MaxValue,
            UInt16 intensity = UInt16.MaxValue, AxisPosition originAxisPosition = AxisPosition.BottomLeft)
        {
            Int32 xInProjectorRange;
            Int32 yInProjectorRange;
            if (ConvertPointFromDrawingAreaToProjectorRange(x, y, out xInProjectorRange, out yInProjectorRange, originAxisPosition))
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

        public void ReplaceLastPoint(ref NativeMethods.JMLaser.JMVectorStruct point)
        {
            VectorImagePoints.RemoveAt(VectorImagePoints.Count - 1);
            VectorImagePoints.Add(point);
        }
    }
}
