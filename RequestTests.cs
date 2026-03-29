namespace Learn_Mediator;

public class RequestTests
{
    ServiceCollection _services;

    [SetUp]
    public void Setup()
    {
        _services = [];
        _services.AddSingleton<ITestLogger, NoopTestLogger>();
        _services.AddMediator();
    }

    [Test]
    public async Task GetValueTypeResponse()
    {
        var provider = _services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var response = await mediator.Send(new Ping("Hello, Mediator!"));
        Assert.That(response, Is.EqualTo("Pong: Hello, Mediator!"));
    }

    [Test]
    public async Task GetReferenceTypeResponse()
    {
        var provider = _services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var response = await mediator.Send(new GetOrderV1());
        Assert.That(response, Is.EqualTo(new OrderV1("Widget", 10)));
    }

    [TearDown]
    public void TearDown()
    {
        _services.Clear();
    }
}

public record Ping(string Message) : IRequest<string>;

public class PingHandler : IRequestHandler<Ping, string>
{
    public async ValueTask<string> Handle(
        Ping request,
        CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);
        return $"Pong: {request.Message}";
    }
}

public record OrderV1(string ProductName, int Quantity);

public record GetOrderV1() : IRequest<OrderV1>;

public class GetOrderV1Handler : IRequestHandler<GetOrderV1, OrderV1>
{
    public async ValueTask<OrderV1> Handle(
        GetOrderV1 request,
        CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);
        return new OrderV1("Widget", 10);
    }
}

