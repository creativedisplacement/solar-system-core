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
    public class StarServiceTests
    {
        public IStarService service { get; set; }
        public Star star { get; set; }
        public IList<Star> stars { get; set; }
        public IList<Star> starsToAdd { get; set; }

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            var dbContext = new DBContext(options);

            stars = new List<Star>
            {
                new Star { Id = 1, Name = "Star 1", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1},
                new Star { Id = 2, Name = "Star 2", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2},
            };

            star = new Star
            {
                Id = 3,
                CreatedDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now,
                Name = "Star 3"

            };

            starsToAdd = new List<Star>
            {
                new Star { Id = 3, Name = "Star 3", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1},
                new Star { Id = 4, Name = "Star 4", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2},
            };


            foreach (var p in stars)
            {
                dbContext.Stars.Add(p);
            }

            dbContext.SaveChanges();

            var repository = new Repository<Star>(dbContext);
            service = new StarService(repository);
        }

        [TestMethod]
        public async Task Service_GetAllStars_ReturnsExpectedNumberOfStars()
        {
            var result = await service.GetAllStarsAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Service_GetAllStars_ReturnsUnexpectedNumberOfStars()
        {
            var result = await service.GetAllStarsAsync();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, (await service.GetAllStarsAsync()).Count());
        }

        [TestMethod]
        public async Task Service_GetStar_ReturnsExpectedResult()
        {
            var result = await service.GetStarAsync(2);
            Assert.IsNotNull(result);
            Assert.AreEqual("Star 2", result.Name);
        }

        [TestMethod]
        public async Task Service_GetStar_ReturnsUnexpectedResult()
        {
            var result = await service.GetStarAsync(1);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Star 2", result.Name);
        }

        [TestMethod]
        public async Task Service_FindStar_ReturnsExpectedStar()
        {
            var result = await service.FindStarsAsync(p => p.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Star 1");
        }

        [TestMethod]
        public async Task Service_FindStar_ReturnsUnexpectedStar()
        {
            var result = await service.FindStarsAsync(p => p.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Star 2");
        }

        [TestMethod]
        public async Task Service_AddStar_ReturnsExpectedResult()
        {
            var result = await service.AddStarAsync(star);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, star.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Service_AddNullStar_ReturnsException()
        {
            var result = await service.AddStarAsync(new Star());
        }

        [TestMethod]
        public async Task Service_AddStarList_ReturnsExpectedResult()
        {
            var result = await service.AddStarsAsync(starsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), starsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, starsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, starsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Service_AddNullStarList_ReturnsException()
        {
            var stars = new List<Star>();
            var result = await service.AddStarsAsync(stars);
        }

        [TestMethod]
        public async Task Service_SaveStar_ReturnsExpectedResult()
        {
            var starToSave = stars.SingleOrDefault(s => s.Id == 1);
            starToSave.Name = "Star 1 Saved";
            var result = await service.SaveStarAsync(starToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, starToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Service_SaveNullStar_ReturnsException()
        {
            var starToSave = stars.SingleOrDefault(s => s.Id == 88);
            starToSave.Name = "Star 1 Saved";
            var result = await service.SaveStarAsync(starToSave);
        }

        [TestMethod]
        public async Task Service_DeleteStarWithValidId_ReturnsExpectedResult()
        {
            var starToDelete = stars.SingleOrDefault(s => s.Id == 1);
            await service.DeleteStarAsync(starToDelete.Id);
            Assert.IsNull(await service.GetStarAsync(starToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Service_DeleteStarWithInvalidId_ReturnsException()
        {
            var starToDelete = stars.SingleOrDefault(s => s.Id == 88);
            await service.DeleteStarAsync(starToDelete.Id);
        }
    }
}
