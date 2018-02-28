using SolarSystemCore.SharedKernel;
using System.Collections.Generic;

namespace SolarSystemCore.Core.Entities
{
    public class Star : BaseEntity
    {
        public virtual ICollection<Planet> Planets { get; set; } = new HashSet<Planet>();
    }
}
