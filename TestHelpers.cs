namespace Learn_Mediator;

public static class TestState
{
    static readonly object _lock = new object();
    public static List<string> NotificationLogs { get; } = new List<string>();
    public static List<string> BehaviorLogs { get; } = new List<string>();
    public static void Clear()
    {
        lock (_lock)
        {
            NotificationLogs.Clear();
            BehaviorLogs.Clear();
        }
    }
}

public class NoopTestLogger : ITestLogger
{
    public Task LogInformationAsync(string message) => Task.CompletedTask;
}
