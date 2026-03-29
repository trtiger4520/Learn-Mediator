namespace Learn_Mediator;

public class GenericHandlerTests
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
    public async Task Closed_Generic_Handler_Registration_Works()
    {
        var provider = _services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new GenericRequestInt(123));
        Assert.That(result, Is.EqualTo(123));
    }
}

public record GenericRequestInt(int Value) : IRequest<int>;
public class GenericHandlerInt : IRequestHandler<GenericRequestInt, int>
{
    public ValueTask<int> Handle(GenericRequestInt request, CancellationToken cancellationToken) => new ValueTask<int>(request.Value);
}
