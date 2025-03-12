using NSubstitute;
using WebhookTester.Core.Common;
using WebhookTester.Core.Entities;
using WebhookTester.Core.Interfaces;
using WebhookTester.Core.Services;

namespace WebhookTester.Core.Tests
{
    [TestClass]
    public sealed class TokenServiceTests
    {
        private readonly ITokenRepository _repository = Substitute.For<ITokenRepository>();
        private TokenService _tokenService = null!;

        [TestInitialize]
        public void Setup()
        {
            _tokenService = new TokenService(_repository);
        }

        [TestMethod]
        public async Task CreateToken_ShouldReturnNewGuid()
        {
            // Arrange
            _repository.AddAsync(Arg.Any<Token>()).Returns(Task.CompletedTask);

            // Act
            var result = await _tokenService.CreateToken();
            var result2 = await _tokenService.CreateToken();

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result2.Success);
            Assert.AreNotEqual(Guid.Empty, result.Data.Id);
            Assert.AreNotEqual(result.Data.Id, result2.Data.Id);
        }

        [TestMethod]
        public async Task ValidateToken_WithMalformattedToken_ShouldReturnBadRequest()
        {
            // Arrange
            var token = "123%4..432";

            // Act
            var result = await _tokenService.ValidateToken(token.ToString());

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(ErrorCode.BadRequest, result.Error.Code);
        }

        [TestMethod]
        public async Task ValidateToken_WithEmptyToken_ShouldReturnBadRequest()
        {
            // Arrange
            var token = string.Empty;

            // Act
            var result = await _tokenService.ValidateToken(token.ToString());

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(ErrorCode.BadRequest, result.Error.Code);
        }

        [TestMethod]
        public async Task ValidateToken_WithNullToken_ShouldReturnUnauthorized()
        {
            // Arrange
            var token = Guid.NewGuid();
            _repository.GetByIdAsync(token).Returns((Token?)null);

            // Act
            var result = await _tokenService.ValidateToken(token.ToString());

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(ErrorCode.Unauthorized, result.Error.Code);
        }

        [TestMethod]
        public async Task ValidateToken_WithExpiredToken_ShouldReturnUnauthorized()
        {
            // Arrange
            var token = Guid.NewGuid();
            _repository.GetByIdAsync(token).Returns(new Token() { Id = token, Created = DateTimeOffset.UtcNow.AddYears(-3) });

            // Act
            var result = await _tokenService.ValidateToken(token.ToString());

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(ErrorCode.Unauthorized, result.Error.Code);
        }

        [TestMethod]
        public async Task ValidateToken_WithValidToken_ShouldReturnSuccess()
        {
            // Arrange
            var token = Guid.NewGuid();
            _repository.GetByIdAsync(token).Returns(new Token() { Id = token, Created = DateTimeOffset.UtcNow.AddMinutes(-20)});

            // Act
            var result = await _tokenService.ValidateToken(token.ToString());

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(token, result.Data.Id);
        }
    }
}
