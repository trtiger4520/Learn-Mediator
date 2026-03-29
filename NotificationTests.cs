namespace Learn_Mediator;

public class NotificationTests
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
    public async Task Notification_Handler_Is_Invoked()
    {
        TestState.Clear();

        var provider = _services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        await mediator.Publish(new PingNotification("hi"));

        Assert.That(TestState.NotificationLogs, Does.Contain("hi"));
    }

    [Test]
    public async Task Multiple_Notification_Handlers_Are_Called()
    {
        TestState.Clear();

        var provider = _services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        await mediator.Publish(new PingNotification("multi"));

        var occurrences = TestState.NotificationLogs.Count(x => x == "multi");
        Assert.That(occurrences, Is.GreaterThanOrEqualTo(2));
    }
}

public record PingNotification(string Message) : INotification;

public class PingNotificationHandler : INotificationHandler<PingNotification>
{
    public ValueTask Handle(PingNotification notification, CancellationToken cancellationToken)
    {
        TestState.NotificationLogs.Add(notification.Message);
        return ValueTask.CompletedTask;
    }
}

public class PingNotificationHandler2 : INotificationHandler<PingNotification>
{
    public ValueTask Handle(PingNotification notification, CancellationToken cancellationToken)
    {
        TestState.NotificationLogs.Add(notification.Message);
        return ValueTask.CompletedTask;
    }
}
