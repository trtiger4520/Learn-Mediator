namespace Learn_Mediator;

public class PipelineBehaviorTests
{
    ServiceCollection _services;

    [SetUp]
    public void Setup()
    {
        _services = [];
        _services.AddMediator(options =>
        {
            options.PipelineBehaviors = [typeof(LoggingBehavior<,>)];
        });
    }

    [Test]
    public async Task LoggingBehaviorIsInvoked()
    {
        var logger = new MyLogger();
        _services.AddSingleton<ITestLogger>(logger);
        var provider = _services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var response = await mediator.Send(new Ping("Hello, Pipeline!"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response, Is.EqualTo("Pong: Hello, Pipeline!"));
            Assert.That(logger._logs, Is.EquivalentTo(
            [
                "Handling Ping",
                "Handled Ping"
            ]));
        }
    }
}

public class LoggingBehavior<TRequest, TResponse>(
    ITestLogger logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async ValueTask<TResponse> Handle(
        TRequest message,
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        await logger.LogInformationAsync($"Handling {typeof(TRequest).Name}");
        var response = await next(message, cancellationToken);
        await logger.LogInformationAsync($"Handled {typeof(TRequest).Name}");
        return response;
    }
}

public class MyLogger : ITestLogger
{
    public readonly List<string> _logs = [];
    public Task LogInformationAsync(string message)
    {
        _logs.Add(message);
        return Task.CompletedTask;
    }
}

public interface ITestLogger
{
    Task LogInformationAsync(string message);
}