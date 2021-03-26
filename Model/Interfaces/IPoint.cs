namespace FuegoDeQuasar.Model.Interfaces
{
    public interface IPoint
    {
        public double DistanceTo(IPoint b);

        public string ToString();
    }
}