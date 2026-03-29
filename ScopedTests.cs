namespace Learn_Mediator;

public class ScopedTests
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
    public async Task Scoped_Dependency_Is_Fresh_Per_Scope()
    {
        // verify scoped DI behaviour (use scopes directly, not mediator)
        _services.AddScoped<IScopedCounter, ScopedCounter>();

        var provider = _services.BuildServiceProvider();

        Guid id1;
        using (var scope = provider.CreateScope())
        {
            var c1 = scope.ServiceProvider.GetRequiredService<IScopedCounter>();
            var c1b = scope.ServiceProvider.GetRequiredService<IScopedCounter>();
            id1 = c1.Id;
            Assert.That(c1b.Id, Is.EqualTo(id1));
        }

        using (var scope2 = provider.CreateScope())
        {
            var c2 = scope2.ServiceProvider.GetRequiredService<IScopedCounter>();
            Assert.That(c2.Id, Is.Not.EqualTo(id1));
        }
    }
}

public interface IScopedCounter { Guid Id { get; } }
public class ScopedCounter : IScopedCounter { public Guid Id { get; } = Guid.NewGuid(); }
