namespace FuegoDeQuasar.Model
{
    public interface IPoint
    {
        public double DistanceTo(IPoint b);

        public string ToString();
    }
}