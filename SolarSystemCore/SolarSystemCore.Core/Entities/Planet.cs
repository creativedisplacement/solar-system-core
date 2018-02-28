using SolarSystemCore.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolarSystemCore.Core.Entities
{
    public class Planet : BaseEntity
    {
        [ForeignKey("StarId")]
        public virtual Star Star { get; set; }

        [Required]
        public Guid StarId { get; set; }
        public virtual ICollection<Moon> Moons { get; set; } = new HashSet<Moon>();
    }
}
