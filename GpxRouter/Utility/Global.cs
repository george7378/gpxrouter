using System;

namespace GpxRouter.Utility
{
    public static class Global
    {
        #region Constants

        public const float EarthRadiusMetres = 6371000;
        public const float MetresToNauticalMiles = 0.000539957f;

        #endregion

        #region Methods

        public static double ToRadians(double angle)
        {
            return angle*Math.PI/180;
        }

        public static double ToDegrees(double angle)
        {
            return angle*180/Math.PI;
        }

        public static (int AbsoluteRoundedDegrees, int AbsoluteRoundedMinutes) GetAbsoluteRoundedDegreesAndMinutes( double signedDecimalAngle )
        {
            int absoluteRoundedDegrees = Math.Abs( (int) Math.Truncate( signedDecimalAngle ) );
            int absoluteRoundedMinutes = Math.Abs( (int) Math.Round( ( signedDecimalAngle - Math.Truncate( signedDecimalAngle ) ) * 60 ) );

            if ( absoluteRoundedMinutes == 60 )
            {
                absoluteRoundedDegrees += 1;
                absoluteRoundedMinutes = 0;
            }

            return ( absoluteRoundedDegrees, absoluteRoundedMinutes );
        }

        #endregion
    }
}
