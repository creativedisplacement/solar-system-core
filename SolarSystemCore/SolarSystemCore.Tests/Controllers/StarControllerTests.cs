using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class StarControllerTests
    {
        public Star star { get; set; }
        public IList<Star> stars { get; set; }
        public IList<Star> starsToAdd { get; set; }
        public StarController controller { get; set; }

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            var dbContext = new DBContext(options);

            stars = new List<Star>
            {
                new Star { Id = 1, Name = "Star 1", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1 },
                new Star { Id = 2, Name = "Star 2", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2 },
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
                new Star { Id = 3, Name = "Star 3", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1 },
                new Star { Id = 4, Name = "Star 4", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2 },
            };

            foreach (var s in stars)
            {
                dbContext.Stars.Add(s);
            }

            dbContext.SaveChanges();

            var repository = new Repository<Star>(dbContext);
            var service = new StarService(repository);
            controller = new StarController(service);
        }

        [TestMethod]
        public async Task Controller_GetAllStars_ReturnsExpectedNumberOfStars()
        {
            var result = await controller.Get();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Controller_GetAllStars_ReturnsUnexpectedNumberOfStars()
        {
            var result = await controller.Get();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, result.Count());
        }

        [TestMethod]
        public async Task Controller_GetStar_ReturnsExpectedResult()
        {
            var result = await controller.Get(1);
            Assert.IsNotNull(result);
            Assert.AreEqual("Star 1", result.Name);
        }

        [TestMethod]
        public async Task Controller_GetStar_ReturnsUnexpectedResult()
        {
            var result = await controller.Get(1);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Star 2", result.Name);
        }

        [TestMethod]
        public async Task Controller_AddStar_ReturnsExpectedResult()
        {
            var result = await controller.Post(star);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, star.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_AddNullStar_ReturnsException()
        {
            var result = await controller.Post(new Star());
        }

        [TestMethod]
        public async Task Controller_AddStarList_ReturnsExpectedResult()
        {
            var result = await controller.Post(starsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), starsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, starsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, starsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Controller_AddNullStarList_ReturnsException()
        {
            var result = await controller.Post(new List<Star>());
        }

        [TestMethod]
        public async Task Controller_SaveStar_ReturnsExpectedResult()
        {
            var starToSave = stars.SingleOrDefault(s => s.Id == 1);
            starToSave.Name = "Star 1 Saved";
            var result = await controller.Put(starToSave.Id, starToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, starToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_SaveNullStar_ReturnsException()
        {
            var starToSave = stars.SingleOrDefault(s => s.Id == 88);
            starToSave.Name = "Star 1 Saved";
            var result = await controller.Put(starToSave.Id, starToSave);
        }

        [TestMethod]
        public async Task Controller_DeleteStarWithValidId_ReturnsExpectedResult()
        {
            var starToDelete = stars.SingleOrDefault(s => s.Id == 1);
            await controller.Delete(starToDelete.Id);
            Assert.IsNull(await controller.Get(starToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_DeleteStarWithInvalidId_ReturnsException()
        {
            var starToDelete = stars.SingleOrDefault(s => s.Id == 88);
            await controller.Delete(starToDelete.Id);
        }
    }
}
