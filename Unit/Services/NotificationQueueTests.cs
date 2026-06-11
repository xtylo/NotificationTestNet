using Application.Dtos;
using Infrastructure.Queues;

namespace Unit.Services
{
    public class NotificationQueueTests
    {

        [Fact]
        public async Task Message_Many_Should_Enqueue_In_Order()
        {

            var queue = new NotificationQueue();

            var job1 = new NotificationJob(1, Guid.NewGuid());
            var job2 = new NotificationJob(2, Guid.NewGuid());
            var job3 = new NotificationJob(3, Guid.NewGuid());

            await queue.EnqueueAsync(job1);
            await queue.EnqueueAsync(job2);
            await queue.EnqueueAsync(job3);

            var results = new List<NotificationJob>
            {
                await queue.DequeueAsync(CancellationToken.None),
                await queue.DequeueAsync(CancellationToken.None),
                await queue.DequeueAsync(CancellationToken.None)
            };

            Assert.Collection(
                results,
                item => Assert.Equal(job1.MessageId, item.MessageId),
                item => Assert.Equal(job2.MessageId, item.MessageId),
                item => Assert.Equal(job3.MessageId, item.MessageId));

        }

        [Fact]
        public async Task DequeueAsync_Should_Return_Enqueued_Message()
        {
            var queue = new NotificationQueue();

            var job = new NotificationJob(1, Guid.NewGuid());

            await queue.EnqueueAsync(job);

            var result = await queue.DequeueAsync(CancellationToken.None);

            Assert.Equal(job.MessageId, result.MessageId);
            Assert.Equal(job.CorrelationId, result.CorrelationId);
        }

        [Fact]
        public async Task DequeueAsync_Should_Wait_Until_Message_Arrives()
        {
            var queue = new NotificationQueue();

            var dequeueTask = queue.DequeueAsync(
                CancellationToken.None);

            // Give the dequeue operation a moment to start
            await Task.Delay(100);

            Assert.False(dequeueTask.IsCompleted);

            var job = new NotificationJob(
                1,
                Guid.NewGuid());

            await queue.EnqueueAsync(job);

            var result = await dequeueTask;

            Assert.Equal(job.MessageId, result.MessageId);
        }

        [Fact]
        public async Task DequeueAsync_Should_Throw_When_Cancelled()
        {
            var queue = new NotificationQueue();

            using var cts = new CancellationTokenSource();

            cts.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(
                async () =>
                    await queue.DequeueAsync(
                        cts.Token));
        }

    }
}
