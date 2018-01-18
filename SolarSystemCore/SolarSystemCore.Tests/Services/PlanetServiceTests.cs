using Moq;
using SolarSystemCore.Models;
using SolarSystemCore.Repositories;
using SolarSystemCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace SolarSystemCore.Tests.Services
{
    public class PlanetServiceTests
    {
        public Mock<IRepository<Planet>> repository { get; set; }
        public List<Planet> planets { get; set; }
        public IPlanetService service { get; set; }

        public PlanetServiceTests()
        {
            planets = new List<Planet>
            {
                new Planet { Id = 1, Name = "Planet 1", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1 },
                new Planet { Id = 2, Name = "Planet 2", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2 },
            };
            repository = new Mock<IRepository<Planet>>();
            repository
                .Setup(p => p.GetAllAsync())
                .ReturnsAsync(planets);
            repository.Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Planet, bool>>>()))
                .ReturnsAsync(planets.FirstOrDefault(p => p.Id == 1));

            service = new PlanetService(repository.Object);
        }

        [Fact]
        public async void GetAllPlanets_ReturnsExpectedPlanets()
        {
            Assert.Equal(2, (await service.GetAllPlanetsAsync()).Count());
        }

        [Fact]
        public async void GetAllPlanets_ReturnsNull()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async void GetPlanet_ReturnsExpectedPlanet()
        {
            var planet = await service.GetPlanetAsync(1);
            Assert.IsType<Planet>(planet);
            Assert.Equal("Planet 1", planet.Name);
        }

        [Fact]
        public async void GetPlanet_ReturnsIncorrectPlanet()
        {
            var planet = await service.GetPlanetAsync(1);
            Assert.IsType<Planet>(planet);
            Assert.NotEqual("Planet 2", planet.Name);
        }

        [Fact]
        public async void FindPlanet_ReturnsExpectedPlanet()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async void AddPlanet_ReturnsTrue()
        {
            throw new NotImplementedException();
        }
    }
}
