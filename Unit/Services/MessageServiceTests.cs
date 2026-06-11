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
            Mock<INotificationQueue> Queue, 
            MessageService Service
            ) Setup()
        {
            var messageRepositoryMock = new Mock<IMessageRepository>();

            var categoryRepositoryMock = new Mock<ICategoryRepository>();

            categoryRepositoryMock
                .Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(true);

            var notificationQueueMock = new Mock<INotificationQueue>();

            var service = new MessageService(
                messageRepositoryMock.Object,
                categoryRepositoryMock.Object,
                notificationQueueMock.Object);

            return(messageRepositoryMock, notificationQueueMock, service);
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

            mocks.Queue.Verify(
                x => x.EnqueueAsync(
                    It.IsAny<NotificationJob>()),
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

            mocks.Queue.Verify(
                x => x.EnqueueAsync(
                    It.IsAny<NotificationJob>()),
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

            mocks.Queue.Verify(
                x => x.EnqueueAsync(
                    It.IsAny<NotificationJob>()),
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

            mocks.Queue.Verify(
                x => x.EnqueueAsync(
                    It.IsAny<NotificationJob>()),
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

            mocks.Queue.Verify(
                x => x.EnqueueAsync(
                    It.IsAny<NotificationJob>()),
                Times.Never);
        }

    }
}
