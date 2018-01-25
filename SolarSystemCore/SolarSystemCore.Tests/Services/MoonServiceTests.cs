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
    public class MoonServiceTests
    {
        public Mock<IRepository<Moon>> repository { get; set; }
        public List<Moon> moons { get; set; }
        public IMoonService service { get; set; }
        public Moon moon { get; set; }

        [TestInitialize]
        public void Setup()
        {
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

            repository = new Mock<IRepository<Moon>>();
            service = new MoonService(repository.Object);
        }

        [TestMethod]
        public async Task GetAllMoons_ReturnsExpectedNumberOfMoons()
        {
            repository
                .Setup(p => p.GetAllAsync())
                .ReturnsAsync(moons);

            var result = await service.GetAllMoonsAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetAllMoons_ReturnsUnexpectedNumberOfMoons()
        {
            repository
                .Setup(p => p.GetAllAsync())
                .ReturnsAsync(moons);

            var result = await service.GetAllMoonsAsync();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(3, (await service.GetAllMoonsAsync()).Count());
        }

        [TestMethod]
        public async Task GetAllMoonsByPlanetId_ReturnsExpectedNumberOfMoons()
        {
            repository
               .Setup(p => p.FindAsync(It.IsAny<Expression<Func<Moon, bool>>>()))
               .ReturnsAsync(moons.Where(p => p.PlanetId == 1));

            var result = await service.GetAllMoonsByPlanetIdAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Moon 1");
        }

        [TestMethod]
        public async Task GetAllMoonsByPlanetId_ReturnsUnexpectedMoon()
        {
            repository
                .Setup(p => p.FindAsync(It.IsAny<Expression<Func<Moon, bool>>>()))
                .ReturnsAsync(moons.Where(p => p.PlanetId == 2));

            var result = await service.GetAllMoonsByPlanetIdAsync(2);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Moon 1");
        }

        [TestMethod]
        public async Task GetMoon_ReturnsExpectedMoon()
        {
            repository
                .Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Moon, bool>>>()))
                .ReturnsAsync(moon);

            var result = await service.GetMoonAsync(3);
            Assert.IsInstanceOfType(moon, typeof(Moon));
            Assert.AreEqual("Moon 3", moon.Name);
        }

        [TestMethod]
        public async Task GetMoon_ReturnsIncorrectMoon()
        {
            repository
              .Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Moon, bool>>>()))
              .ReturnsAsync(moon);

            var result = await service.GetMoonAsync(1);
            Assert.IsInstanceOfType(moon, typeof(Moon));
            Assert.AreNotEqual("Moon 1", moon.Name);
        }

        [TestMethod]
        public async Task FindMoon_ReturnsExpectedMoon()
        {
            repository
              .Setup(p => p.FindAsync(It.IsAny<Expression<Func<Moon, bool>>>()))
              .ReturnsAsync(moons.Where(p => p.Id == 1));

            var result = await service.FindMoonsAsync(p => p.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(result.FirstOrDefault().Name, "Moon 1");
        }

        [TestMethod]
        public async Task FindMoon_ReturnsUnexpectedMoon()
        {
            repository
               .Setup(p => p.FindAsync(It.IsAny<Expression<Func<Moon, bool>>>()))
               .ReturnsAsync(moons.Where(p => p.Id == 1));

            var result = await service.FindMoonsAsync(p => p.Id == 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreNotEqual(result.FirstOrDefault().Name, "Moon 2");
        }

        [TestMethod]
        public async Task AddMoon_ReturnsTrue()
        {
            var result = await service.AddMoonAsync(moon);
            repository.Verify(x => x.AddAsync(moon), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task AddNullMoon_ReturnsException()
        {
            var result = await service.AddMoonAsync(new Moon());
            repository.Verify(x => x.AddAsync(moon), Times.Never());
        }

        [TestMethod]
        public async Task AddMoonList_ReturnsTrue()
        {
            var planets = new List<Moon> {
                new Moon { Id = 3, Name = "Moon 3", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now},
                new Moon { Id = 4, Name = "Moon 4", CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now},
            };
            var result = await service.AddMoonsAsync(planets);
            repository.Verify(x => x.AddRangeAsync(planets), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddNullMoonList_ReturnsNullException()
        {
            var planets = new List<Moon>();
            var result = await service.AddMoonsAsync(planets);
            repository.Verify(x => x.AddRangeAsync(planets), Times.Never());
        }

        [TestMethod]
        public async Task SaveMoon_ReturnsTrue()
        {
            repository
             .Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Moon, bool>>>()))
             .ReturnsAsync(moon);

            var result = await service.SaveMoonAsync(moon);
            repository.Verify(x => x.SaveAsync(moon), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task SaveNullMoon_ReturnsException()
        {
            var result = await service.SaveMoonAsync(null);
            repository.Verify(x => x.SaveAsync(moon), Times.Never());
        }

        [TestMethod]
        public async Task DeleteMoonWithValidId()
        {
            repository
              .Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Moon, bool>>>()))
              .ReturnsAsync(moon);

            var result = await service.DeleteMoonAsync(3);
            repository.Verify(x => x.DeleteAsync(moon), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task DeleteMoonWithInvalidId_ReturnsException()
        {
            repository
              .Setup(p => p.FirstOrDefaultAsync(It.IsAny<Expression<Func<Moon, bool>>>()))
              .ReturnsAsync(new Moon());

            await service.DeleteMoonAsync(88);
            repository.Verify(x => x.DeleteAsync(moon), Times.Never());
        }
    }
}
