namespace FuegoDeQuasar.Model
{
    public interface IPoint
    {
        public double DistanceTo(IPoint b);

        public string ToString();

        //public IPoint Trilateration(ISatellite p1, double? r1, ISatellite p2, double? r2, ISatellite p3, double? r3);
    }
}