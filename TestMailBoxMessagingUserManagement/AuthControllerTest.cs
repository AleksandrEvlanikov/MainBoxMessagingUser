using MailBoxMessagingUserManagement.Controllers;
using MailBoxMessagingUserManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;

namespace MailBoxMessagingUserManagement
{
    public class AuthControllerTest
    {

        private AuthController _authController;
        private Mock<ApplicationDbContext> _mockDbContext;

        public AuthControllerTest()
        {
            _mockDbContext = new Mock<ApplicationDbContext>();
            _authController = new AuthController(_mockDbContext.Object);
        }

        private DbSet<T> MockDbSet<T>(List<T> data) where T : class
        {
            var queryableData = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryableData.GetEnumerator());

            return mockSet.Object;
        }



        [Fact]
        public async Task RegisterValidUserReturnsSuccess()
        {
            _mockDbContext.Setup(context => context.Users)
                .Returns(MockDbSet(new List<User>()));

            var result = await _authController.Register("test@example.com", "Abcdef");

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

            var viewResult = (ViewResult)result;
            Assert.Equal("Login", viewResult.ViewName);
            Assert.Equal("Регистрация прошла успешно. Теперь вы можете войти.", viewResult.ViewData["SuccessMessage"]);
        }
        [Fact]
        public void RegisterExistingUser()
        {
            _mockDbContext.Setup(context => context.Users)
                .Returns(MockDbSet(new List<User> { new User { Email = "test@example.com" } }));

            var result = _authController.Register("test@example.com", "Abcdef").Result;

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

            var viewResult = (ViewResult)result;
            Assert.Equal("Пользователь с таким email уже существует.", viewResult.ViewData["ErrorMessage"]);
        }

        [Fact]
        public async Task RegisterInvalidPassword()
        {
            var result = await _authController.Register("test@example.com", "abc");

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

            var viewResult = (ViewResult)result;
            Assert.Equal("Некорректный пароль. Пароль должен быть больше 5 символов и с 1 заглавной буквой.", viewResult.ViewData["ErrorMessage"]);

        }
    }
}