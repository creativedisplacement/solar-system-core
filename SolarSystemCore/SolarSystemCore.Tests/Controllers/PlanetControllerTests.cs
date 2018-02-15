using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarSystemCore.Core;
using SolarSystemCore.Data;
using SolarSystemCore.Models;
using SolarSystemCore.Repositories;
using SolarSystemCore.Services;
using SolarSystemCore.Tests.Helpers;
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

            var testDataHelper = new TestHelper.PlanetData();
            planets = testDataHelper.GetPlanets();
            planet = testDataHelper.GetPlanet();
            planetsToAdd = testDataHelper.GetPlanetsToAdd();

            foreach (var p in planets)
            {
                dbContext.Planets.Add(p);
            }

            dbContext.SaveChanges();

            var repository = new Repository<Planet>(dbContext);
            var service = new PlanetService(repository);
            var logger = new NullLogger<PlanetController>();
            IAppSettings appSettings = new AppSettings();
            controller = new PlanetController(service, appSettings, logger);
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
            var result = await controller.Get(planets.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Planet 1", result.Name);
        }

        [TestMethod]
        public async Task Controller_GetPlanet_ReturnsUnexpectedResult()
        {
            var result = await controller.Get(planets.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Planet 2", result.Name);
        }

        [TestMethod]
        public async Task Controller_GetAllPlanetsByStarId_ReturnsExpectedNumberOfPlanets()
        {
            var result = await controller.Get(planets.Skip(1).Take(1).FirstOrDefault().StarId, "star");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Planet 2");
        }

        [TestMethod]
        public async Task Controller_GetAllPlanetsByStarId_ReturnsUnexpectedPlanet()
        {
            var result = await controller.Get(planets.Skip(1).Take(1).FirstOrDefault().StarId, "star");
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
            var planetToSave = planets.SingleOrDefault(s => s.Id == planets.FirstOrDefault().Id);
            planetToSave.Name = "Planet 1 Saved";
            var result = await controller.Put(planetToSave.Id, planetToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, planetToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_SaveNullPlanet_ReturnsException()
        {
            var planetToSave = planets.SingleOrDefault(s => s.Id == new Guid());
            planetToSave.Name = "Planet 1 Saved";
            var result = await controller.Put(planetToSave.Id,planetToSave);
        }

        [TestMethod]
        public async Task Controller_DeletePlanetWithValidId_ReturnsExpectedResult()
        {
            var planetToDelete = planets.SingleOrDefault(s => s.Id == planets.FirstOrDefault().Id);
            await controller.Delete(planetToDelete.Id);
            Assert.IsNull(await controller.Get(planetToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_DeletePlanetWithInvalidId_ReturnsException()
        {
            var planetToDelete = planets.SingleOrDefault(s => s.Id == new Guid());
            await controller.Delete(planetToDelete.Id);
        }
    }
}
