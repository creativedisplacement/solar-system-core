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
    public class MoonControllerTests
    {
        private Moon _moon;
        private List<Moon> _moons;
        private List<Moon> _moonsToAdd;
        private MoonController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            var dbContext = new DBContext(options);

            var testDataHelper = new TestHelper.MoonData();
            _moons = testDataHelper.GetMoons();
            _moon = testDataHelper.GetMoon();
            _moonsToAdd = testDataHelper.GetMoonsToAdd();

            foreach (var m in _moons)
            {
                dbContext.Moons.Add(m);
            }

            dbContext.SaveChanges();

            var repository = new Repository<Moon>(dbContext);
            var moonRepository = new MoonRepository(repository);
            var service = new MoonService(moonRepository);
            var logger = new NullLogger<MoonController>();

            IAppSettings appSettings = new AppSettings();
            _controller = new MoonController(service, appSettings, logger);
        }

        [TestMethod]
        public async Task Controller_GetAllMoons_ReturnsExpectedNumberOfMoons()
        {
            var result = await _controller.Get();
            Assert.IsNotNull(result);
            Assert.AreEqual(_moons.Count(), result.Count());
        }

        [TestMethod]
        public async Task Controller_GetAllMoons_ReturnsUnexpectedNumberOfMoons()
        {
            var result = await _controller.Get();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, result.Count());
        }

        [TestMethod]
        public async Task Controller_GetMoon_ReturnsExpectedResult()
        {
            var moon = _moons.FirstOrDefault();
            var result = await _controller.Get(moon.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(moon.Name, result.Name);

            Assert.IsNotNull(result.Planet);
            Assert.AreEqual(result.Planet.Name, moon.Planet.Name);
        }

        [TestMethod]
        public async Task Controller_GetMoon_ReturnsUnexpectedResult()
        {
            var moon = _moons.FirstOrDefault();
            var result = await _controller.Get(moon.Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Moon 2", result.Name);
        }

        [TestMethod]
        public async Task Controller_GetAllMoonsByPlanetId_ReturnsExpectedNumberOfMoons()
        {
            var moon = _moons.Skip(1).Take(1).FirstOrDefault();

            var result = await _controller.Get(moon.PlanetId, "planet");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());

            var resultMoon = result.FirstOrDefault();
            Assert.IsNotNull(resultMoon);
            Assert.AreEqual(resultMoon.Name, moon.Name);
            Assert.AreEqual(resultMoon.Planet.Name, moon.Planet.Name);
        }

        [TestMethod]
        public async Task Controller_GetAllMoonsByPlanetId_ReturnsUnexpectedMoon()
        {
            var result = await _controller.Get(_moons.Skip(1).Take(1).FirstOrDefault().PlanetId, "planet");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            var moon = result.FirstOrDefault();
            Assert.AreNotEqual(moon.Name, "Moon 1");
        }

        [TestMethod]
        public async Task Controller_AddMoon_ReturnsExpectedResult()
        {
            var result = await _controller.Post(_moon);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, _moon.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_AddNullMoon_ReturnsException()
        {
            var result = await _controller.Post(new Moon());
        }

        [TestMethod]
        public async Task Controller_AddMoonList_ReturnsExpectedResult()
        {
            var result = await _controller.Post(_moonsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), _moonsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, _moonsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, _moonsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Controller_AddNullMoonList_ReturnsException()
        {
            var result = await _controller.Post(new List<Moon>());
        }

        [TestMethod]
        public async Task Controller_SaveMoon_ReturnsExpectedResult()
        {
            var moonToSave = _moons.SingleOrDefault(s => s.Id == _moons.FirstOrDefault().Id);
            moonToSave.Name = "Moon 1 Saved";
            var result = await _controller.Put(moonToSave.Id, moonToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, moonToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_SaveNullMoon_ReturnsException()
        {
            var moonToSave = _moons.SingleOrDefault(s => s.Id == new Guid());
            moonToSave.Name = "Moon 1 Saved";
            var result = await _controller.Put(moonToSave.Id, moonToSave);
        }

        [TestMethod]
        public async Task Controller_DeleteMoonWithValidId_ReturnsExpectedResult()
        {
            var moonToDelete = _moons.SingleOrDefault(s => s.Id == _moons.FirstOrDefault().Id);
            await _controller.Delete(moonToDelete.Id);
            Assert.IsNull(await _controller.Get(moonToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_DeleteMoonWithInvalidId_ReturnsException()
        {
            var moonToDelete = _moons.SingleOrDefault(s => s.Id == new Guid());
            await _controller.Delete(moonToDelete.Id);
        }
    }
}
