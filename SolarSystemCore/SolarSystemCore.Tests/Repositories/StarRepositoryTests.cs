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
    public class StarRepositoryTests
    {
        private IStarRepository _repository;
        private Star _star;
        private List<Star> _stars;
        private List<Star> _starsToAdd;

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

            foreach (var p in _stars)
            {
                dbContext.Stars.Add(p);
            }

            dbContext.SaveChanges();

            var repository = new Repository<Star>(dbContext);
            _repository = new StarRepository(repository);
        }

        [TestMethod]
        public async Task Repository_GetAllStars_ReturnsExpectedNumberOfStars()
        {
            var result = await _repository.GetAllStars();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Repository_GetAllStars_ReturnsUnexpectedNumberOfStars()
        {
            var result = await _repository.GetAllStars();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, (await _repository.GetAllStars()).Count());
        }

        [TestMethod]
        public async Task Repository_GetStar_ReturnsExpectedResult()
        {
            var result = await _repository.GetStar(new Guid("591D922F-D11C-469F-B61B-AF783D71E60A"));
            Assert.IsNotNull(result);
            Assert.AreEqual("Star 2", result.Name);

            Assert.IsNotNull(result.Planets);
            Assert.AreEqual(result.Planets.Count(), _stars.FirstOrDefault().Planets.Count());
            Assert.AreEqual(result.Planets.FirstOrDefault().Name, _stars.FirstOrDefault().Planets.FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task Repository_GetStar_ReturnsUnexpectedResult()
        {
            var result = await _repository.GetStar(new Guid("DF9AA280-C912-4E42-A5B5-4573CF97FDB0"));
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Star 2", result.Name);
        }

        [TestMethod]
        public async Task Repository_FindStar_ReturnsExpectedStar()
        {
            var result = await _repository.FindStars(p => p.Id == new Guid("DF9AA280-C912-4E42-A5B5-4573CF97FDB0"));
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Star 1");
        }

        [TestMethod]
        public async Task Repository_FindStar_ReturnsUnexpectedStar()
        {
            var result = await _repository.FindStars(p => p.Id == new Guid("DF9AA280-C912-4E42-A5B5-4573CF97FDB0"));
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Star 2");
        }

        [TestMethod]
        public async Task Repository_AddStar_ReturnsExpectedResult()
        {
            var result = await _repository.AddStar(_star);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, _star.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Repository_AddNullStar_ReturnsException()
        {
            var result = await _repository.AddStar(new Star());
        }

        [TestMethod]
        public async Task Repository_AddStarList_ReturnsExpectedResult()
        {
            var result = await _repository.AddStars(_starsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), _starsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, _starsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, _starsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task Repository_AddNullStarList_ReturnsException()
        {
            var stars = new List<Star>();
            var result = await _repository.AddStars(stars);

            Assert.AreEqual(stars.Any(), result.Any());
        }

        [TestMethod]
        public async Task Repository_SaveStar_ReturnsExpectedResult()
        {
            var starToSave = _stars.SingleOrDefault(s => s.Id == _stars.FirstOrDefault().Id);
            starToSave.Name = "Star 1 Saved";
            var result = await _repository.SaveStar(starToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, starToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Repository_SaveNullStar_ReturnsException()
        {
            var starToSave = _stars.SingleOrDefault(s => s.Id == new Guid("030B729D-60BF-4A26-B38C-F59F85296D35"));
            starToSave.Name = "Star 1 Saved";
            var result = await _repository.SaveStar(starToSave);
        }

        [TestMethod]
        public async Task Repository_DeleteStarWithValidId_ReturnsExpectedResult()
        {
            var starToDelete = _stars.SingleOrDefault(s => s.Id == new Guid("DF9AA280-C912-4E42-A5B5-4573CF97FDB0"));
            await _repository.DeleteStar(starToDelete);
            Assert.IsNull(await _repository.GetStar(starToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Repository_DeleteStarWithInvalidId_ReturnsException()
        {
            var starToDelete = _stars.SingleOrDefault(s => s.Id == new Guid("030B729D-60BF-4A26-B38C-F59F85296D35"));
            await _repository.DeleteStar(starToDelete);
        }
    }
}
