using SolarSystemCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolarSystemCore.Data
{
    public static class SeedData
    {
        public static void Initialize(DBContext context)
        {
            context.Database.EnsureCreated();

            if (context.Stars.Any())
            {
                return;
            }

            var stars = new List<Star>
            {
                new Star { Id = new Guid("DF9AA280-C912-4E42-A5B5-4573CF97FDB0"), Name = "Star 1", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1},
                new Star { Id = new Guid("591D922F-D11C-469F-B61B-AF783D71E60A"), Name = "Star 2", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2},
            };

            foreach (var star in stars)
            {
                context.Stars.Add(star);
            }
            context.SaveChanges();

            var planets = new List<Planet>
            {
                new Planet {Id = new Guid("576B0C15-0010-4186-B45F-9E98B9E3F56B"), Name = "Planet 1", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1, StarId = new Guid("DF9AA280-C912-4E42-A5B5-4573CF97FDB0") },
                new Planet {Id = new Guid("9720021D-A5F7-4B52-A0C6-C89CC73830A7"), Name = "Planet 2", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2, StarId = new Guid("591D922F-D11C-469F-B61B-AF783D71E60A") },
            };

            foreach (var planet in planets)
            {
                context.Planets.Add(planet);
            }
            context.SaveChanges();

            var moons = new List<Moon>
            {
                new Moon {Id = new Guid("29E55F9D-D713-4237-BC2A-3BC5812ED2FD"), Name = "Moon 1", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1, PlanetId = new Guid("576B0C15-0010-4186-B45F-9E98B9E3F56B") },
                new Moon {Id = new Guid("B81A5D0F-988B-4EB2-AC2D-E64A3DF780AE"), Name = "Moon 2", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2, PlanetId = new Guid("9720021D-A5F7-4B52-A0C6-C89CC73830A7") },
            };

            foreach (var moon in moons)
            {
                context.Moons.Add(moon);
            }
            context.SaveChanges();
        }
    }
}
