using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarSystemCore.Data;
using SolarSystemCore.Models;
using SolarSystemCore.Repositories;
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

            star = new Star { Id = 3, Name = "Star 1", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1 };

            starsToAdd = new List<Star>
            {
                new Star { Id = 3, Name = "Star 1", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 1 },
                new Star { Id = 4, Name = "Star 2", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, Ordinal = 2 },
            };


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
            var result = await repository.FindAsync(s => s.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual("Star 1", result.FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task Repository_FindAsync_ReturnsNotExpectedResult()
        {
            var result = await repository.FindAsync(s => s.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Star 2", result.FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task Repository_SingleOrDefaultAsync_ReturnsExpectedResult()
        {
            var result = await repository.SingleOrDefaultAsync(s => s.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual("Star 1", result.Name);
        }

        [TestMethod]
        public async Task Repository_SingleOrDefaultAsync_ReturnsNotExpectedResult()
        {
            var result = await repository.SingleOrDefaultAsync(s => s.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreNotEqual("Star 2", result.Name);
        }

        [TestMethod]
        public async Task Repository_FirstOrDefaultAsync_ReturnsExpectedResult()
        {
            var result = await repository.FirstOrDefaultAsync(s => s.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual("Star 1", result.Name);
        }

        [TestMethod]
        public async Task Repository_FirstOrDefaultAsync_ReturnsNotExpectedResult()
        {
            var result = await repository.FirstOrDefaultAsync(s => s.Id == 1);
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
        [ExpectedException(typeof(InvalidOperationException))]
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
        public async Task Repository_SaveChangesAsync_ReturnsExpectedResult()
        {
            var starToSave = stars.SingleOrDefault(s => s.Id == 1);
            starToSave.Name = "Star 1 Saved";
            var result = await repository.SaveAsync(starToSave);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, starToSave.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task Repository_SaveChangesAsync_ReturnsException()
        {
            var starToSave = stars.SingleOrDefault(s => s.Id == 88);
            starToSave.Name = "Star 1 Saved";
            var result = await repository.SaveAsync(starToSave);
        }

        [TestMethod]
        public async Task Repository_DeleteAsync_ReturnsExpectedResult()
        {
            var starToDelete = stars.SingleOrDefault(s => s.Id == 1);
            await repository.DeleteAsync(starToDelete);
            Assert.IsNull(await repository.SingleOrDefaultAsync(x => x.Id == starToDelete.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Repository_DeleteAsync_ReturnsException()
        {
            var starToDelete = stars.SingleOrDefault(s => s.Id == 88);
            await repository.DeleteAsync(starToDelete);
        }
    }
}
