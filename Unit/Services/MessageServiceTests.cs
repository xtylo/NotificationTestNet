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
            Mock<INotificationDispatcherService> Dispatcher, 
            MessageService Service
            ) Setup()
        {
            var messageRepositoryMock =
                new Mock<IMessageRepository>();

            var dispatcherMock =
                new Mock<INotificationDispatcherService>();

            var categoryRepositoryMock =
                new Mock<ICategoryRepository>();

            categoryRepositoryMock
                .Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(true);

            var service = new MessageService(
                messageRepositoryMock.Object,
                dispatcherMock.Object,
                categoryRepositoryMock.Object);

            return(messageRepositoryMock, dispatcherMock, service);
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

            mocks.Dispatcher.Verify(
                x => x.DispatchAsync(
                    It.IsAny<Message>()),
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

            mocks.Dispatcher.Verify(
                x => x.DispatchAsync(
                    It.IsAny<Message>()),
                Times.Never);
        }

    }
}
