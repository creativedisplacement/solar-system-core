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
            service = new PlanetService(repository);
        }

        [TestMethod]
        public async Task Service_GetAllPlanets_ReturnsExpectedNumberOfPlanets()
        {
            var result = await service.GetAllPlanetsAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Service_GetAllPlanets_ReturnsUnexpectedNumberOfPlanets()
        {
            var result = await service.GetAllPlanetsAsync();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, result.Count());
        }

        [TestMethod]
        public async Task Service_GetAllPlanetsByStarId_ReturnsExpectedNumberOfPlanets()
        {
            var result = await service.GetAllPlanetsByStarIdAsync(planets.FirstOrDefault().StarId);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Planet 1");
        }

        [TestMethod]
        public async Task Service_GetAllPlanetsByStarId_ReturnsUnexpectedPlanet()
        {
            var result = await service.GetAllPlanetsByStarIdAsync(planets.Skip(1).Take(1).FirstOrDefault().StarId);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Planet 1");
        }

        [TestMethod]
        public async Task Service_GetPlanet_ReturnsExpectedResult()
        {
            var result = await service.GetPlanetAsync(planets.Skip(1).Take(1).FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Planet 2", result.Name);
        }

        [TestMethod]
        public async Task Service_GetPlanet_ReturnsUnexpectedResult()
        {
            var result = await service.GetPlanetAsync(planets.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Planet 2", result.Name);
        }

        [TestMethod]
        public async Task Service_FindPlanet_ReturnsExpectedPlanet()
        {
            var result = await service.FindPlanetsAsync(p => p.Id == planets.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Planet 1");
        }

        [TestMethod]
        public async Task Service_FindPlanet_ReturnsUnexpectedPlanet()
        {
            var result = await service.FindPlanetsAsync(p => p.Id == planets.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Planet 2");
        }

        [TestMethod]
        public async Task Service_AddPlanet_ReturnsExpectedResult()
        {
            var result = await service.AddPlanetAsync(planet);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, planet.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Service_AddNullPlanet_ReturnsException()
        {
            var result = await service.AddPlanetAsync(new Planet());
        }

        [TestMethod]
        public async Task Service_AddPlanetList_ReturnsExpectedResult()
        {
            var result = await service.AddPlanetsAsync(planetsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), planetsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, planetsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, planetsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Service_AddNullPlanetList_ReturnsException()
        {
            var planets = new List<Planet>();
            var result = await service.AddPlanetsAsync(planets);
        }

        [TestMethod]
        public async Task Service_SavePlanet_ReturnsExpectedResult()
        {
            var planetToSave = planets.SingleOrDefault(s => s.Id == planets.FirstOrDefault().Id);
            planetToSave.Name = "Planet 1 Saved";
            var result = await service.SavePlanetAsync(planetToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, planetToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Service_SaveNullPlanet_ReturnsException()
        {
            var planetToSave = planets.SingleOrDefault(s => s.Id == new Guid());
            planetToSave.Name = "Planet 1 Saved";
            var result = await service.SavePlanetAsync(planetToSave);
        }

        [TestMethod]
        public async Task Service_DeletePlanetWithValidId_ReturnsExpectedResult()
        {
            var planetToDelete = planets.SingleOrDefault(s => s.Id == planets.FirstOrDefault().Id);
            await service.DeletePlanetAsync(planetToDelete.Id);
            Assert.IsNull(await service.GetPlanetAsync(planetToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Service_DeletePlanetWithInvalidId_ReturnsException()
        {
            var planetToDelete = planets.SingleOrDefault(s => s.Id == new Guid());
            await service.DeletePlanetAsync(planetToDelete.Id);
        }
    }
}
