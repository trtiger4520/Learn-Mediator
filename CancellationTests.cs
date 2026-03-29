namespace Learn_Mediator;

public class CancellationTests
{
    ServiceCollection _services = null!;

    [SetUp]
    public void Setup()
    {
        _services = new ServiceCollection();
        _services.AddSingleton<ITestLogger, NoopTestLogger>();
        _services.AddMediator();
    }

    [Test]
    public void Cancellation_Is_Propagated()
    {
        var provider = _services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(50);

        Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await mediator.Send(new SlowRequest(), cts.Token);
        });
    }
}

public record SlowRequest() : IRequest<string>;
public class SlowRequestHandler : IRequestHandler<SlowRequest, string>
{
    public async ValueTask<string> Handle(SlowRequest request, CancellationToken cancellationToken)
    {
        await Task.Delay(Timeout.Infinite, cancellationToken);
        return "done";
    }
}
