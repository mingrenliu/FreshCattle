namespace ServiceTest
{
    public interface ITestService
    {
        Task DeleteAsync(string id);

        Task<string> Get(string id);

        void Display(string name);

        string GetName(string name);
    }
}