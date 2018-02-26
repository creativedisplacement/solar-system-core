using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarSystemCore.Data;
using SolarSystemCore.Models;
using SolarSystemCore.Repositories;
using SolarSystemCore.Services;
using SolarSystemCore.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarSystemCore.Tests.Services
{
    [TestClass]
    public class PlanetServiceTests
    {
        private IPlanetService _service;
        private Planet _planet;
        private List<Planet> _planets;
        private List<Planet> _planetsToAdd;

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
            _service = new PlanetService(planetRepository);
        }

        [TestMethod]
        public async Task Service_GetAllPlanets_ReturnsExpectedNumberOfPlanets()
        {
            var result = await _service.GetAllPlanets();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Service_GetAllPlanets_ReturnsUnexpectedNumberOfPlanets()
        {
            var result = await _service.GetAllPlanets();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, result.Count());
        }

        [TestMethod]
        public async Task Service_GetAllPlanetsByStarId_ReturnsExpectedNumberOfPlanets()
        {
            var result = await _service.GetAllPlanetsByStarId(_planets.FirstOrDefault().StarId);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Planet 1");

            var planet = result.FirstOrDefault();
            Assert.AreEqual(planet.Name, _planets.FirstOrDefault().Name);
            Assert.IsNotNull(planet.Moons);
            Assert.AreEqual(planet.Moons.Count(), _planets.FirstOrDefault().Moons.Count());
            Assert.AreEqual(planet.Moons.FirstOrDefault().Name, _planets.FirstOrDefault().Moons.FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task Service_GetAllPlanetsByStarId_ReturnsUnexpectedPlanet()
        {
            var result = await _service.GetAllPlanetsByStarId(_planets.Skip(1).Take(1).FirstOrDefault().StarId);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Planet 1");
        }

        [TestMethod]
        public async Task Service_GetPlanet_ReturnsExpectedResult()
        {
            var result = await _service.GetPlanet(_planets.Skip(1).Take(1).FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Planet 2", result.Name);

            Assert.AreEqual(result.Moons.Count(), _planets.Skip(1).Take(1).FirstOrDefault().Moons.Count());
            Assert.AreEqual(result.Moons.FirstOrDefault().Name, _planets.Skip(1).Take(1).FirstOrDefault().Moons.FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task Service_GetPlanet_ReturnsUnexpectedResult()
        {
            var result = await _service.GetPlanet(_planets.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Planet 2", result.Name);
        }

        [TestMethod]
        public async Task Service_FindPlanet_ReturnsExpectedPlanet()
        {
            var result = await _service.FindPlanets(p => p.Id == _planets.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Planet 1");
        }

        [TestMethod]
        public async Task Service_FindPlanet_ReturnsUnexpectedPlanet()
        {
            var result = await _service.FindPlanets(p => p.Id == _planets.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Planet 2");
        }

        [TestMethod]
        public async Task Service_AddPlanet_ReturnsExpectedResult()
        {
            var result = await _service.AddPlanet(_planet);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, _planet.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Service_AddNullPlanet_ReturnsException()
        {
            var result = await _service.AddPlanet(new Planet());
        }

        [TestMethod]
        public async Task Service_AddPlanetList_ReturnsExpectedResult()
        {
            var result = await _service.AddPlanets(_planetsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), _planetsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, _planetsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, _planetsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Service_AddNullPlanetList_ReturnsException()
        {
            var planets = new List<Planet>();
            var result = await _service.AddPlanets(planets);
        }

        [TestMethod]
        public async Task Service_SavePlanet_ReturnsExpectedResult()
        {
            var planetToSave = _planets.SingleOrDefault(s => s.Id == _planets.FirstOrDefault().Id);
            planetToSave.Name = "Planet 1 Saved";
            var result = await _service.SavePlanet(planetToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, planetToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Service_SaveNullPlanet_ReturnsException()
        {
            var planetToSave = _planets.SingleOrDefault(s => s.Id == new Guid());
            planetToSave.Name = "Planet 1 Saved";
            var result = await _service.SavePlanet(planetToSave);
        }

        [TestMethod]
        public async Task Service_DeletePlanetWithValidId_ReturnsExpectedResult()
        {
            var planetToDelete = _planets.SingleOrDefault(s => s.Id == _planets.FirstOrDefault().Id);
            await _service.DeletePlanet(planetToDelete.Id);
            Assert.IsNull(await _service.GetPlanet(planetToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Service_DeletePlanetWithInvalidId_ReturnsException()
        {
            var planetToDelete = _planets.SingleOrDefault(s => s.Id == new Guid());
            await _service.DeletePlanet(planetToDelete.Id);
        }
    }
}
