namespace FuegoDeQuasar.Model
{
    public interface ISatellite
    {
        public string GetName();

        public IPoint GetCoords();

        public double DistanceTo(ISatellite satellite);

        public double DistanceToPoint(IPoint point);

        public string ToString();
    }
}