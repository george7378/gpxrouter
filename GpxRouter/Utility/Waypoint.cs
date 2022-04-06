using System;

namespace GpxRouter.Utility
{
    public class Waypoint
    {
        #region Properties

        public string Name { get; set; }

        public double LatitudeDegrees { get; set; }

        public double LongitudeDegrees { get; set; }

        #endregion

        #region Constructors

        public Waypoint(string name, double latitudeDegrees, double longitudeDegrees)
        {
            Name = name;
            LatitudeDegrees = latitudeDegrees;
            LongitudeDegrees = longitudeDegrees;
        }

        #endregion

        #region Methods

        public float DistanceToMetres(Waypoint waypoint)
        {
            double halfDeltaLatitudeRadians = Global.ToRadians(waypoint.LatitudeDegrees - LatitudeDegrees)/2;
            double halfDeltaLongitudeRadians = Global.ToRadians(waypoint.LongitudeDegrees - LongitudeDegrees)/2;

            double a = Math.Sin(halfDeltaLatitudeRadians)*Math.Sin(halfDeltaLatitudeRadians) + Math.Cos(Global.ToRadians(LatitudeDegrees))*Math.Cos(Global.ToRadians(waypoint.LatitudeDegrees))*Math.Sin(halfDeltaLongitudeRadians)*Math.Sin(halfDeltaLongitudeRadians);

            return (float)(2*Global.EarthRadiusMetres*Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)));
        }

        public float BearingToDegrees(Waypoint waypoint)
        {
            double latitudeRadians = Global.ToRadians(LatitudeDegrees);
            double targetLatitudeRadians = Global.ToRadians(waypoint.LatitudeDegrees);
            double deltaLongitudeRadians = Global.ToRadians(waypoint.LongitudeDegrees - LongitudeDegrees);

            double bearingRadians = Math.Atan2(Math.Sin(deltaLongitudeRadians)*Math.Cos(targetLatitudeRadians), Math.Cos(latitudeRadians)*Math.Sin(targetLatitudeRadians) - Math.Sin(latitudeRadians)*Math.Cos(targetLatitudeRadians)*Math.Cos(deltaLongitudeRadians));

            return (float)((Global.ToDegrees(bearingRadians) + 360) % 360);
        }

        #endregion
    }
}
