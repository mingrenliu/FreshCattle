namespace ExcelUtileTest.Utilities;

[SetUpFixture]
internal abstract class TestBase
{
    [OneTimeSetUp]
    public virtual void Init()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    [OneTimeTearDown]
    public virtual void Dispose()
    {
        Trace.Flush();
    }
}