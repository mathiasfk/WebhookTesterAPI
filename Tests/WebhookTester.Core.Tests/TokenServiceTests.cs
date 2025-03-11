using NSubstitute;
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
    }
}
