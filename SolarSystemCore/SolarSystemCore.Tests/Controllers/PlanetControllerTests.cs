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
        private Planet _planet;
        private List<Planet> _planets;
        private List<Planet> _planetsToAdd;
        private PlanetController _controller; 

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            var dbContext = new DBContext(options);

            var testDataHelper = new TestHelper.PlanetData();
            _planets = testDataHelper.GetPlanets();
            _planet = testDataHelper.GetPlanet();
            _planetsToAdd = testDataHelper.GetPlanetsToAdd();

            foreach (var p in _planets)
            {
                dbContext.Planets.Add(p);
            }

            dbContext.SaveChanges();

            var repository = new Repository<Planet>(dbContext);
            var planetRepository = new PlanetRepository(repository);
            var service = new PlanetService(planetRepository);
            var logger = new NullLogger<PlanetController>();

            IAppSettings appSettings = new AppSettings();
            _controller = new PlanetController(service, appSettings, logger);
        }

        [TestMethod]
        public async Task Controller_GetAllPlanets_ReturnsExpectedNumberOfPlanets()
        {
            var result = await _controller.Get();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Controller_GetAllPlanets_ReturnsUnexpectedNumberOfPlanets()
        {
            var result = await _controller.Get();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, result.Count());
        }

        [TestMethod]
        public async Task Controller_GetPlanet_ReturnsExpectedResult()
        {
            var result = await _controller.Get(_planets.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Planet 1", result.Name);
        }

        [TestMethod]
        public async Task Controller_GetPlanet_ReturnsUnexpectedResult()
        {
            var result = await _controller.Get(_planets.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Planet 2", result.Name);
        }

        [TestMethod]
        public async Task Controller_GetAllPlanetsByStarId_ReturnsExpectedNumberOfPlanets()
        {
            var result = await _controller.Get(_planets.Skip(1).Take(1).FirstOrDefault().StarId, "star");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Planet 2");
        }

        [TestMethod]
        public async Task Controller_GetAllPlanetsByStarId_ReturnsUnexpectedPlanet()
        {
            var result = await _controller.Get(_planets.Skip(1).Take(1).FirstOrDefault().StarId, "star");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Planet 1");
        }

        [TestMethod]
        public async Task Controller_AddPlanet_ReturnsExpectedResult()
        {
            var result = await _controller.Post(_planet);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, _planet.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_AddNullPlanet_ReturnsException()
        {
            var result = await _controller.Post(new Planet());
        }

        [TestMethod]
        public async Task Controller_AddPlanetList_ReturnsExpectedResult()
        {
            var result = await _controller.Post(_planetsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), _planetsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, _planetsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, _planetsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Controller_AddNullPlanetList_ReturnsException()
        {
            var result = await _controller.Post(new List<Planet>());
        }

        [TestMethod]
        public async Task Controller_SavePlanet_ReturnsExpectedResult()
        {
            var planetToSave = _planets.SingleOrDefault(s => s.Id == _planets.FirstOrDefault().Id);
            planetToSave.Name = "Planet 1 Saved";
            var result = await _controller.Put(planetToSave.Id, planetToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, planetToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_SaveNullPlanet_ReturnsException()
        {
            var planetToSave = _planets.SingleOrDefault(s => s.Id == new Guid());
            planetToSave.Name = "Planet 1 Saved";
            var result = await _controller.Put(planetToSave.Id,planetToSave);
        }

        [TestMethod]
        public async Task Controller_DeletePlanetWithValidId_ReturnsExpectedResult()
        {
            var planetToDelete = _planets.SingleOrDefault(s => s.Id == _planets.FirstOrDefault().Id);
            await _controller.Delete(planetToDelete.Id);
            Assert.IsNull(await _controller.Get(planetToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_DeletePlanetWithInvalidId_ReturnsException()
        {
            var planetToDelete = _planets.SingleOrDefault(s => s.Id == new Guid());
            await _controller.Delete(planetToDelete.Id);
        }
    }
}
