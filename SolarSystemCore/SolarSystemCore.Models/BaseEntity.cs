using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolarSystemCore.Models
{
    public abstract class BaseEntity
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string SmallImage { get; set; }

        public int? Ordinal { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; } = DateTime.Now;
    }
}
