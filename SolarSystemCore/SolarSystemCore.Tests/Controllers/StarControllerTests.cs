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
        private Star _star;
        private List<Star> _stars;
        private List<Star> _starsToAdd;
        private StarController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            var dbContext = new DBContext(options);

            var testDataHelper = new TestHelper.StarData();
            _stars = testDataHelper.GetStars();
            _star = testDataHelper.GetStar();
            _starsToAdd = testDataHelper.GetStarsToAdd();

            foreach (var s in _stars)
            {
                dbContext.Stars.Add(s);
            }

            dbContext.SaveChanges();

            var repository = new Repository<Star>(dbContext);
            var starRepository = new StarRepository(repository);
            var service = new StarService(starRepository);
            var logger = new NullLogger<StarController>();
            IAppSettings appSettings = new AppSettings();
            _controller = new StarController(service, appSettings, logger);
        }

        [TestMethod]
        public async Task Controller_GetAllStars_ReturnsExpectedNumberOfStars()
        {
            var result = await _controller.Get();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Controller_GetAllStars_ReturnsUnexpectedNumberOfStars()
        {
            var result = await _controller.Get();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, result.Count());
        }

        [TestMethod]
        public async Task Controller_GetStar_ReturnsExpectedResult()
        {
            var result = await _controller.Get(_stars.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Star 1", result.Name);
        }

        [TestMethod]
        public async Task Controller_GetStar_ReturnsUnexpectedResult()
        {
            var result = await _controller.Get(_stars.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Star 2", result.Name);
        }

        [TestMethod]
        public async Task Controller_AddStar_ReturnsExpectedResult()
        {
            var result = await _controller.Post(_star);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, _star.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_AddNullStar_ReturnsException()
        {
            var result = await _controller.Post(new Star());
        }

        [TestMethod]
        public async Task Controller_AddStarList_ReturnsExpectedResult()
        {
            var result = await _controller.Post(_starsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), _starsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, _starsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, _starsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Controller_AddNullStarList_ReturnsException()
        {
            var result = await _controller.Post(new List<Star>());
        }

        [TestMethod]
        public async Task Controller_SaveStar_ReturnsExpectedResult()
        {
            var starToSave = _stars.SingleOrDefault(s => s.Id == _stars.FirstOrDefault().Id);
            starToSave.Name = "Star 1 Saved";
            var result = await _controller.Put(starToSave.Id, starToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, starToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_SaveNullStar_ReturnsException()
        {
            var starToSave = _stars.SingleOrDefault(s => s.Id == new Guid());
            starToSave.Name = "Star 1 Saved";
            var result = await _controller.Put(starToSave.Id, starToSave);
        }

        [TestMethod]
        public async Task Controller_DeleteStarWithValidId_ReturnsExpectedResult()
        {
            var starToDelete = _stars.SingleOrDefault(s => s.Id == _stars.FirstOrDefault().Id);
            await _controller.Delete(starToDelete.Id);
            Assert.IsNull(await _controller.Get(starToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Controller_DeleteStarWithInvalidId_ReturnsException()
        {
            var starToDelete = _stars.SingleOrDefault(s => s.Id == new Guid());
            await _controller.Delete(starToDelete.Id);
        }
    }
}
