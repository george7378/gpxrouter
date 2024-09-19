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

        public string ToIcaoFplRoutePointString()
        {
            // Latitude part

            (int AbsoluteRoundedDegrees, int AbsoluteRoundedMinutes) latitudeAbsoluteRoundedDegreesAndMinutes =
                Global.GetAbsoluteRoundedDegreesAndMinutes( LatitudeDegrees );

            string latitudeSuffix;
            if ( latitudeAbsoluteRoundedDegreesAndMinutes.Equals( ( 0, 0 ) ) )
            {
                latitudeSuffix = "N";
            }
            else
            {
                switch ( Math.Sign( LatitudeDegrees ) )
                {
                    case -1:
                        latitudeSuffix = "S";
                        break;

                    case 0:
                    case 1:
                        latitudeSuffix = "N";
                        break;

                    default:
                        throw new InvalidOperationException( nameof(LatitudeDegrees) );
                }
            }

            string latitudeIcaoString = $"{latitudeAbsoluteRoundedDegreesAndMinutes.AbsoluteRoundedDegrees:D2}{latitudeAbsoluteRoundedDegreesAndMinutes.AbsoluteRoundedMinutes:D2}{latitudeSuffix}";

            if ( latitudeIcaoString.Length != 5 ) throw new InvalidOperationException( nameof(latitudeIcaoString) );

            // Longitude part

            (int AbsoluteRoundedDegrees, int AbsoluteRoundedMinutes) longitudeAbsoluteRoundedDegreesAndMinutes =
                Global.GetAbsoluteRoundedDegreesAndMinutes( LongitudeDegrees );

            string longitudeSuffix;
            if ( longitudeAbsoluteRoundedDegreesAndMinutes.Equals( ( 0, 0 ) ) )
            {
                longitudeSuffix = "E";
            }
            else
            {
                switch ( Math.Sign( LongitudeDegrees ) )
                {
                    case -1:
                        longitudeSuffix = "W";
                        break;

                    case 0:
                    case 1:
                        longitudeSuffix = "E";
                        break;

                    default:
                        throw new InvalidOperationException( nameof(LongitudeDegrees) );
                }
            }

            string longitudeIcaoString = $"{longitudeAbsoluteRoundedDegreesAndMinutes.AbsoluteRoundedDegrees:D3}{longitudeAbsoluteRoundedDegreesAndMinutes.AbsoluteRoundedMinutes:D2}{longitudeSuffix}";

            if ( longitudeIcaoString.Length != 6 ) throw new InvalidOperationException( nameof(longitudeIcaoString) );

            // Output

            var result = $"{latitudeIcaoString}{longitudeIcaoString}";

            return result;
        }

        #endregion
    }
}
