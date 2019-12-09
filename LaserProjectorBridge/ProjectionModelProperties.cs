namespace LaserProjectorBridge
{
    public class ProjectionModelProperties
    {
        public double ImageWidthInPixels { get; set; } = 2000.0;
        public double ImageHeightInPixels { get; set; } = 2000.0;
        public double FocalLengthXInPixels { get; set; } = 2830.0;
        public double FocalLengthYInPixels { get; set; } = 2800.0;
        public double PrincipalPointXInPixels { get; set; } = 1000.0;
        public double PrincipalPointYInPixels { get; set; } = 1000.0;
        public double DistanceBetweenMirrors { get; set; } = 0.001;
        public double DistanceToImagePlaneForCorrectingDistortion { get; set; } = 2830.0;
        public double DistanceToImagePlaneForConvertingXGalvoAngleToDrawingArea { get; set; } = 2830.0;
        public double DistanceToImagePlaneForConvertingYGalvoAngleToDrawingArea { get; set; } = 2800.0;
        public bool ComputeDistancesToImagePlanes { get; set; } = false;
        public bool ScaleImagePlanePointsUsingIntrinsics { get; set; } = false;
        public bool ChangeToPrincipalPointOriginWhenCorrectingGalvanometerDistortion { get; set; } = true;
        public bool UseRayToPlaneIntersectionForConvertingGalvosAnglesToDrawingArea { get; set; } = false;
        public double RadialDistortionCorrectionFirstCoefficient { get; set; } = 0.0;
        public double RadialDistortionCorrectionSecondCoefficient { get; set; } = 0.0;
        public double RadialDistortionCorrectionThirdCoefficient { get; set; } = 0.0;
        public double TangentialDistortionCorrectionFirstCoefficient { get; set; } = 0.0;
        public double TangentialDistortionCorrectionSecondCoefficient { get; set; } = 0.0;

        public bool HasLensDistortionCoefficients()
        {
            return (RadialDistortionCorrectionFirstCoefficient != 0.0 ||
                    RadialDistortionCorrectionSecondCoefficient != 0.0 ||
                    RadialDistortionCorrectionThirdCoefficient != 0.0 ||
                    TangentialDistortionCorrectionFirstCoefficient != 0.0 ||
                    TangentialDistortionCorrectionSecondCoefficient != 0.0);
        }
    }
}
