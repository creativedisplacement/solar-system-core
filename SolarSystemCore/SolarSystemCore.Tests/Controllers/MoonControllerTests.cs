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
    public class MoonControllerTests
    {
        public Moon moon { get; set; }
        public IList<Moon> moons { get; set; }
        public IList<Moon> moonsToAdd { get; set; }
        public MoonController controller { get; set; }

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            var dbContext = new DBContext(options);

            moons = new List<Moon>
            {
                new Moon { Id = 1, Name = "Moon 1", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1, PlanetId = 1 },
                new Moon { Id = 2, Name = "Moon 2", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2, PlanetId = 2 },
            };

            moon = new Moon
            {
                Id = 3,
                CreatedDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now,
                Name = "Moon 3",
                PlanetId = 1
            };

            moonsToAdd = new List<Moon>
            {
                new Moon { Id = 3, Name = "Moon 3", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1, PlanetId = 1 },
                new Moon { Id = 4, Name = "Moon 4", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2, PlanetId = 2 },
            };

            foreach (var m in moons)
            {
                dbContext.Moons.Add(m);
            }

            dbContext.SaveChanges();

            var repository = new Repository<Moon>(dbContext);
            var service = new MoonService(repository);
            IAppSettings appSettings = new AppSettings();
            controller = new MoonController(service, appSettings);
        }

        [TestMethod]
        public async Task Controller_GetAllMoons_ReturnsExpectedNumberOfMoons()
        {
            var result = await controller.Get();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Controller_GetAllMoons_ReturnsUnexpectedNumberOfMoons()
        {
            var result = await controller.Get();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, result.Count());
        }

        [TestMethod]
        public async Task Controller_GetMoon_ReturnsExpectedResult()
        {
            var result = await controller.Get(1);
            Assert.IsNotNull(result);
            Assert.AreEqual("Moon 1", result.Name);
        }

        [TestMethod]
        public async Task Controller_GetMoon_ReturnsUnexpectedResult()
        {
            var result = await controller.Get(1);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Moon 2", result.Name);
        }

        [TestMethod]
        public async Task Controller_GetAllMoonsByStarId_ReturnsExpectedNumberOfMoons()
        {
            var result = await controller.Get(2, "planet");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Moon 2");
        }

        [TestMethod]
        public async Task Controller_GetAllMoonsByStarId_ReturnsUnexpectedMoon()
        {
            var result = await controller.Get(2, "planet");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Moon 1");
        }

        [TestMethod]
        public async Task Controller_AddMoon_ReturnsExpectedResult()
        {
            var result = await controller.Post(moon);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, moon.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_AddNullMoon_ReturnsException()
        {
            var result = await controller.Post(new Moon());
        }

        [TestMethod]
        public async Task Controller_AddMoonList_ReturnsExpectedResult()
        {
            var result = await controller.Post(moonsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), moonsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, moonsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, moonsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Controller_AddNullMoonList_ReturnsException()
        {
            var result = await controller.Post(new List<Moon>());
        }

        [TestMethod]
        public async Task Controller_SaveMoon_ReturnsExpectedResult()
        {
            var moonToSave = moons.SingleOrDefault(s => s.Id == 1);
            moonToSave.Name = "Moon 1 Saved";
            var result = await controller.Put(moonToSave.Id, moonToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, moonToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_SaveNullMoon_ReturnsException()
        {
            var moonToSave = moons.SingleOrDefault(s => s.Id == 88);
            moonToSave.Name = "Moon 1 Saved";
            var result = await controller.Put(moonToSave.Id, moonToSave);
        }

        [TestMethod]
        public async Task Controller_DeleteMoonWithValidId_ReturnsExpectedResult()
        {
            var moonToDelete = moons.SingleOrDefault(s => s.Id == 1);
            await controller.Delete(moonToDelete.Id);
            Assert.IsNull(await controller.Get(moonToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_DeleteMoonWithInvalidId_ReturnsException()
        {
            var moonToDelete = moons.SingleOrDefault(s => s.Id == 88);
            await controller.Delete(moonToDelete.Id);
        }
    }
}
