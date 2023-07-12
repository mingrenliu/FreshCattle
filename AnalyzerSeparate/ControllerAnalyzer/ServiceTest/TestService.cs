namespace ServiceTest
{
    public interface ITestService
    {
        void Display(string name);
    }
    public class TestService: ITestService
    {
        public void Display(string name) { }
    }
}