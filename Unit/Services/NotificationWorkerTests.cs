using Application.Dtos;
using Application.Interfaces;
using Infrastructure.Queues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Unit.Services
{
    public class NotificationWorkerTests
    {
        private static IServiceProvider BuildProvider(INotificationJobProcessor processor)
        {
            var services = new ServiceCollection();
            services.AddSingleton(processor);
            return services.BuildServiceProvider();
        }

        [Fact]
        public async Task Worker_Should_Process_Enqueued_Job()
        {
            var queue = new NotificationQueue();
            var processorMock = new Mock<INotificationJobProcessor>();

            var worker = new NotificationWorker(
                BuildProvider(processorMock.Object),
                queue,
                NullLogger<NotificationWorker>.Instance);

            await worker.StartAsync(CancellationToken.None);

            var job = new NotificationJob(1, Guid.NewGuid());
            await queue.EnqueueAsync(job);

            await Task.Delay(200); // allow background loop to pick it up

            processorMock.Verify(
                x => x.ProcessAsync(
                    It.Is<NotificationJob>(j => j.MessageId == job.MessageId),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            await worker.StopAsync(CancellationToken.None);
        }

        [Fact]
        public async Task Worker_Should_Keep_Running_After_Processor_Throws()
        {
            var queue = new NotificationQueue();
            var processorMock = new Mock<INotificationJobProcessor>();

            processorMock
                .SetupSequence(x => x.ProcessAsync(It.IsAny<NotificationJob>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("boom"))
                .Returns(Task.CompletedTask);

            var worker = new NotificationWorker(
                BuildProvider(processorMock.Object),
                queue,
                NullLogger<NotificationWorker>.Instance);

            await worker.StartAsync(CancellationToken.None);

            await queue.EnqueueAsync(new NotificationJob(1, Guid.NewGuid()));
            await Task.Delay(100);

            await queue.EnqueueAsync(new NotificationJob(2, Guid.NewGuid()));
            await Task.Delay(100);

            // both jobs were processed despite the first one throwing
            processorMock.Verify(
                x => x.ProcessAsync(It.IsAny<NotificationJob>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2));

            await worker.StopAsync(CancellationToken.None);
        }
    }
}