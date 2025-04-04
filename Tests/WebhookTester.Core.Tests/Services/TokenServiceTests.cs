﻿using NSubstitute;
using WebhookTester.Core.Common;
using WebhookTester.Core.Entities;
using WebhookTester.Core.Interfaces;
using WebhookTester.Core.Services;

namespace WebhookTester.Core.Tests.Services
{
    [TestClass]
    public sealed class TokenServiceTests
    {
        private readonly ITokenRepository _repository = Substitute.For<ITokenRepository>();
        private readonly ICache<Token> _cache = Substitute.For<ICache<Token>>();
        private TokenService _tokenService = null!;

        private readonly DateTimeOffset _validLastUsedTime = DateTimeOffset.UtcNow.AddMinutes(-20);
        private readonly DateTimeOffset _expiredLastUsedTime = DateTimeOffset.UtcNow.AddYears(-3);

        [TestInitialize]
        public void Setup()
        {
            _tokenService = new TokenService(_repository, _cache);
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
        public async Task ValidateToken_WithNonExistentToken_ShouldReturnUnauthorized()
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
        public async Task ValidateToken_WithExpiredUncachedToken_ShouldReturnUnauthorized()
        {
            // Arrange
            var token = Guid.NewGuid();
            _repository.GetByIdAsync(token).Returns(new Token() { Id = token, LastUsed = _expiredLastUsedTime });

            // Act
            var result = await _tokenService.ValidateToken(token.ToString());

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(ErrorCode.Unauthorized, result.Error.Code);
        }

        [TestMethod]
        public async Task ValidateToken_WithValidUncachedToken_ShouldReturnSuccess()
        {
            // Arrange
            var token = Guid.NewGuid();
            _repository.GetByIdAsync(token).Returns(new Token() { Id = token, LastUsed = _validLastUsedTime });

            // Act
            var result = await _tokenService.ValidateToken(token.ToString());

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(token, result.Data.Id);
        }

        [TestMethod]
        public async Task ValidateToken_WithValidCachedToken_ShouldReturnSuccess()
        {
            // Arrange
            var token = Guid.NewGuid();
            _cache.GetAsync(Arg.Any<string>()).Returns(new Token() { Id = token, LastUsed = _validLastUsedTime });

            // Act
            var result = await _tokenService.ValidateToken(token.ToString());

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(token, result.Data.Id);
        }

        [TestMethod]
        public async Task ValidateToken_WithExpiredCachedToken_ShouldReturnUnauthorized()
        {
            // Arrange
            var token = Guid.NewGuid();
            _cache.GetAsync(Arg.Any<string>()).Returns(new Token() { Id = token, LastUsed = _expiredLastUsedTime });

            // Act
            var result = await _tokenService.ValidateToken(token.ToString());

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(ErrorCode.Unauthorized, result.Error.Code);
        }

        [TestMethod]
        public async Task RefreshToken_WithValidToken_ShouldReturnSuccess()
        {
            // Arrange
            var token = Guid.NewGuid();
            var tokenEntity = new Token() { Id = token, LastUsed = _validLastUsedTime };
            _repository.GetByIdAsync(token).Returns(tokenEntity);
            _cache.GetAsync(Arg.Any<string>()).Returns(tokenEntity);

            // Act
            var result = await _tokenService.RefreshToken(token.ToString());

            // Assert
            Assert.IsTrue(result.Success);
            await _repository.Received(1).UpdateAsync(Arg.Is<Token>(t => t.Id == token && t.LastUsed > _validLastUsedTime));
        }

        [TestMethod]
        public async Task RefreshToken_WithNonExistentToken_ShouldReturnFailure()
        {
            // Arrange
            var token = Guid.NewGuid();
            _repository.GetByIdAsync(token).Returns((Token?)null);

            // Act
            var result = await _tokenService.RefreshToken(token.ToString());

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(ErrorCode.Unauthorized, result.Error.Code);
        }

        [TestMethod]
        public async Task RefreshToken_WithExpiredToken_ShouldReturnFailure()
        {
            // Arrange
            var token = Guid.NewGuid();
            var tokenEntity = new Token() { Id = token, LastUsed = _expiredLastUsedTime };
            _repository.GetByIdAsync(token).Returns(tokenEntity);
            _cache.GetAsync(Arg.Any<string>()).Returns(tokenEntity);

            // Act
            var result = await _tokenService.RefreshToken(token.ToString());

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(ErrorCode.Unauthorized, result.Error.Code);
        }
    }
}
