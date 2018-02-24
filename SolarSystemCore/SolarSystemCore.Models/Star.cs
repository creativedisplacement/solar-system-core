using System.Collections.Generic;

namespace SolarSystemCore.Models
{
    public class Star : BaseEntity
    {
        public virtual ICollection<Planet> Planets { get; set; } = new HashSet<Planet>();
    }
}
