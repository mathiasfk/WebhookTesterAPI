﻿using NSubstitute;
using WebhookTester.Core.Entities;
using WebhookTester.Core.Interfaces;
using WebhookTester.Core.Services;

namespace WebhookTester.Core.Tests
{
    [TestClass]
    public class WebhooksServiceTests
    {
        private readonly IWebhooksRepository _repository = Substitute.For<IWebhooksRepository>();
        private readonly IServerSentEventsService _sse = Substitute.For<IServerSentEventsService>();
        private WebhooksService _webhooksService = null!;

        [TestInitialize]
        public void Setup()
        {
            _webhooksService = new WebhooksService(_repository, _sse);
        }

        [TestMethod]
        public async Task CreateWebhook_ShouldReturnNewWebhook()
        {
            // Arrange
            var token = Guid.NewGuid();
            _repository.AddAsync(Arg.Any<Webhook>()).Returns(Task.CompletedTask);

            // Act
            var result = await _webhooksService.CreateWebhook(token);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(token, result.OwnerToken);
            await _repository.Received(1).AddAsync(Arg.Any<Webhook>());
        }

        [TestMethod]
        public async Task ListWebhooks_WhenWebhooksExist_ShouldReturnList()
        {
            // Arrange
            var token = Guid.NewGuid();
            var webhooks = new List<Webhook> { new() { OwnerToken = token } };
            _repository.GetByTokenAsync(token).Returns(webhooks);

            // Act
            var result = await _webhooksService.ListWebhooks(token);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(token, result.First().OwnerToken);
        }

        [TestMethod]
        public async Task ListWebhooks_WhenWebhooksDontExist_ShouldReturnEmptyList()
        {
            // Arrange
            var token = Guid.NewGuid();
            var webhooks = new List<Webhook> {};
            _repository.GetByTokenAsync(token).Returns(webhooks);

            // Act
            var result = await _webhooksService.ListWebhooks(token);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task DeleteWebhook_WhenWebhookExists_ShouldReturnTrue()
        {
            // Arrange
            var token = Guid.NewGuid();
            var webhookId = Guid.NewGuid();
            var webhook = new Webhook { Id = webhookId, OwnerToken = token };
            _repository.GetByIdAsync(webhookId).Returns(webhook);
            _repository.RemoveAsync(webhook).Returns(Task.CompletedTask);

            // Act
            var result = await _webhooksService.DeleteWebhook(token, webhookId);

            // Assert
            Assert.IsTrue(result);
            await _repository.Received(1).RemoveAsync(webhook);
        }

        [TestMethod]
        public async Task DeleteWebhook_WhenWebhookDoesntExist_ShouldReturnFalse()
        {
            // Arrange
            var token = Guid.NewGuid();
            var webhookId = Guid.NewGuid();
            _repository.GetByIdAsync(webhookId).Returns(Task.FromResult<Webhook?>(null));

            // Act
            var result = await _webhooksService.DeleteWebhook(token, webhookId);

            // Assert
            Assert.IsFalse(result);
            await _repository.Received(0).RemoveAsync(Arg.Any<Webhook>());
        }

        [TestMethod]
        public async Task GetRequests_WhenWebhookExists_ShouldReturnRequests()
        {
            // Arrange
            var token = Guid.NewGuid();
            var webhookId = Guid.NewGuid();
            var requests = new List<WebhookRequest> { new() { WebhookId = webhookId } };
            var webhook = new Webhook { Id = webhookId, OwnerToken = token, Requests = requests };
            _repository.GetByIdAsync(webhookId).Returns(webhook);

            // Act
            var result = await _webhooksService.GetRequests(token, webhookId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(webhookId, result.First().WebhookId);
        }

        [TestMethod]
        public async Task GetRequests_WhenWebhookDoesntHaveRequests_ShouldReturnEmptyList()
        {
            // Arrange
            var token = Guid.NewGuid();
            var webhookId = Guid.NewGuid();
            var requests = new List<WebhookRequest> {};
            var webhook = new Webhook { Id = webhookId, OwnerToken = token, Requests = requests };
            _repository.GetByIdAsync(webhookId).Returns(webhook);

            // Act
            var result = await _webhooksService.GetRequests(token, webhookId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetRequests_WhenWebhookDoesntExist_ShouldReturnEmptyList()
        {
            // Arrange
            var token = Guid.NewGuid();
            var webhookId = Guid.NewGuid();
            _repository.GetByIdAsync(webhookId).Returns(Task.FromResult<Webhook?>(null));

            // Act
            var result = await _webhooksService.GetRequests(token, webhookId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task HandleRequestAsync_WhenWebhookExists_ShouldReturnTrue()
        {
            // Arrange
            var webhookId = Guid.NewGuid();
            var request = new WebhookRequest { WebhookId = webhookId };
            var webhook = new Webhook { Id = webhookId };
            _repository.GetByIdAsync(webhookId).Returns(webhook);
            _repository.AddRequestAsync(request).Returns(Task.CompletedTask);
            _sse.WriteToChannelAsync(webhookId, request).Returns(Task.CompletedTask);

            // Act
            var result = await _webhooksService.HandleRequestAsync(webhookId, request);

            // Assert
            Assert.IsTrue(result);
            await _repository.Received(1).AddRequestAsync(request);
            await _sse.Received(1).WriteToChannelAsync(webhookId, request);
        }

        [TestMethod]
        public async Task HandleRequestAsync_WhenWebhookDoesntExist_ShouldReturnFalse()
        {
            // Arrange
            var webhookId = Guid.NewGuid();
            var request = new WebhookRequest { WebhookId = webhookId };
            _repository.GetByIdAsync(webhookId).Returns(Task.FromResult<Webhook?>(null));
            _sse.WriteToChannelAsync(webhookId, request).Returns(Task.CompletedTask);

            // Act
            var result = await _webhooksService.HandleRequestAsync(webhookId, request);

            // Assert
            Assert.IsFalse(result);
            await _repository.Received(0).AddRequestAsync(Arg.Any<WebhookRequest>());
            await _sse.Received(0).WriteToChannelAsync(Arg.Any<Guid>(), Arg.Any<WebhookRequest>());
        }
    }
}
