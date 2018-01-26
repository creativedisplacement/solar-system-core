using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarSystemCore.Data;
using SolarSystemCore.Models;
using SolarSystemCore.Repositories;
using SolarSystemCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarSystemCore.Tests.Services
{
    [TestClass]
    public class PlanetServiceTests
    {
        public DBContext dbContext { get; set; }
        public IRepository<Planet> repository { get; set; }
        public IPlanetService service { get; set; }
        public Planet planet { get; set; }
        public IList<Planet> planets { get; set; }
        public IList<Planet> planetsToAdd { get; set; }

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            dbContext = new DBContext(options);

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

            planetsToAdd = new List<Planet>
            {
                new Planet { Id = 3, Name = "Planet 3", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1, StarId = 1 },
                new Planet { Id = 4, Name = "Planet 4", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2, StarId = 2 },
            };


            foreach (var p in planets)
            {
                dbContext.Planets.Add(p);
            }

            dbContext.SaveChanges();

            repository = new Repository<Planet>(dbContext);
            service = new PlanetService(repository);
        }

        [TestMethod]
        public async Task GetAllPlanets_ReturnsExpectedNumberOfPlanets()
        {
            var result = await service.GetAllPlanetsAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetAllPlanets_ReturnsUnexpectedNumberOfPlanets()
        {
            var result = await service.GetAllPlanetsAsync();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, (await service.GetAllPlanetsAsync()).Count());
        }

        [TestMethod]
        public async Task GetAllPlanetsByStarId_ReturnsExpectedNumberOfPlanets()
        {
            var result = await service.GetAllPlanetsByStarIdAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Planet 1");
        }

        [TestMethod]
        public async Task GetAllPlanetsByStarId_ReturnsUnexpectedPlanet()
        {
            var result = await service.GetAllPlanetsByStarIdAsync(2);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Planet 1");
        }

        [TestMethod]
        public async Task GetPlanet_ReturnsExpectedResult()
        {
            var result = await service.GetPlanetAsync(2);
            Assert.IsNotNull(result);
            Assert.AreEqual("Planet 2", result.Name);
        }

        [TestMethod]
        public async Task GetPlanet_ReturnsUnexpectedResult()
        {
            var result = await service.GetPlanetAsync(1);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Planet 2", result.Name);
        }

        [TestMethod]
        public async Task FindPlanet_ReturnsExpectedPlanet()
        {
            var result = await service.FindPlanetsAsync(p => p.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Planet 1");
        }

        [TestMethod]
        public async Task FindPlanet_ReturnsUnexpectedPlanet()
        {
            var result = await service.FindPlanetsAsync(p => p.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Planet 2");
        }

        [TestMethod]
        public async Task AddPlanet_ReturnsExpectedResult()
        {
            var result = await service.AddPlanetAsync(planet);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task AddNullPlanet_ReturnsException()
        {
            var result = await service.AddPlanetAsync(new Planet());
        }

        [TestMethod]
        public async Task AddPlanetList_ReturnsExpectedResult()
        {
            var result = await service.AddPlanetsAsync(planetsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddNullPlanetList_ReturnsException()
        {
            var planets = new List<Planet>();
            var result = await service.AddPlanetsAsync(planets);
        }

        [TestMethod]
        public async Task SavePlanet_ReturnsExpectedResult()
        {
            var planetToSave = planets.SingleOrDefault(s => s.Id == 1);
            planetToSave.Name = "Planet 1 Saved";
            var result = await service.SavePlanetAsync(planetToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task SaveNullPlanet_ReturnsException()
        {
            var planetToSave = planets.SingleOrDefault(s => s.Id == 88);
            planetToSave.Name = "Planet 1 Saved";
            var result = await service.SavePlanetAsync(planetToSave);
        }

        [TestMethod]
        public async Task DeletePlanetWithValidId_ReturnsExpectedResult()
        {
            var planetToDelete = planets.SingleOrDefault(s => s.Id == 1);
            var result = await service.DeletePlanetAsync(planetToDelete.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task DeletePlanetWithInvalidId_ReturnsException()
        {
            var planetToDelete = planets.SingleOrDefault(s => s.Id == 88);
            var result = await service.DeletePlanetAsync(planetToDelete.Id);
        }
    }
}
