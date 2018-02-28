using SolarSystemCore.Core.Interfaces;

namespace SolarSystemCore.Core.Settings
{
    public class AppSettings : IAppSettings
    {
        public int FailureThreshold { get; set; }
        public int OpenCircuitTimeout { get; set; }
    }
}
