namespace SolarSystemCore.Core.Interfaces
{
    public interface IAppSettings
    {
        int FailureThreshold { get; set; }
        int OpenCircuitTimeout { get; set; }
    }
}
