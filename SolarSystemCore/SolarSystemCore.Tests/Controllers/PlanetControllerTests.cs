using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarSystemCore.Core;
using SolarSystemCore.Data;
using SolarSystemCore.Models;
using SolarSystemCore.Repositories;
using SolarSystemCore.Services;
using SolarSystemCore.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarSystemCore.Tests.Controllers
{
    [TestClass]
    public class PlanetControllerTests
    {
        public Planet planet { get; set; }
        public IList<Planet> planets { get; set; }
        public IList<Planet> planetsToAdd { get; set; }
        public PlanetController controller { get; set; } 

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            var dbContext = new DBContext(options);

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

            var repository = new Repository<Planet>(dbContext);
            var service = new PlanetService(repository);
            IAppSettings appSettings = new AppSettings();
            controller = new PlanetController(service, appSettings);
        }

        [TestMethod]
        public async Task Controller_GetAllPlanets_ReturnsExpectedNumberOfPlanets()
        {
            var result = await controller.Get();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Controller_GetAllPlanets_ReturnsUnexpectedNumberOfPlanets()
        {
            var result = await controller.Get();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, result.Count());
        }

        [TestMethod]
        public async Task Controller_GetPlanet_ReturnsExpectedResult()
        {
            var result = await controller.Get(1);
            Assert.IsNotNull(result);
            Assert.AreEqual("Planet 1", result.Name);
        }

        [TestMethod]
        public async Task Controller_GetPlanet_ReturnsUnexpectedResult()
        {
            var result = await controller.Get(1);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Planet 2", result.Name);
        }

        [TestMethod]
        public async Task Controller_GetAllPlanetsByStarId_ReturnsExpectedNumberOfPlanets()
        {
            var result = await controller.Get(2, "star");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Planet 2");
        }

        [TestMethod]
        public async Task Controller_GetAllPlanetsByStarId_ReturnsUnexpectedPlanet()
        {
            var result = await controller.Get(2, "star");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Planet 1");
        }

        [TestMethod]
        public async Task Controller_AddPlanet_ReturnsExpectedResult()
        {
            var result = await controller.Post(planet);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, planet.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_AddNullPlanet_ReturnsException()
        {
            var result = await controller.Post(new Planet());
        }

        [TestMethod]
        public async Task Controller_AddPlanetList_ReturnsExpectedResult()
        {
            var result = await controller.Post(planetsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), planetsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, planetsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, planetsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Controller_AddNullPlanetList_ReturnsException()
        {
            var result = await controller.Post(new List<Planet>());
        }

        [TestMethod]
        public async Task Controller_SavePlanet_ReturnsExpectedResult()
        {
            var planetToSave = planets.SingleOrDefault(s => s.Id == 1);
            planetToSave.Name = "Planet 1 Saved";
            var result = await controller.Put(planetToSave.Id, planetToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, planetToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_SaveNullPlanet_ReturnsException()
        {
            var planetToSave = planets.SingleOrDefault(s => s.Id == 88);
            planetToSave.Name = "Planet 1 Saved";
            var result = await controller.Put(planetToSave.Id,planetToSave);
        }

        [TestMethod]
        public async Task Controller_DeletePlanetWithValidId_ReturnsExpectedResult()
        {
            var planetToDelete = planets.SingleOrDefault(s => s.Id == 1);
            await controller.Delete(planetToDelete.Id);
            Assert.IsNull(await controller.Get(planetToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_DeletePlanetWithInvalidId_ReturnsException()
        {
            var planetToDelete = planets.SingleOrDefault(s => s.Id == 88);
            await controller.Delete(planetToDelete.Id);
        }
    }
}
