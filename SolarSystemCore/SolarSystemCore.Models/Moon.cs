using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolarSystemCore.Models
{
    public class Moon : BaseEntity
    {
        [ForeignKey("PlanetId")]
        public virtual Planet Planet { get; set; }

        [Required]
        public Guid PlanetId { get; set; }
    }
}
