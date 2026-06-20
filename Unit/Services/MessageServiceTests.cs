using Application.Abstractions;
using Application.Dtos;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Unit.Services
{
    public class MessageServiceTests
    {

        private (
            Mock<IMessageRepository> Repository,
            Mock<INotificationPublisher> Publisher,
            MessageService Service
            ) Setup()
        {
            var messageRepositoryMock = new Mock<IMessageRepository>();

            var categoryRepositoryMock = new Mock<ICategoryRepository>();

            categoryRepositoryMock
                .Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(true);

            var notificationPublisherMock = new Mock<INotificationPublisher>();

            var service = new MessageService(
                messageRepositoryMock.Object,
                categoryRepositoryMock.Object,
                notificationPublisherMock.Object);

            return(messageRepositoryMock, notificationPublisherMock, service);
        }
      

        [Fact]
        public async Task CreateAsync_Should_Save_Message()
        {
            // Arrange
            var mocks  = Setup();

            var request = new CreateMessageDto
            {
                CategoryId = 1,
                Body = "Hello"
            };

            // Act
            await mocks.Service.CreateAsync(request);

            // Assert
            mocks.Repository.Verify(
                x => x.CreateAsync(
                    It.IsAny<Message>()),
                Times.Once);

            mocks.Publisher.Verify(
                x => x.PublishAsync(
                    It.IsAny<NotificationJob>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [Fact]
        public async Task CreateAsync_Should_Fail_InvalidCategory()
        {
            // Arrange
            var mocks = Setup();

            var request = new CreateMessageDto
            {
                CategoryId = 0,
                Body = "Hello"
            };

            // Act
            var action = async () => await mocks.Service.CreateAsync(request);

            // Assert
            await action.Should()
                .ThrowAsync<ArgumentException>();


            mocks.Repository.Verify(
                x => x.CreateAsync(
                    It.IsAny<Message>()),
                Times.Never);

            mocks.Publisher.Verify(
                x => x.PublishAsync(
                    It.IsAny<NotificationJob>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }


        [Fact]
        public async Task CreateAsync_Should_Fail_EmptyBody()
        {
            // Arrange
            var mocks = Setup();

            var request = new CreateMessageDto
            {
                CategoryId = 1,
                Body = String.Empty
            };

            // Act
            var action = async () => await mocks.Service.CreateAsync(request);

            // Assert
            await action.Should()
                .ThrowAsync<ArgumentException>();


            mocks.Repository.Verify(
                x => x.CreateAsync(
                    It.IsAny<Message>()),
                Times.Never);

            mocks.Publisher.Verify(
                x => x.PublishAsync(
                    It.IsAny<NotificationJob>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_Should_Fail_CategoryDoesNotExist()
        {
            // Arrange
            var mocks = Setup();

            var request = new CreateMessageDto
            {
                CategoryId = 99,
                Body = "Hello"
            };

            // Act
            var action = async () => await mocks.Service.CreateAsync(request);

            // Assert
            await action.Should()
                .ThrowAsync<ArgumentException>();

            mocks.Repository.Verify(
                x => x.CreateAsync(
                    It.IsAny<Message>()),
                Times.Never);

            mocks.Publisher.Verify(
                x => x.PublishAsync(
                    It.IsAny<NotificationJob>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_Should_Fail_WhitespaceBody()
        {
            // Arrange
            var mocks = Setup();

            var request = new CreateMessageDto
            {
                CategoryId = 1,
                Body = "   "
            };

            // Act
            var action = async () => await mocks.Service.CreateAsync(request);

            // Assert
            await action.Should()
                .ThrowAsync<ArgumentException>();

            mocks.Repository.Verify(
                x => x.CreateAsync(
                    It.IsAny<Message>()),
                Times.Never);

            mocks.Publisher.Verify(
                x => x.PublishAsync(
                    It.IsAny<NotificationJob>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

    }
}
