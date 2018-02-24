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
    public class MoonServiceTests
    {
        private IMoonService _service;
        private Moon _moon;
        private List<Moon> _moons;
        private List<Moon> _moonsToAdd;

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

            foreach (var p in _moons)
            {
                dbContext.Moons.Add(p);
            }

            dbContext.SaveChanges();

            var repository = new Repository<Moon>(dbContext);
            var moonRepository = new MoonRepository(repository);
            _service = new MoonService(moonRepository);
        }

        [TestMethod]
        public async Task Service_GetAllMoons_ReturnsExpectedNumberOfMoons()
        {
            var result = await _service.GetAllMoons();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Service_GetAllMoons_ReturnsUnexpectedNumberOfMoons()
        {
            var result = await _service.GetAllMoons();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, (await _service.GetAllMoons()).Count());
        }

        [TestMethod]
        public async Task Service_GetAllMoonsByPlanetId_ReturnsExpectedNumberOfMoons()
        {
            var result = await _service.GetAllMoonsByPlanetId(_moons.FirstOrDefault().PlanetId);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Moon 1");
        }

        [TestMethod]
        public async Task Service_GetAllMoonsByPlanetId_ReturnsUnexpectedMoon()
        {
            var result = await _service.GetAllMoonsByPlanetId(_moons.Skip(1).Take(1).FirstOrDefault().PlanetId);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Moon 1");
        }

        [TestMethod]
        public async Task Service_GetMoon_ReturnsExpectedResult()
        {
            var result = await _service.GetMoon(_moons.Skip(1).Take(1).FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Moon 2", result.Name);
        }

        [TestMethod]
        public async Task Service_GetMoon_ReturnsUnexpectedResult()
        {
            var result = await _service.GetMoon(_moons.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Moon 2", result.Name);
        }

        [TestMethod]
        public async Task Service_FindMoon_ReturnsExpectedMoon()
        {
            var result = await _service.FindMoons(p => p.Id == _moons.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Moon 1");
        }

        [TestMethod]
        public async Task Service_FindMoon_ReturnsUnexpectedMoon()
        {
            var result = await _service.FindMoons(p => p.Id == _moons.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Moon 2");
        }

        [TestMethod]
        public async Task Service_AddMoon_ReturnsExpectedResult()
        {
            var result = await _service.AddMoon(_moon);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, _moon.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Service_AddNullMoon_ReturnsException()
        {
            var result = await _service.AddMoon(new Moon());
        }

        [TestMethod]
        public async Task Service_AddMoonList_ReturnsExpectedResult()
        {
            var result = await _service.AddMoons(_moonsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), _moonsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, _moonsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, _moonsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Service_AddNullMoonList_ReturnsException()
        {
            var moons = new List<Moon>();
            var result = await _service.AddMoons(moons);
        }

        [TestMethod]
        public async Task Service_SaveMoon_ReturnsExpectedResult()
        {
            var moonToSave = _moons.SingleOrDefault(s => s.Id == _moons.FirstOrDefault().Id);
            moonToSave.Name = "Moon 1 Saved";
            var result = await _service.SaveMoon(moonToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, moonToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Service_SaveNullMoon_ReturnsException()
        {
            var moonToSave = _moons.SingleOrDefault(s => s.Id == new Guid());
            moonToSave.Name = "Moon 1 Saved";
            var result = await _service.SaveMoon(moonToSave);
        }

        [TestMethod]
        public async Task Service_DeleteMoonWithValidId_ReturnsExpectedResult()
        {
            var moonToDelete = _moons.SingleOrDefault(s => s.Id == _moons.FirstOrDefault().Id);
            await _service.DeleteMoon(moonToDelete.Id);
            Assert.IsNull(await _service.GetMoon(moonToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Service_DeleteMoonWithInvalidId_ReturnsException()
        {
            var moonToDelete = _moons.SingleOrDefault(s => s.Id == new Guid());
            await _service.DeleteMoon(moonToDelete.Id);
        }
    }
}
