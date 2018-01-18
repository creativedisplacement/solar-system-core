using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolarSystemCore.Models
{
    public partial class Moon : BaseEntity
    {
        [ForeignKey("PlanetId")]
        public virtual Planet Planet { get; set; }

        [Required]
        public int PlanetId { get; set; }

        public string Test { get; set; }
    }
}
