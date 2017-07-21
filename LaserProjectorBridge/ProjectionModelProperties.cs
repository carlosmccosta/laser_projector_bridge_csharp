namespace LaserProjectorBridge
{
    public class ProjectionModelProperties
    {
        public double ImageWidthInPixels { get; set; } = 2000.0;
        public double ImageHeightInPixels { get; set; } = 2000.0;
        public double FocalLengthXInPixels { get; set; } = 3000.0;
        public double FocalLengthYInPixels { get; set; } = 3000.0;
        public double PrincipalPointXInPixels { get; set; } = 1000.0;
        public double PrincipalPointYInPixels { get; set; } = 1000.0;
        public double DistanceBetweenMirrorsInProjectorRangePercentage { get; set; } = 0.01;
        public double RadialDistortionCorrectionFirstCoefficient { get; set; } = 0.0;
        public double RadialDistortionCorrectionSecondCoefficient { get; set; } = 0.0;
        public double RadialDistortionCorrectionThirdCoefficient { get; set; } = 0.0;
        public double TangencialDistortionCorrectionFirstCoefficient { get; set; } = 0.0;
        public double TangencialDistortionCorrectionSecondCoefficient { get; set; } = 0.0;

        public bool HasLensDistortionCoefficients()
        {
            return (RadialDistortionCorrectionFirstCoefficient != 0.0 ||
                    RadialDistortionCorrectionSecondCoefficient != 0.0 ||
                    RadialDistortionCorrectionThirdCoefficient != 0.0 ||
                    TangencialDistortionCorrectionFirstCoefficient != 0.0 ||
                    TangencialDistortionCorrectionSecondCoefficient != 0.0);
        }
    }
}
