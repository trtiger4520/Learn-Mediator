namespace Learn_Mediator;

public class ExceptionTests
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
    public void Handler_Exception_Is_Propagated()
    {
        var provider = _services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await mediator.Send(new FaultyRequest());
        });
    }
}

public record FaultyRequest() : IRequest<string>;
public class FaultyHandler : IRequestHandler<FaultyRequest, string>
{
    public ValueTask<string> Handle(FaultyRequest request, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException("fail");
    }
}
