namespace SolarSystemCore.Core
{
    public class AppSettings : IAppSettings
    {
        public int FailureThreshold { get; set; }
        public int OpenCircuitTimeout { get; set; }
    }
}
