namespace SolarSystemCore.Core
{
    public interface IAppSettings
    {
        int FailureThreshold { get; set; }
        int OpenCircuitTimeout { get; set; }
    }
}
