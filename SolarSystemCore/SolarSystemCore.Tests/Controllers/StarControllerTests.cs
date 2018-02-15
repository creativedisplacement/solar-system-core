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
    public class StarControllerTests
    {
        private Star star { get; set; }
        private IList<Star> stars { get; set; }
        private IList<Star> starsToAdd { get; set; }
        private StarController controller { get; set; }

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            var dbContext = new DBContext(options);

            var testDataHelper = new TestHelper.StarData();
            stars = testDataHelper.GetStars();
            star = testDataHelper.GetStar();
            starsToAdd = testDataHelper.GetStarsToAdd();

            foreach (var s in stars)
            {
                dbContext.Stars.Add(s);
            }

            dbContext.SaveChanges();

            var repository = new Repository<Star>(dbContext);
            var service = new StarService(repository);
            var logger = new NullLogger<StarController>();
            IAppSettings appSettings = new AppSettings();
            controller = new StarController(service, appSettings, logger);
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
            var result = await controller.Get(stars.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Star 1", result.Name);
        }

        [TestMethod]
        public async Task Controller_GetStar_ReturnsUnexpectedResult()
        {
            var result = await controller.Get(stars.FirstOrDefault().Id);
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
            var starToSave = stars.SingleOrDefault(s => s.Id == stars.FirstOrDefault().Id);
            starToSave.Name = "Star 1 Saved";
            var result = await controller.Put(starToSave.Id, starToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, starToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_SaveNullStar_ReturnsException()
        {
            var starToSave = stars.SingleOrDefault(s => s.Id == new Guid());
            starToSave.Name = "Star 1 Saved";
            var result = await controller.Put(starToSave.Id, starToSave);
        }

        [TestMethod]
        public async Task Controller_DeleteStarWithValidId_ReturnsExpectedResult()
        {
            var starToDelete = stars.SingleOrDefault(s => s.Id == stars.FirstOrDefault().Id);
            await controller.Delete(starToDelete.Id);
            Assert.IsNull(await controller.Get(starToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_DeleteStarWithInvalidId_ReturnsException()
        {
            var starToDelete = stars.SingleOrDefault(s => s.Id == new Guid());
            await controller.Delete(starToDelete.Id);
        }
    }
}
