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
                new Star { Name = "Star 1", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1},
                new Star { Name = "Star 2", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2},
            };

            foreach (var star in stars)
            {
                context.Stars.Add(star);
            }
            context.SaveChanges();

            var planets = new List<Planet>
            {
                new Planet { Name = "Planet 1", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1, StarId = 1 },
                new Planet { Name = "Planet 2", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2, StarId = 2 },
            };

            foreach (var planet in planets)
            {
                context.Planets.Add(planet);
            }
            context.SaveChanges();

            var moons = new List<Moon>
            {
                new Moon { Name = "Moon 1", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1, PlanetId = 1 },
                new Moon { Name = "Moon 2", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2, PlanetId = 1 },
            };

            foreach (var moon in moons)
            {
                context.Moons.Add(moon);
            }
            context.SaveChanges();
        }
    }
}
