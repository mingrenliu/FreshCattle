using ServiceTest;
using System.Threading.Tasks;

namespace ControllerTest
{
    public class TestController
    {
        private readonly ITestService _testService;

        /// <summary>
        /// 
        /// </summary>
        /// <paramtestService="testService"></param>
        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <paramid="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task Delete(string id)
        {
            await _testService.DeleteAsync(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <paramid="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Get(string id)
        {
            return await _testService.Get(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <paramname="name"></param>
        /// <returns></returns>
        [HttpPost]
        public void Display(string name)
        {
            _testService.Display(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <paramname="name"></param>
        /// <returns></returns>
        [HttpPost]
        public string GetName(string name)
        {
            return _testService.GetName(name);
        }
    }
}