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
                new Planet { Id = 1, Name = "Planet 1", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1 },
                new Planet { Id = 2, Name = "Planet 2", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2 },
            };
            repository = new Mock<IRepository<Planet>>();
            repository
                .Setup(p => p.GetAllAsync())
                .ReturnsAsync(planets);
            repository.Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Planet, bool>>>()))
                .ReturnsAsync(planets.FirstOrDefault(p => p.Id == 1));

            planet = new Planet
            {
                Id = 3,
                CreatedDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now,
                Name = "Planet 3",
                StarId = 1

            };

            service = new PlanetService(repository.Object);
        }

        [TestMethod]
        public async Task GetAllPlanets_ReturnsExpectedNumberOfPlanets()
        {
            Assert.AreEqual(2, (await service.GetAllPlanetsAsync()).Count());
        }

        [TestMethod]
        public async Task GetAllPlanets_ReturnsUnexpectedNumberOfPlanets()
        {
            Assert.AreNotEqual(3, (await service.GetAllPlanetsAsync()).Count());
        }

        [TestMethod]
        public async Task GetPlanet_ReturnsExpectedPlanet()
        {
            var planet = await service.GetPlanetAsync(1);
            Assert.IsInstanceOfType(planet, typeof(Planet));
            Assert.AreEqual("Planet 1", planet.Name);
        }

        [TestMethod]
        public async Task GetPlanet_ReturnsIncorrectPlanet()
        {
            var planet = await service.GetPlanetAsync(1);
            Assert.IsInstanceOfType(planet, typeof(Planet));
            Assert.AreNotEqual("Planet 2", planet.Name);
        }

        [TestMethod]
        public async Task FindPlanet_ReturnsExpectedPlanet()
        {
            throw new NotImplementedException();
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
        public async Task AddPlanetWithoutCreationDates_ReturnsFalse()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task AddPlanets_ReturnsTrue()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task AddNullPlanetList_ReturnsNullException()
        {
            //Assert.Throws<ArgumentException>(act);
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task AddPlanetsWithoutCreationDates_ReturnsFalse()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task SavePlanet_ReturnsTrue()
        {
            var planet = planets.FirstOrDefault(p => p.Id == 1);
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
            var planet = planets.FirstOrDefault(p => p.Id == 1);
            var result = await service.DeletePlanetAsync(planet.Id);
            repository.Verify(x => x.DeleteAsync(planet), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task DeletePlanetWithInvalidId_ReturnsException()
        {
            var planet = planets.FirstOrDefault(p => p.Id == 88);
            await service.DeletePlanetAsync(planet.Id);
            repository.Verify(x => x.DeleteAsync(planet), Times.Never());
        }
    }
}
