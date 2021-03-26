using FuegoDeQuasar.Model.Interfaces;

namespace FuegoDeQuasar.Model
{
    public class Satellite : ISatellite
    {
        public Satellite()
        {
            this.Name = "";
            this.Coordinates = new Point2D();
        }

        public IPoint Coordinates { get; set; }
        public string Name { get; set; }

        public string GetName() => Name;

        public IPoint GetCoords() => Coordinates;

        public double DistanceTo(ISatellite satellite)
        {
            return Coordinates.DistanceTo(((Satellite)satellite).Coordinates);
        }

        public override string ToString()
        {
            return $"{{Name: {Name}, Coordinates: {Coordinates.ToString()}}}";
        }

        public double DistanceToPoint(IPoint point)
        {
            return Coordinates.DistanceTo(point);
        }
    }
}