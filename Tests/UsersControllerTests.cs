using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Movies.Context;
using Movies.Controllers;
using Movies.DataTransferObjects;
using Movies.Models;
using Movies.Tests;
using Movies.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Movies.Tests
{
    [TestFixture]
    public class UsersControllerTests
    {
        private Mock<IMoviesContext> _mockContext;
        private Mock<IMapper> _mockMapper;
        private UsersController _controller;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<IMoviesContext>();
            _mockMapper = new Mock<IMapper>();
            _controller = new UsersController(_mockContext.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetUsers_ReturnsListOfUsers()
        {
            // Arrange
            var users = new List<User>
        {
            new User { Id = 1, Name = "John Doe" },
            new User { Id = 2, Name = "Jane Smith" }
        };
            var mockDbSet = MockDbSetHelper.CreateMockDbSet(users);
            _mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

            // Act
            var result = await _controller.GetUsers();

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnedUsers = okResult.Value as IEnumerable<User>;
            Assert.AreEqual(users.Count, returnedUsers.Count());
        }

        [Test]
        public async Task GetUser_ValidId_ReturnsUser()
        {
            //// Arrange
            //var user = new User { Id = 1, Name = "John Doe", Comments = new List<Comment>(), InterestedCategories = new List<Category>() };
            //_mockContext.Setup(c => c.Users.FindAsync(It.IsAny<int>())).ReturnsAsync(user);

            //// Act
            //var result = await _controller.GetUser(user.Id);

            //// Assert
            //var okResult = result.Result as OkObjectResult;
            //var returnedUser = okResult.Value as User;
            //Assert.AreEqual(user.Id, returnedUser.Id);
            //Assert.AreEqual(user.Name, returnedUser.Name);
            // Arrange
            var options = new DbContextOptionsBuilder<MoviesContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new MoviesContext(options))
            {
                var user = new User { Id = 1, Name = "John Doe", Comments = new List<Comment>(), InterestedCategories = new List<Category>() };
                context.Users.Add(user);
                context.SaveChanges();

                var controller = new UsersController(context, _mockMapper.Object);

                // Act
                var result = await controller.GetUser(user.Id);

                // Assert
                var okResult = result.Result as OkObjectResult;
                var returnedUser = okResult.Value as User;
                Assert.AreEqual(user.Id, returnedUser.Id);
                Assert.AreEqual(user.Name, returnedUser.Name);
            }
        }

        [Test]
        public async Task CreateUser_ValidUserDto_CreatesUser()
        {
            // Arrange
            var userDto = new UserDto { Name = "John Doe", InterestedCategories = new List<CategoryDto> { new CategoryDto { Name = "Comedy" } } };
            var user = new User { Id = 1, Name = userDto.Name, InterestedCategories = new List<Category> { new Category { Id = 1, Name = "Comedy" } }, Comments = new List<Comment>() };
            _mockMapper.Setup(m => m.Map<User>(userDto)).Returns(user);
            var users = new List<User>();
            var mockDbSet = MockDbSetHelper.CreateMockDbSet(users);
            _mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.Users.Add(It.IsAny<User>())).Callback<User>(u => users.Add(u)).Returns((EntityEntry<User>)null);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(1);

            // Act
            var result = await _controller.CreateUser(userDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as CreateUserResponse;
            Assert.AreEqual(user.Id, response.UserId);
        }

        [Test]
        public async Task DeleteUser_ValidId_DeletesUser()
        {
            // Arrange
            var id = 1;
            var user = new User { Id = id, Name = "John Doe" };
            _mockContext.Setup(c => c.Users.FindAsync(id)).ReturnsAsync(user);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteUser(id);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
            _mockContext.Verify(c => c.Users.Remove(user), Times.Once);
        }
    }
}