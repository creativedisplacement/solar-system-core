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
    public class RepositoryTests
    {
        private IRepository<Star> _repository;
        private Star _star;
        private List<Star> _stars;
        private List<Star> _starsToAdd;
        private List<Star> _starsToAddWithDefaultGuids;

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
            _starsToAddWithDefaultGuids = testDataHelper.GetStarsToAddWithDefaultGuids();

            foreach (var s in _stars)
            {
                dbContext.Stars.Add(s);
            }

            dbContext.SaveChanges();

            _repository = new Repository<Star>(dbContext);
        }

        [TestMethod]
        public async Task Repository_GetAllAsync_ReturnsExpectedResult()
        {
            var result = await _repository.GetAllAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Repository_GetAllAsync_NonReturnsExpectedResult()
        {
            var result = await _repository.GetAllAsync();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, result.Count());
        }

        [TestMethod]
        public async Task Repository_FindAsync_ReturnsExpectedResult()
        {
            var result = await _repository.FindAsync(s => s.Id == _stars.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Star 1", result.FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task Repository_FindAsync_ReturnsNotExpectedResult()
        {
            var result = await _repository.FindAsync(s => s.Id == _stars.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Star 2", result.FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task Repository_SingleOrDefaultAsync_ReturnsExpectedResult()
        {
            var result = await _repository.SingleOrDefaultAsync(s => s.Id == _stars.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Star 1", result.Name);
        }

        [TestMethod]
        public async Task Repository_SingleOrDefaultAsync_ReturnsNotExpectedResult()
        {
            var result = await _repository.SingleOrDefaultAsync(s => s.Id == _stars.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Star 2", result.Name);
        }

        [TestMethod]
        public async Task Repository_FirstOrDefaultAsync_ReturnsExpectedResult()
        {
            var result = await _repository.FirstOrDefaultAsync(s => s.Id == _stars.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Star 1", result.Name);
        }

        [TestMethod]
        public async Task Repository_FirstOrDefaultAsync_ReturnsNotExpectedResult()
        {
            var result = await _repository.FirstOrDefaultAsync(s => s.Id == _stars.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Star 2", result.Name);
        }

        [TestMethod]
        public async Task Repository_AddAsync_ReturnsExpectedResult()
        {
            var result = await _repository.AddAsync(_star);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, _star.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Repository_AddAsync_ReturnsException()
        {
            var result = await _repository.AddAsync(new Star());
        }

        [TestMethod]
        public async Task Repository_AddRangeAsync_ReturnsExpectedResult()
        {
            var result = await _repository.AddRangeAsync(_starsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), _starsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, _starsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, _starsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task Repository_AddRangeAsync_ReturnsNotExpectedResult()
        {
            var result = await _repository.AddRangeAsync(_starsToAdd);
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Repository_AddRangeAsync_ReturnsException()
        {
            var result = await _repository.AddRangeAsync(_starsToAddWithDefaultGuids);
        }

        [TestMethod]
        public async Task Repository_SaveChangesAsync_ReturnsExpectedResult()
        {
            var starToSave = _stars.SingleOrDefault(s => s.Id == _stars.FirstOrDefault().Id);
            starToSave.Name = "Star 1 Saved";
            var result = await _repository.SaveAsync(starToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, starToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Repository_SaveChangesAsync_ReturnsException()
        {
            var starToSave = _stars.SingleOrDefault(s => s.Id == new Guid());
            starToSave.Name = "Star 1 Saved";
            var result = await _repository.SaveAsync(starToSave);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Repository_SaveChangesAsyncWithDefaultGuid_ReturnsException()
        {
           await _repository.SaveAsync(new Star { Id = default(Guid) });
        }

        [TestMethod]
        public async Task Repository_DeleteAsync_ReturnsExpectedResult()
        {
            var starToDelete = _stars.SingleOrDefault(s => s.Id == _stars.FirstOrDefault().Id);
            await _repository.DeleteAsync(starToDelete);
            Assert.IsNull(await _repository.SingleOrDefaultAsync(x => x.Id == starToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Repository_DeleteAsync_ReturnsException()
        {
            var starToDelete = _stars.SingleOrDefault(s => s.Id == new Guid());
            await _repository.DeleteAsync(starToDelete);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Repository_DeleteAsyncWithDefaultGuid_ReturnsException()
        {
            await _repository.DeleteAsync(new Star { Id = default(Guid) });
        }
    }
}
