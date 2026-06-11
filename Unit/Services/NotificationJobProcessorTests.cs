using Application.Abstractions;
using Application.Dtos;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;

namespace Unit.Services
{
    public class NotificationJobProcessorTests
    {
        private readonly Mock<IMessageRepository> _messageRepositoryMock = new();
        private readonly Mock<INotificationDispatcherService> _dispatcherMock = new();

        private NotificationJobProcessor CreateProcessor() =>
            new(_messageRepositoryMock.Object, _dispatcherMock.Object);

        [Fact]
        public async Task ProcessAsync_Should_Dispatch_When_Message_Found()
        {
            var message = new Message { Id = 1, Body = "Hello", CategoryId = 1 };
            var job = new NotificationJob(1, Guid.NewGuid());

            _messageRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(message);

            var processor = CreateProcessor();

            await processor.ProcessAsync(job, CancellationToken.None);

            _dispatcherMock.Verify(x => x.DispatchAsync(message), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_Should_Not_Dispatch_When_Message_Not_Found()
        {
            var job = new NotificationJob(99, Guid.NewGuid());

            _messageRepositoryMock
                .Setup(x => x.GetByIdAsync(99))
                .ReturnsAsync((Message?)null);

            var processor = CreateProcessor();

            await processor.ProcessAsync(job, CancellationToken.None);

            _dispatcherMock.Verify(
                x => x.DispatchAsync(It.IsAny<Message>()),
                Times.Never);
        }

        [Fact]
        public async Task ProcessAsync_Should_Propagate_Exception_From_Dispatcher()
        {
            var message = new Message { Id = 1, Body = "Hello", CategoryId = 1 };
            var job = new NotificationJob(1, Guid.NewGuid());

            _messageRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(message);

            _dispatcherMock
                .Setup(x => x.DispatchAsync(message))
                .ThrowsAsync(new InvalidOperationException("dispatch failed"));

            var processor = CreateProcessor();

            var act = async () => await processor.ProcessAsync(job, CancellationToken.None);

            await Assert.ThrowsAsync<InvalidOperationException>(act);
        }
    }
}