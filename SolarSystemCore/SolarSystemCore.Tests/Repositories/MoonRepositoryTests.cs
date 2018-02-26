using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarSystemCore.Data;
using SolarSystemCore.Models;
using SolarSystemCore.Repositories;
using SolarSystemCore.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarSystemCore.Tests.Repositories
{
    [TestClass]
    public class MoonRepositoryTests
    {
        private IMoonRepository _repository;
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
            _repository = new MoonRepository(repository);
        }

        [TestMethod]
        public async Task Repository_GetAllMoons_ReturnsExpectedNumberOfMoons()
        {
            var result = await _repository.GetAllMoons();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Repository_GetAllMoons_ReturnsUnexpectedNumberOfMoons()
        {
            var result = await _repository.GetAllMoons();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, (await _repository.GetAllMoons()).Count());
        }

        [TestMethod]
        public async Task Repository_GetAllMoonsByPlanetId_ReturnsExpectedNumberOfMoons()
        {
            var result = await _repository.GetMoonsByPlanetId(_moons.FirstOrDefault().PlanetId);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Moon 1");
        }

        [TestMethod]
        public async Task Repository_GetAllMoonsByPlanetId_ReturnsUnexpectedMoon()
        {
            var moon = _moons.Skip(1).Take(1).FirstOrDefault();
            var result = await _repository.GetMoonsByPlanetId(_moons.Skip(1).Take(1).FirstOrDefault().PlanetId);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Moon 1");

            var resultMoon = result.FirstOrDefault();
            Assert.IsNotNull(resultMoon);
            Assert.AreEqual(resultMoon.Name, moon.Name);
            Assert.AreEqual(resultMoon.Planet.Name, moon.Planet.Name);
        }

        [TestMethod]
        public async Task Repository_GetMoon_ReturnsExpectedResult()
        {
            var moon = _moons.Skip(1).Take(1).FirstOrDefault();
            var result = await _repository.GetMoon(moon.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Moon 2", result.Name);

            Assert.IsNotNull(result.Planet);
            Assert.AreEqual(result.Planet.Name, moon.Planet.Name);
        }

        [TestMethod]
        public async Task Repository_GetMoon_ReturnsUnexpectedResult()
        {
            var result = await _repository.GetMoon(_moons.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Moon 2", result.Name);
        }

        [TestMethod]
        public async Task Repository_FindMoon_ReturnsExpectedMoon()
        {
            var result = await _repository.FindMoons(p => p.Id == _moons.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Moon 1");
        }

        [TestMethod]
        public async Task Repository_FindMoon_ReturnsUnexpectedMoon()
        {
            var result = await _repository.FindMoons(p => p.Id == _moons.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Moon 2");
        }

        [TestMethod]
        public async Task Repository_AddMoon_ReturnsExpectedResult()
        {
            var result = await _repository.AddMoon(_moon);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, _moon.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Repository_AddNullMoon_ReturnsException()
        {
            var result = await _repository.AddMoon(new Moon());
        }

        [TestMethod]
        public async Task Repository_AddMoonList_ReturnsExpectedResult()
        {
            var result = await _repository.AddMoons(_moonsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), _moonsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, _moonsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, _moonsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task Repository_AddNullMoonList_ReturnsException()
        {
            var moons = new List<Moon>();
            var result = await _repository.AddMoons(moons);

            Assert.AreEqual(moons.Any(), result.Any());
        }

        [TestMethod]
        public async Task Repository_SaveMoon_ReturnsExpectedResult()
        {
            var moonToSave = _moons.SingleOrDefault(s => s.Id == _moons.FirstOrDefault().Id);
            moonToSave.Name = "Moon 1 Saved";
            var result = await _repository.SaveMoon(moonToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, moonToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Repository_SaveNullMoon_ReturnsException()
        {
            var moonToSave = _moons.SingleOrDefault(s => s.Id == new Guid());
            moonToSave.Name = "Moon 1 Saved";
            var result = await _repository.SaveMoon(moonToSave);
        }

        [TestMethod]
        public async Task Repository_DeleteMoonWithValidId_ReturnsExpectedResult()
        {
            var moonToDelete = _moons.SingleOrDefault(s => s.Id == _moons.FirstOrDefault().Id);
            await _repository.DeleteMoon(moonToDelete);
            Assert.IsNull(await _repository.GetMoon(moonToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Repository_DeleteMoonWithInvalidId_ReturnsException()
        {
            var moonToDelete = _moons.SingleOrDefault(s => s.Id == new Guid());
            await _repository.DeleteMoon(moonToDelete);
        }
    }
}
