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
        public IMoonService service { get; set; }
        public Moon moon { get; set; }
        public IList<Moon> moons { get; set; }
        public IList<Moon> moonsToAdd { get; set; }

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            var dbContext = new DBContext(options);

            var testDataHelper = new TestHelper.MoonData();
            moons = testDataHelper.GetMoons();
            moon = testDataHelper.GetMoon();
            moonsToAdd = testDataHelper.GetMoonsToAdd();

            foreach (var p in moons)
            {
                dbContext.Moons.Add(p);
            }

            dbContext.SaveChanges();

            var repository = new Repository<Moon>(dbContext);
            service = new MoonService(repository);
        }

        [TestMethod]
        public async Task Service_GetAllMoons_ReturnsExpectedNumberOfMoons()
        {
            var result = await service.GetAllMoonsAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Service_GetAllMoons_ReturnsUnexpectedNumberOfMoons()
        {
            var result = await service.GetAllMoonsAsync();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, (await service.GetAllMoonsAsync()).Count());
        }

        [TestMethod]
        public async Task Service_GetAllMoonsByStarId_ReturnsExpectedNumberOfMoons()
        {
            var result = await service.GetAllMoonsByPlanetIdAsync(moons.FirstOrDefault().PlanetId);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Moon 1");
        }

        [TestMethod]
        public async Task Service_GetAllMoonsByStarId_ReturnsUnexpectedMoon()
        {
            var result = await service.GetAllMoonsByPlanetIdAsync(moons.Skip(1).Take(1).FirstOrDefault().PlanetId);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Moon 1");
        }

        [TestMethod]
        public async Task Service_GetMoon_ReturnsExpectedResult()
        {
            var result = await service.GetMoonAsync(moons.Skip(1).Take(1).FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Moon 2", result.Name);
        }

        [TestMethod]
        public async Task Service_GetMoon_ReturnsUnexpectedResult()
        {
            var result = await service.GetMoonAsync(moons.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Moon 2", result.Name);
        }

        [TestMethod]
        public async Task Service_FindMoon_ReturnsExpectedMoon()
        {
            var result = await service.FindMoonsAsync(p => p.Id == moons.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Moon 1");
        }

        [TestMethod]
        public async Task Service_FindMoon_ReturnsUnexpectedMoon()
        {
            var result = await service.FindMoonsAsync(p => p.Id == moons.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Moon 2");
        }

        [TestMethod]
        public async Task Service_AddMoon_ReturnsExpectedResult()
        {
            var result = await service.AddMoonAsync(moon);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, moon.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Service_AddNullMoon_ReturnsException()
        {
            var result = await service.AddMoonAsync(new Moon());
        }

        [TestMethod]
        public async Task Service_AddMoonList_ReturnsExpectedResult()
        {
            var result = await service.AddMoonsAsync(moonsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), moonsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, moonsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, moonsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Service_AddNullMoonList_ReturnsException()
        {
            var moons = new List<Moon>();
            var result = await service.AddMoonsAsync(moons);
        }

        [TestMethod]
        public async Task Service_SaveMoon_ReturnsExpectedResult()
        {
            var moonToSave = moons.SingleOrDefault(s => s.Id == moons.FirstOrDefault().Id);
            moonToSave.Name = "Moon 1 Saved";
            var result = await service.SaveMoonAsync(moonToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, moonToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Service_SaveNullMoon_ReturnsException()
        {
            var moonToSave = moons.SingleOrDefault(s => s.Id == new Guid());
            moonToSave.Name = "Moon 1 Saved";
            var result = await service.SaveMoonAsync(moonToSave);
        }

        [TestMethod]
        public async Task Service_DeleteMoonWithValidId_ReturnsExpectedResult()
        {
            var moonToDelete = moons.SingleOrDefault(s => s.Id == moons.FirstOrDefault().Id);
            await service.DeleteMoonAsync(moonToDelete.Id);
            Assert.IsNull(await service.GetMoonAsync(moonToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Service_DeleteMoonWithInvalidId_ReturnsException()
        {
            var moonToDelete = moons.SingleOrDefault(s => s.Id == new Guid());
            await service.DeleteMoonAsync(moonToDelete.Id);
        }
    }
}
