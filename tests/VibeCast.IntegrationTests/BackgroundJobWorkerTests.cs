using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VibeCast.Infrastructure.Jobs;
using VibeCast.Infrastructure.Options;

namespace VibeCast.IntegrationTests;

[TestClass]
public sealed class BackgroundJobWorkerTests
{
    [TestMethod]
    public async Task StopAsync_WhileWaitingForJob_CompletesWithoutCancellation()
    {
        using var services = new ServiceCollection().BuildServiceProvider();
        var queue = new ChannelBackgroundJobQueue(Options.Create(new BackgroundJobsOptions()));
        using var worker = new BackgroundJobWorker(
            queue,
            services.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<BackgroundJobWorker>.Instance);

        await worker.StartAsync(CancellationToken.None);
        await worker.StopAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(5));

        Assert.IsNotNull(worker.ExecuteTask);
        Assert.IsTrue(worker.ExecuteTask.IsCompletedSuccessfully);
    }
}
