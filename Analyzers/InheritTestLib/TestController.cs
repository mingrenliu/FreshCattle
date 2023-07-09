namespace InheritTestLib
{
    public class TestController
    {
        private readonly ITestService _testService;
        public void Test1(string name)
        {
            _testService.Display(name);
        }
    }
    public interface ITestService
    {
        void Display1(string message);
        void Display(string message);
    }
}
