namespace SmartHome.Domain.Simulations;

public interface ISimulationService
{
    void SetAmbientTemperature(string location, int temperature);
    double GetAmbientTemperature(string location);
}