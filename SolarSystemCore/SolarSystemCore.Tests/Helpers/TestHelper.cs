using SolarSystemCore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolarSystemCore.Tests.Helpers
{
    public class TestHelper
    {
        public class StarData
        {
            public List<Star> GetStars()
            {
                return new List<Star>
                {
                    new Star
                    {
                        Id = Guid.Parse("DF9AA280-C912-4E42-A5B5-4573CF97FDB0"),
                        Name = "Star 1",
                        CreatedDate = DateTime.Now,
                        LastUpdatedDate = DateTime.Now,
                        Ordinal = 1,
                        Planets = new List<Planet>
                        {
                          new Planet
                          {
                              Id = Guid.NewGuid(),
                              Name = "Planet 1",
                              CreatedDate = DateTime.Now,
                              LastUpdatedDate = DateTime.Now,
                              StarId = Guid.Parse("DF9AA280-C912-4E42-A5B5-4573CF97FDB0")
                          }
                        }
                    },
                    new Star
                    {
                        Id = Guid.Parse("591D922F-D11C-469F-B61B-AF783D71E60A"),
                        Name = "Star 2",
                        CreatedDate = DateTime.Now,
                        LastUpdatedDate = DateTime.Now,
                        Ordinal = 2,
                        Planets = new List<Planet>
                        {
                            new Planet
                            {
                                Id = Guid.NewGuid(),
                                Name = "Planet 1",
                                CreatedDate = DateTime.Now,
                                LastUpdatedDate = DateTime.Now,
                                StarId = Guid.Parse("DF9AA280-C912-4E42-A5B5-4573CF97FDB0")
                            }
                         }
                    }
                };
            }

            public Star GetStar()
            {
                return new Star
                {
                    Id = Guid.Parse("E8D1FEAF-866D-47EC-A44E-A0E170DC29DC"),
                    CreatedDate = DateTime.Now,
                    LastUpdatedDate = DateTime.Now,
                    Name = "Star 3"
                };
            }

            public List<Star> GetStarsToAdd()
            {
                return new List<Star>
                {
                    new Star { Id = Guid.Parse("576B0C15-0010-4186-B45F-9E98B9E3F56B"), Name = "Star 3", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1},
                    new Star { Id = Guid.Parse("9720021D-A5F7-4B52-A0C6-C89CC73830A7"), Name = "Star 4", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2},
                };
            }

            public List<Star> GetStarsToAddWithDefaultGuids()
            {
                return new List<Star>
                {
                    new Star { },
                    new Star { },
                };
            }
        }

        public class PlanetData
        {
            public List<Planet> GetPlanets()
            {
                return new List<Planet>
                {
                    new Planet
                    {
                        Id =Guid.Parse("DF9AA280-C912-4E42-A5B5-4573CF97FDB0"),
                        Name = "Planet 1",
                        CreatedDate = DateTime.Now,
                        LastUpdatedDate = DateTime.Now,
                        Ordinal = 1,
                        StarId = Guid.NewGuid(),
                        Moons =
                        {
                            new Moon
                            {
                                Id = new Guid(),
                                Name =  "Moon 1",
                                Ordinal = 1,
                                CreatedDate = DateTime.Now,
                                LastUpdatedDate = DateTime.Now,
                                PlanetId = Guid.Parse("DF9AA280-C912-4E42-A5B5-4573CF97FDB0")
                            }
                        }
                    },
                    new Planet
                    {
                        Id =Guid.Parse("591D922F-D11C-469F-B61B-AF783D71E60A"),
                        Name = "Planet 2",
                        CreatedDate = DateTime.Now,
                        LastUpdatedDate = DateTime.Now,
                        Ordinal = 2,
                        StarId = Guid.NewGuid(),
                        Moons =
                        {
                            new Moon
                            {
                                Id = new Guid(),
                                Name =  "Moon 2",
                                Ordinal = 1,
                                CreatedDate = DateTime.Now,
                                LastUpdatedDate = DateTime.Now,
                                PlanetId = Guid.Parse("DF9AA280-C912-4E42-A5B5-4573CF97FDB0")
                            }
                        }
                    },
                };
            }

            public Planet GetPlanet()
            {
                return new Planet
                {
                    Id = Guid.Parse("E8D1FEAF-866D-47EC-A44E-A0E170DC29DC"),
                    CreatedDate = DateTime.Now,
                    LastUpdatedDate = DateTime.Now,
                    Name = "Planet 3",
                    StarId = GetPlanets().FirstOrDefault().StarId
                };
            }

            public List<Planet> GetPlanetsToAdd()
            {
                return new List<Planet>
                {
                    new Planet { Id = Guid.Parse("576B0C15-0010-4186-B45F-9E98B9E3F56B"), Name = "Planet 3", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1, StarId = GetPlanets().FirstOrDefault().StarId },
                    new Planet { Id = Guid.Parse("9720021D-A5F7-4B52-A0C6-C89CC73830A7"), Name = "Planet 4", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2, StarId = GetPlanets().Skip(1).Take(1).FirstOrDefault().StarId },
                };
            }
        }

        public class MoonData
        {
            public List<Moon> GetMoons()
            {
                return new List<Moon>
                {
                    new Moon
                    {
                        Id = Guid.Parse("DF9AA280-C912-4E42-A5B5-4573CF97FDB0"),
                        Name = "Moon 1",
                        CreatedDate = DateTime.Now,
                        LastUpdatedDate = DateTime.Now,
                        Ordinal = 1,
                        PlanetId = Guid.Parse("E8D1FEAF-866D-47EC-A44E-A0E170DC29DC"),
                        Planet = new Planet
                        {
                            Id = Guid.Parse("E8D1FEAF-866D-47EC-A44E-A0E170DC29DC"),
                            CreatedDate = DateTime.Now,
                            LastUpdatedDate = DateTime.Now,
                            Name = "Planet 1",
                            StarId = Guid.NewGuid()
                        }
                    },
                    new Moon
                    {
                        Id = Guid.Parse("591D922F-D11C-469F-B61B-AF783D71E60A"),
                        Name = "Moon 2",
                        CreatedDate = DateTime.Now,
                        LastUpdatedDate = DateTime.Now,
                        Ordinal = 2,
                        PlanetId = Guid.Parse("576B0C15-0010-4186-B45F-9E98B9E3F56B"),
                        Planet = new Planet
                        {
                            Id = Guid.Parse("576B0C15-0010-4186-B45F-9E98B9E3F56B"),
                            CreatedDate = DateTime.Now,
                            LastUpdatedDate = DateTime.Now,
                            Name = "Planet 2",
                            StarId = Guid.NewGuid()
                        }
                    },
                };
            }

            public Moon GetMoon()
            {
                return new Moon
                {
                    Id = Guid.Parse("E8D1FEAF-866D-47EC-A44E-A0E170DC29DC"),
                    CreatedDate = DateTime.Now,
                    LastUpdatedDate = DateTime.Now,
                    Name = "Moon 3",
                    PlanetId = GetMoons().FirstOrDefault().PlanetId
                };
            }

            public List<Moon> GetMoonsToAdd()
            {
                return new List<Moon>
                {
                    new Moon { Id = Guid.Parse("576B0C15-0010-4186-B45F-9E98B9E3F56B"), Name = "Moon 3", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1, PlanetId = GetMoons().FirstOrDefault().PlanetId },
                    new Moon { Id = Guid.Parse("9720021D-A5F7-4B52-A0C6-C89CC73830A7"), Name = "Moon 4", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2, PlanetId = GetMoons().Skip(1).Take(1).FirstOrDefault().PlanetId },
                };
            }
        }
    }
}
