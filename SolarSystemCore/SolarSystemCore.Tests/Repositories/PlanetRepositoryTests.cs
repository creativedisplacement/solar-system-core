using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarSystemCore.Core.Entities;
using SolarSystemCore.Core.Interfaces;
using SolarSystemCore.Infrastructure.Data;
using SolarSystemCore.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarSystemCore.Tests.Repositories
{
    [TestClass]
    public class PlanetRepositoryTests
    {
        private IPlanetRepository _repository;
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
            _repository = new PlanetRepository(repository);
        }

        [TestMethod]
        public async Task Repository_GetAllPlanets_ReturnsExpectedNumberOfPlanets()
        {
            var result = await _repository.GetAllPlanets();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Repository_GetAllPlanets_ReturnsUnexpectedNumberOfPlanets()
        {
            var result = await _repository.GetAllPlanets();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, result.Count());
        }

        [TestMethod]
        public async Task Repository_GetAllPlanetsByStarId_ReturnsExpectedNumberOfPlanets()
        {
            var result = await _repository.GetPlanetsByStarId(_planets.FirstOrDefault().StarId);
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
        public async Task Repository_GetAllPlanetsByStarId_ReturnsUnexpectedPlanet()
        {
            var result = await _repository.GetPlanetsByStarId(_planets.Skip(1).Take(1).FirstOrDefault().StarId);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Planet 1");
        }

        [TestMethod]
        public async Task Repository_GetPlanet_ReturnsExpectedResult()
        {
            var result = await _repository.GetPlanet(_planets.Skip(1).Take(1).FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Planet 2", result.Name);

            Assert.AreEqual(result.Moons.Count(), _planets.Skip(1).Take(1).FirstOrDefault().Moons.Count());
            Assert.AreEqual(result.Moons.FirstOrDefault().Name, _planets.Skip(1).Take(1).FirstOrDefault().Moons.FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task Repository_GetPlanet_ReturnsUnexpectedResult()
        {
            var result = await _repository.GetPlanet(_planets.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Planet 2", result.Name);
        }

        [TestMethod]
        public async Task Repository_FindPlanet_ReturnsExpectedPlanet()
        {
            var result = await _repository.FindPlanets(p => p.Id == _planets.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Planet 1");
        }

        [TestMethod]
        public async Task Repository_FindPlanet_ReturnsUnexpectedPlanet()
        {
            var result = await _repository.FindPlanets(p => p.Id == _planets.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Planet 2");
        }

        [TestMethod]
        public async Task Repository_AddPlanet_ReturnsExpectedResult()
        {
            var result = await _repository.AddPlanet(_planet);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, _planet.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Repository_AddNullPlanet_ReturnsException()
        {
            var result = await _repository.AddPlanet(new Planet());
        }

        [TestMethod]
        public async Task Repository_AddPlanetList_ReturnsExpectedResult()
        {
            var result = await _repository.AddPlanets(_planetsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), _planetsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, _planetsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, _planetsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task Repository_AddNullPlanetList_ReturnsException()
        {
            var planets = new List<Planet>();
            var result = await _repository.AddPlanets(planets);

            Assert.AreEqual(planets.Any(), result.Any());
        }

        [TestMethod]
        public async Task Repository_SavePlanet_ReturnsExpectedResult()
        {
            var planetToSave = _planets.SingleOrDefault(s => s.Id == _planets.FirstOrDefault().Id);
            planetToSave.Name = "Planet 1 Saved";
            var result = await _repository.SavePlanet(planetToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, planetToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Repository_SaveNullPlanet_ReturnsException()
        {
            var planetToSave = _planets.SingleOrDefault(s => s.Id == new Guid());
            planetToSave.Name = "Planet 1 Saved";
            var result = await _repository.SavePlanet(planetToSave);
        }

        [TestMethod]
        public async Task Repository_DeletePlanetWithValidId_ReturnsExpectedResult()
        {
            var planetToDelete = _planets.SingleOrDefault(s => s.Id == _planets.FirstOrDefault().Id);
            await _repository.DeletePlanet(planetToDelete);
            Assert.IsNull(await _repository.GetPlanet(planetToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Repository_DeletePlanetWithInvalidId_ReturnsException()
        {
            var planetToDelete = _planets.SingleOrDefault(s => s.Id == new Guid());
            await _repository.DeletePlanet(planetToDelete);
        }
    }
}
