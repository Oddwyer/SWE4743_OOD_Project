
namespace SmartHome.Domain.Services;

public interface ISimulationService
{
    void SetAmbientTemperature(string location, int temperature);
    double GetAmbientTemperature(string location);
}