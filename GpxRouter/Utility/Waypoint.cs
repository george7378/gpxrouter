
namespace GpxRouter.Utility
{
    public class Waypoint
    {
        #region Properties

        public string Name { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        #endregion

        #region Constructors

        public Waypoint(string name, float latitude, float longitude)
        {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
        }

        #endregion
    }
}
