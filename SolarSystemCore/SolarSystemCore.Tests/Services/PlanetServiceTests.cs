using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SolarSystemCore.Models;
using SolarSystemCore.Repositories;
using SolarSystemCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Tests.Services
{
    [TestClass]
    public class PlanetServiceTests
    {
        public Mock<IRepository<Planet>> repository { get; set; }
        public List<Planet> planets { get; set; }
        public IPlanetService service { get; set; }
        public Planet planet { get; set; }

        [TestInitialize]
        public void Setup()
        {
            planets = new List<Planet>
            {
                new Planet { Id = 1, Name = "Planet 1", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1, StarId = 1 },
                new Planet { Id = 2, Name = "Planet 2", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2, StarId = 2 },
            };
           
            planet = new Planet
            {
                Id = 3,
                CreatedDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now,
                Name = "Planet 3",
                StarId = 1

            };

            repository = new Mock<IRepository<Planet>>();
            service = new PlanetService(repository.Object);
        }

        [TestMethod]
        public async Task GetAllPlanets_ReturnsExpectedNumberOfPlanets()
        {
            repository
                .Setup(p => p.GetAllAsync())
                .ReturnsAsync(planets);

            var result = await service.GetAllPlanetsAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetAllPlanets_ReturnsUnexpectedNumberOfPlanets()
        {
            repository
                .Setup(p => p.GetAllAsync())
                .ReturnsAsync(planets);

            var result = await service.GetAllPlanetsAsync();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, (await service.GetAllPlanetsAsync()).Count());
        }

        [TestMethod]
        public async Task GetAllPlanetsByStarId_ReturnsExpectedNumberOfPlanets()
        {
            repository
               .Setup(p => p.FindAsync(It.IsAny<Expression<Func<Planet, bool>>>()))
               .ReturnsAsync(planets.Where(p => p.StarId == 1));

            var result = await service.GetAllPlanetsByStarIdAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Planet 1");
        }

        [TestMethod]
        public async Task GetAllPlanetsByStarId_ReturnsUnexpectedPlanet()
        {
            repository
                .Setup(p => p.FindAsync(It.IsAny<Expression<Func<Planet, bool>>>()))
                .ReturnsAsync(planets.Where(p => p.StarId == 2));

            var result = await service.GetAllPlanetsByStarIdAsync(2);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Planet 1");
        }

        [TestMethod]
        public async Task GetPlanet_ReturnsExpectedPlanet()
        {
            repository
                .Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Planet, bool>>>()))
                .ReturnsAsync(planet);

            var result = await service.GetPlanetAsync(3);
            Assert.IsInstanceOfType(planet, typeof(Planet));
            Assert.AreEqual("Planet 3", planet.Name);
        }

        [TestMethod]
        public async Task GetPlanet_ReturnsIncorrectPlanet()
        {
            repository
              .Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Planet, bool>>>()))
              .ReturnsAsync(planet);

            var result = await service.GetPlanetAsync(1);
            Assert.IsInstanceOfType(planet, typeof(Planet));
            Assert.AreNotEqual("Planet 1", planet.Name);
        }

        [TestMethod]
        public async Task FindPlanet_ReturnsExpectedPlanet()
        {
            repository
              .Setup(p => p.FindAsync(It.IsAny<Expression<Func<Planet, bool>>>()))
              .ReturnsAsync(planets.Where(p => p.Id == 1));

            var result = await service.FindPlanetsAsync(p => p.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Planet 1");
        }

        [TestMethod]
        public async Task FindPlanet_ReturnsUnexpectedPlanet()
        {
            repository
               .Setup(p => p.FindAsync(It.IsAny<Expression<Func<Planet, bool>>>()))
               .ReturnsAsync(planets.Where(p => p.Id == 1));

            var result = await service.FindPlanetsAsync(p => p.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Planet 2");
        }

        [TestMethod]
        public async Task AddPlanet_ReturnsTrue()
        {
            var result = await service.AddPlanetAsync(planet);
            repository.Verify(x => x.AddAsync(planet), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task AddNullPlanet_ReturnsException()
        {
            var result = await service.AddPlanetAsync(new Planet());
            repository.Verify(x => x.AddAsync(planet), Times.Never());
        }

        [TestMethod]
        public async Task AddPlanetList_ReturnsTrue()
        {
            var planets = new List<Planet> {
                new Planet { Id = 3, Name = "Planet 3", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now},
                new Planet { Id = 4, Name = "Planet 4", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now},
            };
            var result = await service.AddPlanetsAsync(planets);
            repository.Verify(x => x.AddRangeAsync(planets), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddNullPlanetList_ReturnsNullException()
        {
            var planets = new List<Planet>();
            var result = await service.AddPlanetsAsync(planets);
            repository.Verify(x => x.AddRangeAsync(planets), Times.Never());
        }

        [TestMethod]
        public async Task SavePlanet_ReturnsTrue()
        {
            repository
             .Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Planet, bool>>>()))
             .ReturnsAsync(planet);

            var result = await service.SavePlanetAsync(planet);
            repository.Verify(x => x.SaveAsync(planet), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task SaveNullPlanet_ReturnsException()
        {
            var result = await service.SavePlanetAsync(null);
            repository.Verify(x => x.SaveAsync(planet), Times.Never());
        }

        [TestMethod]
        public async Task DeletePlanetWithValidId()
        {
            repository
              .Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Planet, bool>>>()))
              .ReturnsAsync(planet);

            var result = await service.DeletePlanetAsync(3);
            repository.Verify(x => x.DeleteAsync(planet), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task DeletePlanetWithInvalidId_ReturnsException()
        {
            repository
              .Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Planet, bool>>>()))
              .ReturnsAsync(new Planet());

            await service.DeletePlanetAsync(88);
            repository.Verify(x => x.DeleteAsync(planet), Times.Never());
        }
    }
}
