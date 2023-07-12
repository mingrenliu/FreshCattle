namespace InheritAnalyzerTest
{
    public class Class1
    {
        private readonly ITest test; 
        public string Getstring(string name,IEnumerable<string> list)
        {
            return test.Getstring(name,list);
        }
        //public string
    }

    public interface ITest
    {
        string Getstring(string name, IEnumerable<string> list);
    }
}