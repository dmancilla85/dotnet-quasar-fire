using FuegoDeQuasar.Model.Requests;
using FuegoDeQuasar.Model.Response;

namespace FuegoDeQuasar.Services
{
    public interface ITopSecretService
    {
        public FinalResponse GetFinalResponse(SecretTransmission secret);

        public FinalResponse GetFinalResponseFromSplit(string satellite, SecretTransmissionSplit secret);
    }
}
