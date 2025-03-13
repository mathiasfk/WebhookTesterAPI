using WebhookTester.Core.Common;

namespace WebhookTester.Core.Tests.Common
{
    [TestClass]
    public sealed class OperationResultTests
    {
        [TestMethod]
        public void SuccessResult_ShouldReturnSuccess()
        {
            // Act
            var result = OperationResult.SuccessResult();

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.Error);
        }

        [TestMethod]
        public void FailureResult_ShouldReturnFailure()
        {
            // Arrange
            var errorMessage = "An error occurred";
            var errorCode = ErrorCode.InternalError;

            // Act
            var result = OperationResult.FailureResult(errorMessage, errorCode);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(errorMessage, result.Error.Message);
            Assert.AreEqual(errorCode, result.Error.Code);
        }

        [TestMethod]
        public void GenericSuccessResult_ShouldReturnSuccess()
        {
            // Arrange
            var data = "Test Data";

            // Act
            var result = OperationResult<string>.SuccessResult(data);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.Error);
            Assert.AreEqual(data, result.Data);
        }

        [TestMethod]
        public void GenericFailureResult_ShouldReturnFailure()
        {
            // Arrange
            var errorMessage = "An error occurred";
            var errorCode = ErrorCode.InternalError;

            // Act
            var result = OperationResult<string>.FailureResult(errorMessage, errorCode);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(errorMessage, result.Error.Message);
            Assert.AreEqual(errorCode, result.Error.Code);
        }
    }
}
