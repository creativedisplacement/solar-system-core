using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarSystemCore.Data;
using SolarSystemCore.Models;
using SolarSystemCore.Repositories;
using SolarSystemCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarSystemCore.Tests.Services
{
    [TestClass]
    public class MoonServiceTests
    {
        public DBContext dbContext { get; set; }
        public IRepository<Moon> repository { get; set; }
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

            dbContext = new DBContext(options);

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


            foreach (var p in moons)
            {
                dbContext.Moons.Add(p);
            }

            dbContext.SaveChanges();

            repository = new Repository<Moon>(dbContext);
            service = new MoonService(repository);
        }

        [TestMethod]
        public async Task GetAllMoons_ReturnsExpectedNumberOfMoons()
        {
            var result = await service.GetAllMoonsAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetAllMoons_ReturnsUnexpectedNumberOfMoons()
        {
            var result = await service.GetAllMoonsAsync();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, (await service.GetAllMoonsAsync()).Count());
        }

        [TestMethod]
        public async Task GetAllMoonsByStarId_ReturnsExpectedNumberOfMoons()
        {
            var result = await service.GetAllMoonsByPlanetIdAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Moon 1");
        }

        [TestMethod]
        public async Task GetAllMoonsByStarId_ReturnsUnexpectedMoon()
        {
            var result = await service.GetAllMoonsByPlanetIdAsync(2);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Moon 1");
        }

        [TestMethod]
        public async Task GetMoon_ReturnsExpectedResult()
        {
            var result = await service.GetMoonAsync(2);
            Assert.IsNotNull(result);
            Assert.AreEqual("Moon 2", result.Name);
        }

        [TestMethod]
        public async Task GetMoon_ReturnsUnexpectedResult()
        {
            var result = await service.GetMoonAsync(1);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Moon 2", result.Name);
        }

        [TestMethod]
        public async Task FindMoon_ReturnsExpectedMoon()
        {
            var result = await service.FindMoonsAsync(p => p.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Moon 1");
        }

        [TestMethod]
        public async Task FindMoon_ReturnsUnexpectedMoon()
        {
            var result = await service.FindMoonsAsync(p => p.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Moon 2");
        }

        [TestMethod]
        public async Task AddMoon_ReturnsExpectedResult()
        {
            var result = await service.AddMoonAsync(moon);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task AddNullMoon_ReturnsException()
        {
            var result = await service.AddMoonAsync(new Moon());
        }

        [TestMethod]
        public async Task AddMoonList_ReturnsExpectedResult()
        {
            var result = await service.AddMoonsAsync(moonsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddNullMoonList_ReturnsException()
        {
            var moons = new List<Moon>();
            var result = await service.AddMoonsAsync(moons);
        }

        [TestMethod]
        public async Task SaveMoon_ReturnsExpectedResult()
        {
            var moonToSave = moons.SingleOrDefault(s => s.Id == 1);
            moonToSave.Name = "Moon 1 Saved";
            var result = await service.SaveMoonAsync(moonToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task SaveNullMoon_ReturnsException()
        {
            var moonToSave = moons.SingleOrDefault(s => s.Id == 88);
            moonToSave.Name = "Moon 1 Saved";
            var result = await service.SaveMoonAsync(moonToSave);
        }

        [TestMethod]
        public async Task DeleteMoonWithValidId_ReturnsExpectedResult()
        {
            var moonToDelete = moons.SingleOrDefault(s => s.Id == 1);
            var result = await service.DeleteMoonAsync(moonToDelete.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task DeleteMoonWithInvalidId_ReturnsException()
        {
            var moonToDelete = moons.SingleOrDefault(s => s.Id == 88);
            var result = await service.DeleteMoonAsync(moonToDelete.Id);
        }
    }
}
