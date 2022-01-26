namespace FuegoDeQuasar.Model.Interfaces
{
    public interface ISatellite
    {
        public double DistanceTo(ISatellite satellite);

        public double DistanceToPoint(IPoint point);

        public IPoint GetCoords();

        public string GetName();

        public string ToString();
    }
}