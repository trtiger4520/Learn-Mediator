namespace Learn_Mediator;

public class PipelineBehaviorOrderTests
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
    public async Task Multiple_Pipeline_Behaviors_Invoke_In_Order()
    {
        TestState.Clear();
        _services.AddMediator(options =>
        {
            options.PipelineBehaviors = new[] { typeof(BehaviorA<,>), typeof(BehaviorB<,>) };
        });

        var provider = _services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var response = await mediator.Send(new Ping("PipelineOrder"));
        Assert.That(response, Is.EqualTo("Pong: PipelineOrder"));

        var logs = TestState.BehaviorLogs;
        int iA1 = logs.IndexOf("A before");
        int iB1 = logs.IndexOf("B before", iA1 + 1);
        int iB2 = logs.IndexOf("B after", iB1 + 1);
        int iA2 = logs.IndexOf("A after", iB2 + 1);
        Assert.That(iA1, Is.GreaterThanOrEqualTo(0));
        Assert.That(iB1, Is.GreaterThan(iA1));
        Assert.That(iB2, Is.GreaterThan(iB1));
        Assert.That(iA2, Is.GreaterThan(iB2));
    }
}

public class BehaviorA<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public BehaviorA() { }
    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        TestState.BehaviorLogs.Add("A before");
        var res = await next(message, cancellationToken);
        TestState.BehaviorLogs.Add("A after");
        return res;
    }
}

public class BehaviorB<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public BehaviorB() { }
    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        TestState.BehaviorLogs.Add("B before");
        var res = await next(message, cancellationToken);
        TestState.BehaviorLogs.Add("B after");
        return res;
    }
}
