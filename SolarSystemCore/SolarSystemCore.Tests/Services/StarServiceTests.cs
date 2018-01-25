using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SolarSystemCore.Models;
using SolarSystemCore.Repositories;
using SolarSystemCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystemCore.Tests.Services
{
    [TestClass]
    public class StarServiceTests
    {
        public Mock<IRepository<Star>> repository { get; set; }
        public List<Star> stars { get; set; }
        public IStarService service { get; set; }
        public Star star { get; set; }

        [TestInitialize]
        public void Setup()
        {
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

            repository = new Mock<IRepository<Star>>();
            service = new StarService(repository.Object);
        }

        [TestMethod]
        public async Task GetAllStars_ReturnsExpectedNumberOfStars()
        {
            repository
                .Setup(p => p.GetAllAsync())
                .ReturnsAsync(stars);

            var result = await service.GetAllStarsAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetAllStars_ReturnsUnexpectedNumberOfStars()
        {
            repository
                .Setup(p => p.GetAllAsync())
                .ReturnsAsync(stars);

            var result = await service.GetAllStarsAsync();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, (await service.GetAllStarsAsync()).Count());
        }

        [TestMethod]
        public async Task GetStar_ReturnsExpectedStar()
        {
            repository
                .Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Star, bool>>>()))
                .ReturnsAsync(star);

            var result = await service.GetStarAsync(3);
            Assert.IsInstanceOfType(star, typeof(Star));
            Assert.AreEqual("Star 3", star.Name);
        }

        [TestMethod]
        public async Task GetStar_ReturnsIncorrectStar()
        {
            repository
              .Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Star, bool>>>()))
              .ReturnsAsync(star);

            var result = await service.GetStarAsync(1);
            Assert.IsInstanceOfType(star, typeof(Star));
            Assert.AreNotEqual("Star 1", star.Name);
        }

        [TestMethod]
        public async Task FindStar_ReturnsExpectedStar()
        {
            repository
              .Setup(p => p.FindAsync(It.IsAny<Expression<Func<Star, bool>>>()))
              .ReturnsAsync(stars.Where(p => p.Id == 1));

            var result = await service.FindStarsAsync(p => p.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Star 1");
        }

        [TestMethod]
        public async Task FindStar_ReturnsUnexpectedStar()
        {
            repository
               .Setup(p => p.FindAsync(It.IsAny<Expression<Func<Star, bool>>>()))
               .ReturnsAsync(stars.Where(p => p.Id == 1));

            var result = await service.FindStarsAsync(p => p.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Star 2");
        }

        [TestMethod]
        public async Task AddStar_ReturnsTrue()
        {
            var result = await service.AddStarAsync(star);
            repository.Verify(x => x.AddAsync(star), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task AddNullStar_ReturnsException()
        {
            var result = await service.AddStarAsync(new Star());
            repository.Verify(x => x.AddAsync(star), Times.Never());
        }

        [TestMethod]
        public async Task AddStarList_ReturnsTrue()
        {
            var planets = new List<Star> {
                new Star { Id = 3, Name = "Star 3", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now},
                new Star { Id = 4, Name = "Star 4", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now},
            };
            var result = await service.AddStarsAsync(planets);
            repository.Verify(x => x.AddRangeAsync(planets), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddNullStarList_ReturnsNullException()
        {
            var planets = new List<Star>();
            var result = await service.AddStarsAsync(planets);
            repository.Verify(x => x.AddRangeAsync(planets), Times.Never());
        }

        [TestMethod]
        public async Task SaveStar_ReturnsTrue()
        {
            repository
             .Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Star, bool>>>()))
             .ReturnsAsync(star);

            var result = await service.SaveStarAsync(star);
            repository.Verify(x => x.SaveAsync(star), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task SaveNullStar_ReturnsException()
        {
            var result = await service.SaveStarAsync(null);
            repository.Verify(x => x.SaveAsync(star), Times.Never());
        }

        [TestMethod]
        public async Task DeleteStarWithValidId()
        {
            repository
              .Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Star, bool>>>()))
              .ReturnsAsync(star);

            var result = await service.DeleteStarAsync(3);
            repository.Verify(x => x.DeleteAsync(star), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task DeleteStarWithInvalidId_ReturnsException()
        {
            repository
              .Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Star, bool>>>()))
              .ReturnsAsync(new Star());

            await service.DeleteStarAsync(88);
            repository.Verify(x => x.DeleteAsync(star), Times.Never());
        }
    }
}
