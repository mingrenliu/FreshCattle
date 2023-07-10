using System;

namespace AnalyzerTestLib
{
    public class TestController
    {

    }

    public class TestService:ITestService
    {
        private void Display(string name)
        {
            Console.WriteLine(name);
        }

        private void Display1(string name)
        {
            Console.WriteLine(name);
        }

        private void Display2(string name)
        {
            Console.WriteLine(name);
        }

        private void Display3(string name)
        {
            Console.WriteLine(name);
        }
    }

    public interface ITestService
    {
    }
}