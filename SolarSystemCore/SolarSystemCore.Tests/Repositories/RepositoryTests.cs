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
        public IRepository<Star> repository { get; set; }
        public Star star { get; set; }
        public IList<Star> stars { get; set; }
        public IList<Star> starsToAdd { get; set; }
        public IList<Star> starsToAddWithDefaultGuids { get; set; }

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
            starsToAddWithDefaultGuids = testDataHelper.GetStarsToAddWithDefaultGuids();

            foreach (var s in stars)
            {
                dbContext.Stars.Add(s);
            }

            dbContext.SaveChanges();

            repository = new Repository<Star>(dbContext);
        }

        [TestMethod]
        public async Task Repository_GetAllAsync_ReturnsExpectedResult()
        {
            var result = await repository.GetAllAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Repository_GetAllAsync_NonReturnsExpectedResult()
        {
            var result = await repository.GetAllAsync();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, result.Count());
        }

        [TestMethod]
        public async Task Repository_FindAsync_ReturnsExpectedResult()
        {
            var result = await repository.FindAsync(s => s.Id == stars.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Star 1", result.FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task Repository_FindAsync_ReturnsNotExpectedResult()
        {
            var result = await repository.FindAsync(s => s.Id == stars.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Star 2", result.FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task Repository_SingleOrDefaultAsync_ReturnsExpectedResult()
        {
            var result = await repository.SingleOrDefaultAsync(s => s.Id == stars.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Star 1", result.Name);
        }

        [TestMethod]
        public async Task Repository_SingleOrDefaultAsync_ReturnsNotExpectedResult()
        {
            var result = await repository.SingleOrDefaultAsync(s => s.Id == stars.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Star 2", result.Name);
        }

        [TestMethod]
        public async Task Repository_FirstOrDefaultAsync_ReturnsExpectedResult()
        {
            var result = await repository.FirstOrDefaultAsync(s => s.Id == stars.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Star 1", result.Name);
        }

        [TestMethod]
        public async Task Repository_FirstOrDefaultAsync_ReturnsNotExpectedResult()
        {
            var result = await repository.FirstOrDefaultAsync(s => s.Id == stars.FirstOrDefault().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Star 2", result.Name);
        }

        [TestMethod]
        public async Task Repository_AddAsync_ReturnsExpectedResult()
        {
            var result = await repository.AddAsync(star);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, star.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Repository_AddAsync_ReturnsException()
        {
            var result = await repository.AddAsync(new Star());
        }

        [TestMethod]
        public async Task Repository_AddRangeAsync_ReturnsExpectedResult()
        {
            var result = await repository.AddRangeAsync(starsToAdd);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), starsToAdd.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, starsToAdd.FirstOrDefault().Name);
            Assert.AreEqual(result.Skip(1).Take(1).FirstOrDefault().Name, starsToAdd.Skip(1).Take(1).FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task Repository_AddRangeAsync_ReturnsNotExpectedResult()
        {
            var result = await repository.AddRangeAsync(starsToAdd);
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Repository_AddRangeAsync_ReturnsException()
        {
            var result = await repository.AddRangeAsync(starsToAddWithDefaultGuids);
        }

        [TestMethod]
        public async Task Repository_SaveChangesAsync_ReturnsExpectedResult()
        {
            var starToSave = stars.SingleOrDefault(s => s.Id == stars.FirstOrDefault().Id);
            starToSave.Name = "Star 1 Saved";
            var result = await repository.SaveAsync(starToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, starToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Repository_SaveChangesAsync_ReturnsException()
        {
            var starToSave = stars.SingleOrDefault(s => s.Id == new Guid());
            starToSave.Name = "Star 1 Saved";
            var result = await repository.SaveAsync(starToSave);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Repository_SaveChangesAsyncWithDefaultGuid_ReturnsException()
        {
           await repository.SaveAsync(new Star { Id = default(Guid) });
        }

        [TestMethod]
        public async Task Repository_DeleteAsync_ReturnsExpectedResult()
        {
            var starToDelete = stars.SingleOrDefault(s => s.Id == stars.FirstOrDefault().Id);
            await repository.DeleteAsync(starToDelete);
            Assert.IsNull(await repository.SingleOrDefaultAsync(x => x.Id == starToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Repository_DeleteAsync_ReturnsException()
        {
            var starToDelete = stars.SingleOrDefault(s => s.Id == new Guid());
            await repository.DeleteAsync(starToDelete);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Repository_DeleteAsyncWithDefaultGuid_ReturnsException()
        {
            await repository.DeleteAsync(new Star { Id = default(Guid) });
        }
    }
}
